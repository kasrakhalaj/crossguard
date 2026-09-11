using UnityEngine;
using UnityEngine.InputSystem;

namespace Crossguard.Combat
{
    /// <summary>
    /// Player character controller implementing Option A:
    /// - Right Stick / Mouse: 2D Clock Face Aiming (Direction/Chamber).
    /// - RT / Left Click / Space: Trigger Slash (Committed Strike along aim plane).
    /// - RB / Right Click / R: Trigger Thrust (Forward extension along depth).
    /// - LB / LT / D-Pad / Q,E,Z,C: Discrete Shield Stances.
    /// - Left Stick / WASD: Footwork Movement.
    /// </summary>
    public class DuelistController : MonoBehaviour
    {
        [Header("Subsystem References")]
        [SerializeField] private SwordPlaneController swordController;
        [SerializeField] private ShieldStanceManager shieldController;
        [SerializeField] private CharacterController characterController;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        [Header("Stance Behavior")]
        [Tooltip("If true, pressing a stance button latches into that stance until another is pressed.")]
        [SerializeField] private bool stickyStances = true;

        [Header("Mouse Controls")]
        [Tooltip("Use mouse position relative to screen center for sword plane angle when gamepad is not present.")]
        [SerializeField] private bool useScreenRelativeMouse = true;

        private Vector2 moveInput;
        private Vector2 swordInput;
        private Vector3 velocity;

        [Header("Right Stick Gesture Kinematics")]
        [Tooltip("Angular velocity threshold (deg/s) for triggering a Slash via right stick sweep.")]
        [SerializeField] private float slashAngularSpeedThreshold = 350f;

        [Tooltip("Minimum angular sweep (deg) accumulated over a fast sweep window.")]
        [SerializeField] private float slashMinSweepAngle = 35f;

        [Tooltip("Radial deflection threshold (0..1) required before an angular sweep registers as a slash.")]
        [SerializeField] private float slashMinDeflection = 0.45f;

        [Tooltip("Radial expansion velocity (1/s) required for triggering a Thrust.")]
        [SerializeField] private float thrustRadialVelocityThreshold = 4.5f;

        [Tooltip("Minimum stick deflection (0..1) reached to complete a Thrust.")]
        [SerializeField] private float thrustMinDeflection = 0.82f;

        [Tooltip("Maximum angular deviation permitted during a pure radial push.")]
        [SerializeField] private float thrustMaxAngularDeviation = 28f;

        [Tooltip("Cooldown after triggering an attack gesture before another gesture can register.")]
        [SerializeField] private float gestureCooldownDuration = 0.18f;

        // Stick gesture state
        private float prevStickAngle = 0f;
        private float prevStickMag = 0f;
        private float gestureCooldownTimer = 0f;
        private float sweepAngleAccumulator = 0f;
        private float sweepWindowTimer = 0f;
        private float thrustStartMag = 0f;
        private float thrustStartAngle = 0f;
        private float thrustWindowTimer = 0f;
        private bool wasStickDeflected = false;
        private Vector2 mouseAimDirection = new Vector2(0.707f, 0.707f);

        private void Awake()
        {
            FindSubsystems();
        }

        private void Start()
        {
            FindSubsystems();
        }

        private void FindSubsystems()
        {
            if (swordController == null)
                swordController = GetComponentInChildren<SwordPlaneController>();

            if (shieldController == null)
                shieldController = GetComponentInChildren<ShieldStanceManager>();

            if (characterController == null)
                characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            ReadInputs();
            HandleMovement();
            HandleSwordAiming();
            HandleAttacks();
            HandleShieldStances();
        }

        private void ReadInputs()
        {
            moveInput = Vector2.zero;
            swordInput = Vector2.zero;

            // 1. Gamepad Input (Primary Target)
            Gamepad pad = Gamepad.current;
            if (pad != null)
            {
                moveInput = pad.leftStick.ReadValue();
                Vector2 stick = pad.rightStick.ReadValue();
                if (stick.sqrMagnitude > 0.04f)
                {
                    swordInput = stick;
                }
            }

            // 2. Keyboard & Mouse Fallback
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (kb != null)
            {
                Vector2 kbMove = Vector2.zero;
                if (kb.wKey.isPressed) kbMove.y += 1f;
                if (kb.sKey.isPressed) kbMove.y -= 1f;
                if (kb.aKey.isPressed) kbMove.x -= 1f;
                if (kb.dKey.isPressed) kbMove.x += 1f;

                if (kbMove.sqrMagnitude > 0.01f)
                    moveInput = kbMove.normalized;
            }

            if (mouse != null && swordInput == Vector2.zero)
            {
                Vector2 mouseDelta = mouse.delta.ReadValue();
                if (mouseDelta.sqrMagnitude > 0.5f)
                {
                    mouseAimDirection += mouseDelta * 0.04f;
                    if (mouseAimDirection.sqrMagnitude > 0.01f)
                        mouseAimDirection.Normalize();
                }

                if (useScreenRelativeMouse)
                {
                    Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    Vector2 mousePos = mouse.position.ReadValue();
                    Vector2 dirFromCenter = mousePos - screenCenter;
                    if (dirFromCenter.sqrMagnitude > 64f)
                    {
                        swordInput = dirFromCenter.normalized;
                    }
                    else
                    {
                        swordInput = mouseAimDirection;
                    }
                }
                else
                {
                    swordInput = mouseAimDirection;
                }
            }
        }

        [Header("Duel Lock-On")]
        [SerializeField] private Transform opponentTarget;

        private void HandleLockOnToggle()
        {
            Gamepad pad = Gamepad.current;
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;

            bool toggle = false;
            if (kb != null && kb.tabKey.wasPressedThisFrame) toggle = true;
            if (mouse != null && mouse.middleButton.wasPressedThisFrame) toggle = true;
            if (pad != null && pad.rightStickButton.wasPressedThisFrame) toggle = true; // R3

            if (toggle && CenteredDuelCamera.Instance != null)
            {
                CenteredDuelCamera.Instance.ToggleLockOn();
            }
        }

        private void HandleMovement()
        {
            HandleLockOnToggle();

            if (opponentTarget == null)
            {
                GameObject dummy = GameObject.Find("Target_Dummy");
                if (dummy != null) opponentTarget = dummy.transform;
            }

            bool isLocked = (CenteredDuelCamera.Instance == null || CenteredDuelCamera.Instance.IsLockedOn);
            Vector3 moveDirection;

            if (isLocked && opponentTarget != null)
            {
                // Locked-on Duelist Footwork: Face opponent, strafe around orbit
                Vector3 toOpponent = opponentTarget.position - transform.position;
                toOpponent.y = 0f;

                if (toOpponent.sqrMagnitude > 0.01f)
                {
                    Vector3 duelForward = toOpponent.normalized;
                    Vector3 duelRight = Vector3.Cross(Vector3.up, duelForward).normalized;

                    // W/S = Advance/Retreat, A/D = Circular Strafe
                    moveDirection = (duelForward * moveInput.y) + (duelRight * moveInput.x);

                    // Maintain visual lock-on facing the opponent
                    Quaternion targetRot = Quaternion.LookRotation(duelForward, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
                else
                {
                    moveDirection = Vector3.zero;
                }
            }
            else
            {
                // Free Movement (Unlocked Mode)
                Camera cam = Camera.main;
                Vector3 forward = (cam != null) ? Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized : Vector3.forward;
                Vector3 right = (cam != null) ? Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized : Vector3.right;

                moveDirection = (forward * moveInput.y) + (right * moveInput.x);

                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDirection, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }

            if (characterController != null)
            {
                if (characterController.isGrounded)
                {
                    velocity.y = -0.5f;
                }
                else
                {
                    velocity.y += Physics.gravity.y * Time.deltaTime;
                }

                Vector3 motion = (moveDirection * moveSpeed + velocity) * Time.deltaTime;
                characterController.Move(motion);
            }
            else
            {
                transform.position += moveDirection * (moveSpeed * Time.deltaTime);
            }
        }

        private void HandleSwordAiming()
        {
            if (swordController != null && swordInput.sqrMagnitude > 0.01f)
            {
                swordController.SetInputVector(swordInput);
            }
        }

        private void HandleAttacks()
        {
            if (swordController == null) return;

            Gamepad pad = Gamepad.current;
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;

            // 1. Gamepad Right Stick Gestures (Fast Angular Sweep = Slash, Fast Radial Push = Thrust)
            if (pad != null)
            {
                UpdateRightStickGestures(pad);

                // Optional face button fallbacks
                if (pad.buttonWest.wasPressedThisFrame) // X
                {
                    swordController.TriggerSlash();
                    return;
                }
                if (pad.buttonNorth.wasPressedThisFrame) // Y
                {
                    swordController.TriggerThrust();
                    return;
                }
            }

            // 2. Keyboard & Mouse Attacks (Explicit Buttons)
            bool slashPressed = false;
            bool thrustPressed = false;

            if (mouse != null)
            {
                if (mouse.leftButton.wasPressedThisFrame) slashPressed = true;
                if (mouse.rightButton.wasPressedThisFrame) thrustPressed = true;
            }

            if (kb != null)
            {
                if (kb.spaceKey.wasPressedThisFrame) slashPressed = true;
                if (kb.rKey.wasPressedThisFrame) thrustPressed = true;
            }

            if (slashPressed)
            {
                swordController.TriggerSlash();
                return;
            }

            if (thrustPressed)
            {
                swordController.TriggerThrust();
            }
        }

        /// <summary>
        /// Analyzes Right Stick motion shape to distinguish Slash (angular sweep) from Thrust (radial punch)
        /// while thumbs stay permanently on the sticks.
        /// </summary>
        private void UpdateRightStickGestures(Gamepad pad)
        {
            if (pad == null || swordController == null) return;

            if (gestureCooldownTimer > 0f)
            {
                gestureCooldownTimer -= Time.deltaTime;
                return;
            }

            if (swordController.State != AttackState.Ready)
            {
                sweepAngleAccumulator = 0f;
                sweepWindowTimer = 0f;
                thrustWindowTimer = 0f;
                return;
            }

            Vector2 stick = pad.rightStick.ReadValue();
            float mag = stick.magnitude;
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            if (mag > 0.12f)
            {
                float angle = Mathf.Atan2(stick.y, stick.x) * Mathf.Rad2Deg;

                if (prevStickMag > 0.08f)
                {
                    float deltaAngle = Mathf.DeltaAngle(prevStickAngle, angle);
                    float angularSpeed = Mathf.Abs(deltaAngle) / dt;
                    float radialVelocity = (mag - prevStickMag) / dt;

                    // 1. Thrust Gesture: Fast radial push outward with little/no angular sweep
                    if (!wasStickDeflected && mag < 0.38f)
                    {
                        thrustStartMag = mag;
                        thrustStartAngle = angle;
                        thrustWindowTimer = 0f;
                    }
                    else if (thrustWindowTimer < 0.15f)
                    {
                        thrustWindowTimer += dt;
                        float totalAngularDelta = Mathf.Abs(Mathf.DeltaAngle(thrustStartAngle, angle));

                        if (mag >= thrustMinDeflection &&
                            radialVelocity > thrustRadialVelocityThreshold &&
                            totalAngularDelta <= thrustMaxAngularDeviation)
                        {
                            swordController.TriggerThrust();
                            gestureCooldownTimer = gestureCooldownDuration;
                            sweepAngleAccumulator = 0f;
                            thrustWindowTimer = 0f;
                            prevStickAngle = angle;
                            prevStickMag = mag;
                            wasStickDeflected = true;
                            return;
                        }
                    }

                    // 2. Slash Gesture: Fast angular sweep past velocity threshold
                    if (mag >= slashMinDeflection)
                    {
                        sweepWindowTimer += dt;
                        if (sweepWindowTimer > 0.12f)
                        {
                            sweepAngleAccumulator = 0f;
                            sweepWindowTimer = 0f;
                        }

                        sweepAngleAccumulator += deltaAngle;

                        if (angularSpeed >= slashAngularSpeedThreshold || Mathf.Abs(sweepAngleAccumulator) >= slashMinSweepAngle)
                        {
                            swordController.TriggerSlash();
                            gestureCooldownTimer = gestureCooldownDuration;
                            sweepAngleAccumulator = 0f;
                            sweepWindowTimer = 0f;
                            prevStickAngle = angle;
                            prevStickMag = mag;
                            wasStickDeflected = true;
                            return;
                        }
                    }
                    else
                    {
                        sweepAngleAccumulator = 0f;
                        sweepWindowTimer = 0f;
                    }
                }

                prevStickAngle = angle;
            }
            else
            {
                sweepAngleAccumulator = 0f;
                sweepWindowTimer = 0f;
                thrustWindowTimer = 0f;
            }

            wasStickDeflected = (mag >= slashMinDeflection);
            prevStickMag = mag;
        }

        /// <summary>
        /// Maps discrete 4-quadrant shield stances:
        /// Gamepad: LB = High-Left, LT = Low-Left, RB = High-Right, RT = Low-Right (Index/Middle fingers).
        /// KB+M: Q = High-Left, E = High-Right, Z = Low-Left, C = Low-Right.
        /// </summary>
        private void HandleShieldStances()
        {
            if (shieldController == null) return;

            Gamepad pad = Gamepad.current;
            Keyboard kb = Keyboard.current;

            bool hl = false;
            bool hr = false;
            bool ll = false;
            bool lr = false;

            // Gamepad 4-quadrant shoulder mapping
            if (pad != null)
            {
                if (pad.leftShoulder.isPressed) hl = true;
                if (pad.leftTrigger.ReadValue() > 0.3f) ll = true;
                if (pad.rightShoulder.isPressed) hr = true;
                if (pad.rightTrigger.ReadValue() > 0.3f) lr = true;

                // D-Pad fallbacks
                if (pad.dpad.left.isPressed) hl = true;
                if (pad.dpad.up.isPressed) hr = true;
                if (pad.dpad.down.isPressed) ll = true;
                if (pad.dpad.right.isPressed) lr = true;
            }

            // KB+M spatial layout around WASD
            if (kb != null)
            {
                if (kb.qKey.isPressed || kb.digit1Key.isPressed) hl = true;
                if (kb.eKey.isPressed || kb.digit2Key.isPressed) hr = true;
                if (kb.zKey.isPressed || kb.digit3Key.isPressed) ll = true;
                if (kb.cKey.isPressed || kb.digit4Key.isPressed) lr = true;
            }

            if (hl)
            {
                shieldController.SetStance(ShieldStance.HighLeft);
            }
            else if (hr)
            {
                shieldController.SetStance(ShieldStance.HighRight);
            }
            else if (ll)
            {
                shieldController.SetStance(ShieldStance.LowLeft);
            }
            else if (lr)
            {
                shieldController.SetStance(ShieldStance.LowRight);
            }
            else if (!stickyStances)
            {
                shieldController.SetStance(ShieldStance.Neutral);
            }
        }
    }
}
