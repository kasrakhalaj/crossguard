using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Over-the-shoulder dynamic lock-on camera framing both duelists with right-shoulder offset,
    /// distance scaling, and trauma-based camera shake for heavy weapon clashes.
    /// Inspired by For Honor and Sekiro duel framing.
    /// </summary>
    public class DuelCameraRig : MonoBehaviour
    {
        [Header("Combatants")]
        [SerializeField] private Transform playerTarget;
        [SerializeField] private Transform opponentTarget;

        [Header("Framing Offsets")]
        [Tooltip("Distance behind the player.")]
        [SerializeField] private float distanceOffset = 3.8f;

        [Tooltip("Height above the player.")]
        [SerializeField] private float heightOffset = 1.8f;

        [Tooltip("Right-shoulder lateral offset so player's body doesn't block the opponent's weapon.")]
        [SerializeField] private float shoulderOffset = 0.85f;

        [Tooltip("Height bias for the look-at target.")]
        [SerializeField] private float lookHeightOffset = 1.35f;

        [Header("Damping & Responsiveness")]
        [SerializeField] private float positionSmoothTime = 0.08f;
        [SerializeField] private float rotationDamping = 12f;

        [Header("Field of View")]
        [SerializeField] private float baseFOV = 62f;

        // Runtime state
        private Vector3 positionVelocity;
        private Camera targetCamera;
        private float trauma = 0f;
        private float traumaDecayRate = 2.2f;

        public static DuelCameraRig Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            targetCamera = GetComponent<Camera>();
            if (targetCamera != null)
            {
                targetCamera.fieldOfView = baseFOV;
            }
        }

        public void SetTargets(Transform player, Transform opponent)
        {
            playerTarget = player;
            opponentTarget = opponent;
        }

        private void LateUpdate()
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

            if (playerTarget == null) return;

            UpdateCameraPositionAndRotation();
            UpdateCameraShake();
        }

        private void UpdateCameraPositionAndRotation()
        {
            Vector3 playerPos = playerTarget.position;
            Vector3 duelDir;
            float duelDist = 3.0f;

            if (opponentTarget != null)
            {
                duelDir = (opponentTarget.position - playerPos);
                duelDist = duelDir.magnitude;
                duelDir.Normalize();
            }
            else
            {
                duelDir = playerTarget.forward;
            }

            Vector3 duelRight = Vector3.Cross(Vector3.up, duelDir).normalized;

            // Compute desired camera position (behind player, offset right, elevated)
            float dynamicDist = distanceOffset + (duelDist * 0.12f);
            Vector3 desiredPosition = playerPos - (duelDir * dynamicDist) + (duelRight * shoulderOffset);
            desiredPosition.y = playerPos.y + heightOffset;

            // Apply position shake
            Vector3 shakeOffset = Vector3.zero;
            if (trauma > 0.01f)
            {
                float shakeMagnitude = trauma * trauma * 0.35f;
                float time = Time.time * 28f;
                shakeOffset = new Vector3(
                    (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f * shakeMagnitude,
                    (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f * shakeMagnitude,
                    0f
                );
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition + shakeOffset, ref positionVelocity, positionSmoothTime);

            // Look-at Target: weighted toward opponent's upper torso / weapon line
            Vector3 lookTarget;
            if (opponentTarget != null)
            {
                Vector3 midpoint = (playerPos + opponentTarget.position) * 0.5f;
                lookTarget = midpoint + (Vector3.up * lookHeightOffset) + (duelDir * 0.4f);
            }
            else
            {
                lookTarget = playerPos + (playerTarget.forward * 4f) + (Vector3.up * lookHeightOffset);
            }

            Quaternion targetRotation = Quaternion.LookRotation((lookTarget - transform.position).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);
        }

        private void UpdateCameraShake()
        {
            if (trauma > 0f)
            {
                trauma = Mathf.Max(0f, trauma - (Time.deltaTime * traumaDecayRate));
            }
        }

        /// <summary>
        /// Triggers trauma-based camera shake (0 to 1).
        /// </summary>
        public void TriggerShake(float amount)
        {
            trauma = Mathf.Clamp01(trauma + amount);
        }
    }
}
