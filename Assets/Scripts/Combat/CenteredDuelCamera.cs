using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Centered Third-Person Camera (Dark Souls / Sekiro style) featuring:
    /// - Zero lateral shoulder offset: perfectly symmetrical framing so left and right attacks are equally visible.
    /// - Close combat distance (2.2m) and downward pitch (12°) so weapons and opponent stances are prominent and readable.
    /// - Butter-smooth continuous yaw damping (SmoothDampAngle) eliminating all strafe stutter and micro-judder.
    /// - Manual Lock-On toggle support (Locked duel orbit vs Unlocked free camera).
    /// </summary>
    public class CenteredDuelCamera : MonoBehaviour
    {
        [Header("Combatants")]
        [SerializeField] private Transform playerTarget;
        [SerializeField] private Transform opponentTarget;

        [Header("Framing & Proximity")]
        [Tooltip("Distance behind the player. Closer = larger, more readable weapon silhouettes.")]
        [SerializeField] private float distance = 2.1f;

        [Tooltip("Height above the player.")]
        [SerializeField] private float height = 1.65f;

        [Tooltip("Downward pitch angle in degrees looking over the player's head into opponent.")]
        [SerializeField] private float downwardPitch = 12f;

        [Header("Smooth Damping Tuning (Zero Stutter)")]
        [Tooltip("Critically-damped yaw tracking time in Locked mode. Eliminates all strafe judder.")]
        [Range(0.04f, 0.25f)]
        [SerializeField] private float yawSmoothTime = 0.09f;

        [Tooltip("Yaw tracking smooth time in Unlocked mode following player facing/movement.")]
        [Range(0.08f, 0.40f)]
        [SerializeField] private float unlockedYawSmoothTime = 0.22f;

        [Tooltip("Positional follow smooth time behind the player.")]
        [SerializeField] private float positionSmoothTime = 0.06f;

        [Header("Lock-On State")]
        [SerializeField] private bool isLockedOn = true;

        [Header("Camera Shake")]
        [SerializeField] private float traumaDecayRate = 2.4f;

        // Runtime state
        private Vector3 positionVelocity;
        private float currentYaw;
        private float yawVelocity;
        private float trauma = 0f;
        private Camera cam;

        public static CenteredDuelCamera Instance { get; private set; }
        public bool IsLockedOn => isLockedOn;

        private void Awake()
        {
            Instance = this;
            cam = GetComponent<Camera>();
            if (cam != null)
            {
                cam.fieldOfView = 65f;
                cam.nearClipPlane = 0.05f;
            }
        }

        private void Start()
        {
            if (playerTarget != null)
            {
                currentYaw = playerTarget.eulerAngles.y;
            }
        }

        public void SetTargets(Transform player, Transform opponent)
        {
            playerTarget = player;
            opponentTarget = opponent;
        }

        public void ToggleLockOn()
        {
            isLockedOn = !isLockedOn;
            Debug.Log($"<color=cyan>[CAMERA] Lock-On: {(isLockedOn ? "ENGAGED" : "DISENGAGED")}</color>");
        }

        public void SetLockOn(bool locked)
        {
            isLockedOn = locked;
        }

        private void LateUpdate()
        {
            FindTargetsIfNull();
            if (playerTarget == null) return;

            UpdateCameraFraming();
            UpdateTraumaShake();
        }

        private void FindTargetsIfNull()
        {
            if (playerTarget == null)
            {
                DuelistController duelist = FindAnyObjectByType<DuelistController>();
                if (duelist != null) playerTarget = duelist.transform;
            }

            if (opponentTarget == null)
            {
                GameObject dummy = GameObject.Find("Target_Dummy");
                if (dummy != null) opponentTarget = dummy.transform;
            }
        }

        private void UpdateCameraFraming()
        {
            Vector3 playerPos = playerTarget.position;
            float targetYaw = currentYaw;
            float activeYawSmooth = yawSmoothTime;

            // Compute Target Yaw
            if (isLockedOn && opponentTarget != null)
            {
                // Locked mode: Autonomously soft-orients yaw toward locked opponent
                Vector3 toOpponent = opponentTarget.position - playerPos;
                toOpponent.y = 0f;

                if (toOpponent.sqrMagnitude > 0.01f)
                {
                    targetYaw = Quaternion.LookRotation(toOpponent).eulerAngles.y;
                }
                activeYawSmooth = yawSmoothTime;
            }
            else
            {
                // Unlocked mode: Autonomously follows player movement / facing direction with smooth damping - never snaps
                Vector3 playerFacing = playerTarget.forward;
                playerFacing.y = 0f;
                if (playerFacing.sqrMagnitude > 0.01f)
                {
                    targetYaw = Quaternion.LookRotation(playerFacing).eulerAngles.y;
                }
                activeYawSmooth = unlockedYawSmoothTime;
            }

            // Butter-smooth continuous yaw damping — ZERO deadzones, ZERO judder!
            currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVelocity, activeYawSmooth);

            // Compute Centered Position directly behind player along currentYaw
            Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);
            Vector3 backDirection = yawRotation * (-Vector3.forward);

            Vector3 targetCamPosition = playerPos + (backDirection * distance);
            targetCamPosition.y = playerPos.y + height;

            // Apply trauma shake
            Vector3 shakeOffset = Vector3.zero;
            if (trauma > 0.01f)
            {
                float shakeMagnitude = trauma * trauma * 0.28f;
                float time = Time.time * 30f;
                shakeOffset = new Vector3(
                    (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f * shakeMagnitude,
                    (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f * shakeMagnitude,
                    0f
                );
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetCamPosition + shakeOffset, ref positionVelocity, positionSmoothTime);

            // Symmetrical Centered Rotation with Downward Pitch
            Quaternion pitchRotation = Quaternion.Euler(downwardPitch, 0f, 0f);
            transform.rotation = yawRotation * pitchRotation;
        }

        private void UpdateTraumaShake()
        {
            if (trauma > 0f)
            {
                trauma = Mathf.Max(0f, trauma - (Time.deltaTime * traumaDecayRate));
            }
        }

        public void TriggerShake(float amount)
        {
            trauma = Mathf.Clamp01(trauma + amount);
        }
    }
}
