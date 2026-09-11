using System;
using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Advanced Martial Longsword Controller:
    /// - 2D Clock Face chamber aiming.
    /// - Authentic 3D anatomical hand trajectory (driving forward and across the body).
    /// - True Edge Alignment (Hasuji) so cuts slice blade-first, eliminating the paddle slap.
    /// - Human martial timing: 0.26s Windup Telegraph -> 0.22s Explosive Apex Cut -> 0.30s Follow-Through Recovery.
    /// - Continuous raycast sweep along hilt, mid-blade, and tip to prevent physics tunneling.
    /// </summary>
    public class SwordPlaneController : MonoBehaviour
    {
        [Header("Transforms")]
        [Tooltip("The root hand/shoulder pivot transform.")]
        [SerializeField] private Transform swordPivot;

        [Tooltip("Transform marking the tip of the blade.")]
        [SerializeField] private Transform bladeTip;

        [Tooltip("Collider component on the blade used for hit detection.")]
        [SerializeField] private Collider bladeCollider;

        [Tooltip("TrailRenderer attached to the blade for visualizing the swing plane.")]
        [SerializeField] private TrailRenderer bladeTrail;

        [Header("2D Clock Face Aiming (Guard Chamber)")]
        [Tooltip("Base hand position relative to character center.")]
        [SerializeField] private Vector3 baseHandOffset = new Vector3(0.10f, 0.10f, 0.48f);

        [Tooltip("Radius of the 2D clock face chamber ring.")]
        [SerializeField] private float chamberRadius = 0.28f;

        [Tooltip("Smooth time for aiming the chamber angle.")]
        [SerializeField] private float aimSmoothTime = 0.05f;

        [Header("Martial Cut Timing (Human Perception)")]
        [Tooltip("Wind-up telegraph duration: readable draw-back pose.")]
        [SerializeField] private float slashWindupDuration = 0.26f;

        [Tooltip("Apex strike duration: explosive acceleration driving through target.")]
        [SerializeField] private float slashActiveDuration = 0.22f;

        [Tooltip("Follow-through and recovery back to guard.")]
        [SerializeField] private float slashRecoveryDuration = 0.30f;

        [Tooltip("Forward reach extension driving into the cut.")]
        [SerializeField] private float forwardCutDrive = 0.45f;

        [Header("Thrust Timing & Reach")]
        [SerializeField] private float thrustWindupDuration = 0.18f;
        [SerializeField] private float thrustActiveDuration = 0.16f;
        [SerializeField] private float thrustRecoveryDuration = 0.28f;
        [SerializeField] private float thrustReachDistance = 0.85f;

        [Header("Recoil Dynamics")]
        [SerializeField] private float recoilDuration = 0.18f;

        // State Machine
        private AttackState state = AttackState.Ready;
        private AttackType currentAttackType = AttackType.None;
        private float stateTimer = 0f;

        // Aiming State
        private float currentChamberAngle = 45f; // High-Right default
        private float targetChamberAngle = 45f;
        private float aimAngularVelocity = 0f;

        // Anatomical Cut Trajectory Vectors
        private Vector3 strikeStartHandPos;
        private Vector3 strikeEndHandPos;
        private Quaternion strikeStartRot;
        private Quaternion strikeEndRot;
        private Quaternion windupCockRot;

        private Vector3 recoilHandPos;
        private Quaternion recoilRot;

        // Continuous Sweep Tracking (Anti-tunneling)
        private Vector3 prevHiltPos;
        private Vector3 prevMidPos;
        private Vector3 prevTipPos;
        private Vector3 strikeVelocity;
        private Vector3 strikeDirection;

        public AttackState State => state;
        public AttackType CurrentAttackType => currentAttackType;
        public bool IsAttacking => state == AttackState.ActiveSlash || state == AttackState.ActiveThrust;
        public float ChamberAngle => currentChamberAngle;
        public Vector3 StrikeDirection => strikeDirection;
        public float StrikeSpeed => strikeVelocity.magnitude;
        public Collider BladeCollider => bladeCollider;

        public event Action<AttackState> OnAttackStateChanged;

        private void Awake()
        {
            if (swordPivot == null)
                swordPivot = transform;

            if (bladeCollider == null)
                bladeCollider = GetComponentInChildren<Collider>();

            if (bladeTrail == null)
                bladeTrail = GetComponentInChildren<TrailRenderer>();

            if (bladeCollider != null)
                bladeCollider.isTrigger = true;

            SetTrailActive(false);
        }

        private void Start()
        {
            Vector3 hilt = swordPivot.position;
            Vector3 tip = (bladeTip != null) ? bladeTip.position : hilt + swordPivot.up * 1.38f;

            prevHiltPos = hilt;
            prevTipPos = tip;
            prevMidPos = (hilt + tip) * 0.5f;

            ApplyGuardPose(currentChamberAngle);
        }

        /// <summary>
        /// Updates the 2D clock face chamber angle from stick or mouse direction.
        /// </summary>
        public void SetInputVector(Vector2 inputVector)
        {
            if (inputVector.sqrMagnitude >= 0.04f)
            {
                targetChamberAngle = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;
            }
        }

        /// <summary>
        /// Triggers a committed martial longsword cut driving through the aimed diameter.
        /// </summary>
        public bool TriggerSlash()
        {
            if (state != AttackState.Ready) return false;

            currentAttackType = AttackType.Slash;
            CalculateMartialCutTrajectory(currentChamberAngle);

            ChangeState(AttackState.Windup);
            return true;
        }

        /// <summary>
        /// Triggers an explicit, committed forward thrust attack into depth.
        /// </summary>
        public bool TriggerThrust()
        {
            if (state != AttackState.Ready) return false;

            currentAttackType = AttackType.Thrust;
            ChangeState(AttackState.Windup);
            return true;
        }

        private void Update()
        {
            UpdateAiming();
            UpdateStateMachine();
            UpdateContinuousSweep();
        }

        private void UpdateAiming()
        {
            if (state == AttackState.Ready)
            {
                currentChamberAngle = Mathf.SmoothDampAngle(
                    currentChamberAngle,
                    targetChamberAngle,
                    ref aimAngularVelocity,
                    aimSmoothTime
                );
            }
        }

        private void UpdateStateMachine()
        {
            stateTimer += Time.deltaTime;

            switch (state)
            {
                case AttackState.Ready:
                    ApplyGuardPose(currentChamberAngle);
                    break;

                case AttackState.Windup:
                    HandleWindup();
                    break;

                case AttackState.ActiveSlash:
                    HandleActiveSlash();
                    break;

                case AttackState.ActiveThrust:
                    HandleActiveThrust();
                    break;

                case AttackState.Recoiling:
                    HandleRecoil();
                    break;

                case AttackState.Recovery:
                    HandleRecovery();
                    break;
            }
        }

        /// <summary>
        /// Computes anatomical start, windup, and follow-through poses based on chamber quadrant.
        /// </summary>
        private void CalculateMartialCutTrajectory(float chamberAngle)
        {
            float startRad = chamberAngle * Mathf.Deg2Rad;
            Vector2 startDir = new Vector2(Mathf.Cos(startRad), Mathf.Sin(startRad));

            // Target opposite quadrant across the clock-face diameter
            float endAngle = chamberAngle + 180f;
            float endRad = endAngle * Mathf.Deg2Rad;
            Vector2 endDir = new Vector2(Mathf.Cos(endRad), Mathf.Sin(endRad));

            // Hand Start: at the chamber side
            strikeStartHandPos = GetGuardHandPosition(chamberAngle);

            // Hand End: across the body on the opposite side, slightly lower and extended
            strikeEndHandPos = baseHandOffset + new Vector3(endDir.x * (chamberRadius * 1.15f), endDir.y * (chamberRadius * 0.9f), 0.10f);

            // 1. Guard Rotation
            strikeStartRot = CalculateBladeRotation(chamberAngle, 32f);

            // 2. Windup Telegraph Pose:
            // Cocks blade back slightly along chamber side
            float windupAngle = chamberAngle + (Mathf.Sign(startDir.x) * 15f);
            windupCockRot = CalculateBladeRotation(windupAngle, 18f, startDir.x > 0 ? -15f : 15f);

            // 3. End Pose:
            // Finished across opposite side with blade extending outward
            strikeEndRot = CalculateBladeRotation(endAngle, 38f, startDir.x > 0 ? 20f : -20f);
        }

        /// <summary>
        /// Calculates the 3D blade orientation:
        /// - angleDegrees: Clock-face chamber angle (0° = Right, 45° = High-Right, 90° = High, 135° = High-Left, 180° = Left).
        /// - forwardPitchDegrees: Tilt angle forward into depth (+Z toward opponent).
        /// - edgeRollAngle: Slight roll around blade axis for authentic edge alignment (hasuji).
        /// </summary>
        public static Quaternion CalculateBladeRotation(float angleDegrees, float forwardPitchDegrees, float edgeRollAngle = 0f)
        {
            // 1. Roll aligns the blade (which points along +Y at identity) with the clock-face angle:
            // Zrot = angle - 90° (0°/Right = -90°, 45°/High-Right = -45°, 90°/High = 0°, 135°/High-Left = +45°, 180°/Left = +90°)
            Quaternion roll = Quaternion.Euler(0f, 0f, angleDegrees - 90f);

            // 2. Pitch tilts the tip forward into depth (+Z toward opponent): Xrot = forwardPitch
            Quaternion pitch = Quaternion.Euler(forwardPitchDegrees, 0f, 0f);

            // 3. Optional edge twist along blade for hasuji
            Quaternion twist = (Mathf.Abs(edgeRollAngle) > 0.01f) ? Quaternion.Euler(0f, edgeRollAngle, 0f) : Quaternion.identity;

            return roll * pitch * twist;
        }

        private void HandleWindup()
        {
            float duration = (currentAttackType == AttackType.Slash) ? slashWindupDuration : thrustWindupDuration;
            float t = Mathf.Clamp01(stateTimer / duration);
            float ease = Mathf.SmoothStep(0f, 1f, t);

            if (currentAttackType == AttackType.Slash)
            {
                // Hands pull back into the chamber shoulder
                Vector3 windupHandPos = strikeStartHandPos + new Vector3(0f, 0.06f * ease, -0.16f * ease);
                swordPivot.localPosition = windupHandPos;

                // Blade cocks back into telegraph pose on the starting side
                swordPivot.localRotation = Quaternion.Slerp(strikeStartRot, windupCockRot, ease);
            }
            else
            {
                // Thrust Windup: Hands pull back tight against hip/chest
                swordPivot.localPosition = baseHandOffset + new Vector3(0f, -0.05f * ease, -0.22f * ease);
                swordPivot.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            if (stateTimer >= duration)
            {
                SetTrailActive(true);
                if (currentAttackType == AttackType.Slash)
                    ChangeState(AttackState.ActiveSlash);
                else
                    ChangeState(AttackState.ActiveThrust);
            }
        }

        private void HandleActiveSlash()
        {
            float t = Mathf.Clamp01(stateTimer / slashActiveDuration);
            // Explosive kinetic acceleration curve
            float curve = Mathf.SmoothStep(0f, 1f, t);
            float powerCurve = Mathf.Pow(curve, 1.25f);

            // 1. Hands Drive Forward and Across the Body from Start to End
            Vector3 handPos = Vector3.Lerp(strikeStartHandPos, strikeEndHandPos, powerCurve);
            // Forward reach apex at middle of cut (+Z into opponent)
            float forwardDrive = Mathf.Sin(t * Mathf.PI) * forwardCutDrive;
            handPos.z += forwardDrive;
            swordPivot.localPosition = handPos;

            // 2. Blade Slices across the diameter from Windup to End pose
            swordPivot.localRotation = Quaternion.Slerp(windupCockRot, strikeEndRot, powerCurve);

            if (stateTimer >= slashActiveDuration)
            {
                SetTrailActive(false);
                ChangeState(AttackState.Recovery);
            }
        }

        private void HandleActiveThrust()
        {
            float t = Mathf.Clamp01(stateTimer / thrustActiveDuration);
            float curve = Mathf.Sin(t * Mathf.PI * 0.5f);

            Vector3 thrustPos = baseHandOffset + new Vector3(0f, 0f, curve * thrustReachDistance);
            swordPivot.localPosition = thrustPos;
            // Points straight forward along +Z toward opponent
            swordPivot.localRotation = Quaternion.Euler(90f, 0f, 0f);

            if (stateTimer >= thrustActiveDuration)
            {
                SetTrailActive(false);
                ChangeState(AttackState.Recovery);
            }
        }

        private void HandleRecoil()
        {
            float t = Mathf.Clamp01(stateTimer / recoilDuration);
            float ease = Mathf.SmoothStep(0f, 1f, t);

            // Deflect hands backward and recover
            swordPivot.localPosition = Vector3.Lerp(recoilHandPos, baseHandOffset, ease);
            swordPivot.localRotation = Quaternion.Slerp(recoilRot, strikeStartRot, ease);

            if (stateTimer >= recoilDuration)
            {
                ChangeState(AttackState.Recovery);
            }
        }

        private void HandleRecovery()
        {
            float duration = (currentAttackType == AttackType.Slash) ? slashRecoveryDuration : thrustRecoveryDuration;
            float t = Mathf.Clamp01(stateTimer / duration);
            float ease = Mathf.SmoothStep(0f, 1f, t);

            Vector3 startPos = (currentAttackType == AttackType.Slash) ? strikeEndHandPos : baseHandOffset + new Vector3(0f, 0f, thrustReachDistance * 0.5f);
            Quaternion startRot = (currentAttackType == AttackType.Slash) ? strikeEndRot : Quaternion.Euler(90f, 0f, 0f);

            Vector3 guardPos = GetGuardHandPosition(currentChamberAngle);
            Quaternion guardRot = CalculateBladeRotation(currentChamberAngle, 32f);

            swordPivot.localPosition = Vector3.Lerp(startPos, guardPos, ease);
            swordPivot.localRotation = Quaternion.Slerp(startRot, guardRot, ease);

            if (stateTimer >= duration)
            {
                currentAttackType = AttackType.None;
                ChangeState(AttackState.Ready);
            }
        }

        private void ChangeState(AttackState newState)
        {
            state = newState;
            stateTimer = 0f;
            OnAttackStateChanged?.Invoke(state);
        }

        private void ApplyGuardPose(float angleDegrees)
        {
            swordPivot.localPosition = GetGuardHandPosition(angleDegrees);
            swordPivot.localRotation = CalculateBladeRotation(angleDegrees, 32f);
        }

        private Vector3 GetGuardHandPosition(float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            return baseHandOffset + new Vector3(Mathf.Cos(rad) * chamberRadius, Mathf.Sin(rad) * (chamberRadius * 0.85f), 0f);
        }

        /// <summary>
        /// Halts current strike upon clash and triggers kinetic recoil rebound.
        /// </summary>
        public void ApplyRecoil(float impulseDegrees)
        {
            SetTrailActive(false);

            recoilHandPos = swordPivot.localPosition - new Vector3(0f, 0f, 0.18f);
            Quaternion bounce = Quaternion.Euler(-25f, 0f, impulseDegrees);
            recoilRot = swordPivot.localRotation * bounce;

            ChangeState(AttackState.Recoiling);
        }

        /// <summary>
        /// Continuous multi-point raycast sweep along blade line (hilt, mid, tip) to eliminate physics tunneling.
        /// </summary>
        private void UpdateContinuousSweep()
        {
            Vector3 currentHilt = swordPivot.position;
            Vector3 currentTip = (bladeTip != null) ? bladeTip.position : currentHilt + swordPivot.up * 1.38f;
            Vector3 currentMid = (currentHilt + currentTip) * 0.5f;

            if (Time.deltaTime > 0f)
            {
                strikeVelocity = (currentTip - prevTipPos) / Time.deltaTime;
                if (strikeVelocity.sqrMagnitude > 0.05f)
                {
                    strikeDirection = strikeVelocity.normalized;
                }
            }

            if (IsAttacking)
            {
                CastSegment(prevHiltPos, currentHilt);
                CastSegment(prevMidPos, currentMid);
                CastSegment(prevTipPos, currentTip);
            }

            prevHiltPos = currentHilt;
            prevMidPos = currentMid;
            prevTipPos = currentTip;
        }

        private void CastSegment(Vector3 from, Vector3 to)
        {
            Vector3 delta = to - from;
            float dist = delta.magnitude;
            if (dist < 0.005f) return;

            RaycastHit[] hits = Physics.RaycastAll(from, delta.normalized, dist);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i].collider;
                if (col.transform.root == transform.root) continue; // Ignore self

                ShieldStanceManager shield = col.GetComponentInParent<ShieldStanceManager>();
                if (shield != null)
                {
                    ClashDetector detector = GetComponentInChildren<ClashDetector>();
                    if (detector != null)
                    {
                        detector.ResolveClash(this, shield);
                        break;
                    }
                }
            }
        }

        private void SetTrailActive(bool active)
        {
            if (bladeTrail != null)
            {
                bladeTrail.emitting = active;
                if (!active) bladeTrail.Clear();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (swordPivot == null) return;

            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position + baseHandOffset, chamberRadius);

            if (IsAttacking && strikeVelocity.sqrMagnitude > 0.1f)
            {
                Gizmos.color = Color.red;
                Vector3 tipPos = (bladeTip != null) ? bladeTip.position : swordPivot.position;
                Gizmos.DrawRay(tipPos, strikeDirection * 0.7f);
            }
        }
    }
}
