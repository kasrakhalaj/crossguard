using System;
using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Manages discrete angle-matched shield stances with tactile physical mass and spring-damper dynamics.
    /// Snaps into 4 spatial quadrants with kinetic settle and deflects backwards with an impact shudder on clash.
    /// </summary>
    public class ShieldStanceManager : MonoBehaviour
    {
        [Header("Shield Transform & Pivot")]
        [Tooltip("The transform representing the shield.")]
        [SerializeField] private Transform shieldTransform;

        [Tooltip("Collider component on the shield used for clash detection.")]
        [SerializeField] private Collider shieldCollider;

        [Header("Durability")]
        [SerializeField] private float maxDurability = 100f;
        [SerializeField] private float currentDurability = 100f;

        [Header("Physical Weight & Spring Tuning")]
        [Tooltip("Spring-damper transition time. Higher = heavier feeling shield.")]
        [Range(0.04f, 0.25f)]
        [SerializeField] private float stanceSmoothTime = 0.07f;

        [Tooltip("Elastic bounce recovery speed after blocking an impact.")]
        [SerializeField] private float impactRecoverySpeed = 14f;

        [Header("Stance Positional Offsets (Local)")]
        [SerializeField] private Vector3 neutralOffset = new Vector3(0f, 0f, 0.45f);
        [SerializeField] private Vector3 highLeftOffset = new Vector3(-0.35f, 0.35f, 0.5f);
        [SerializeField] private Vector3 highRightOffset = new Vector3(0.35f, 0.35f, 0.5f);
        [SerializeField] private Vector3 lowLeftOffset = new Vector3(-0.35f, -0.3f, 0.5f);
        [SerializeField] private Vector3 lowRightOffset = new Vector3(0.35f, -0.3f, 0.5f);

        private ShieldStance currentStance = ShieldStance.Neutral;
        private Vector3 targetLocalPosition;
        private Quaternion targetLocalRotation;

        private Vector3 positionVelocity;
        private Vector3 impactOffset;
        private float angleVelocityY;
        private float angleVelocityX;
        private float angleVelocityZ;

        public ShieldStance CurrentStance => currentStance;
        public float Durability => currentDurability;
        public float MaxDurability => maxDurability;
        public Collider ShieldCollider => shieldCollider;

        public event Action<ShieldStance> OnStanceChanged;
        public event Action<float> OnDurabilityChanged;
        public event Action OnShieldBroken;

        private void Awake()
        {
            if (shieldTransform == null)
                shieldTransform = transform;

            if (shieldCollider == null)
                shieldCollider = GetComponentInChildren<Collider>();

            currentDurability = maxDurability;
            SetStance(ShieldStance.Neutral);
        }

        private void Update()
        {
            // Recover from impact shudder offset
            if (impactOffset.sqrMagnitude > 0.0001f)
            {
                impactOffset = Vector3.MoveTowards(impactOffset, Vector3.zero, impactRecoverySpeed * Time.deltaTime);
            }
            else
            {
                impactOffset = Vector3.zero;
            }

            // Weighted spring-damper positional tracking
            Vector3 desiredPos = targetLocalPosition + impactOffset;
            shieldTransform.localPosition = Vector3.SmoothDamp(
                shieldTransform.localPosition,
                desiredPos,
                ref positionVelocity,
                stanceSmoothTime
            );

            // Weighted rotational tracking with smooth slerp
            shieldTransform.localRotation = Quaternion.Slerp(
                shieldTransform.localRotation,
                targetLocalRotation,
                Time.deltaTime * (1f / Mathf.Max(0.01f, stanceSmoothTime))
            );
        }

        /// <summary>
        /// Switch to a new discrete stance.
        /// </summary>
        public void SetStance(ShieldStance newStance)
        {
            if (currentStance == newStance) return;

            currentStance = newStance;
            UpdateStanceTransforms();
            OnStanceChanged?.Invoke(currentStance);
        }

        private void UpdateStanceTransforms()
        {
            switch (currentStance)
            {
                case ShieldStance.HighLeft:
                    targetLocalPosition = highLeftOffset;
                    targetLocalRotation = Quaternion.Euler(15f, -25f, 45f);
                    break;
                case ShieldStance.HighRight:
                    targetLocalPosition = highRightOffset;
                    targetLocalRotation = Quaternion.Euler(15f, 25f, -45f);
                    break;
                case ShieldStance.LowLeft:
                    targetLocalPosition = lowLeftOffset;
                    targetLocalRotation = Quaternion.Euler(-15f, -25f, 135f);
                    break;
                case ShieldStance.LowRight:
                    targetLocalPosition = lowRightOffset;
                    targetLocalRotation = Quaternion.Euler(-15f, 25f, -135f);
                    break;
                case ShieldStance.Neutral:
                default:
                    targetLocalPosition = neutralOffset;
                    targetLocalRotation = Quaternion.identity;
                    break;
            }
        }

        /// <summary>
        /// Gets the normalized world-space stance vector representing the shield's braced orientation.
        /// </summary>
        public Vector3 GetShieldStanceVector()
        {
            switch (currentStance)
            {
                case ShieldStance.HighLeft:
                    return transform.TransformDirection(new Vector3(-1f, 1f, 0f)).normalized;
                case ShieldStance.HighRight:
                    return transform.TransformDirection(new Vector3(1f, 1f, 0f)).normalized;
                case ShieldStance.LowLeft:
                    return transform.TransformDirection(new Vector3(-1f, -1f, 0f)).normalized;
                case ShieldStance.LowRight:
                    return transform.TransformDirection(new Vector3(1f, -1f, 0f)).normalized;
                case ShieldStance.Neutral:
                default:
                    return transform.up;
            }
        }

        /// <summary>
        /// Applies an impact shudder pushing the shield backwards along the strike force, recovering via spring.
        /// </summary>
        public void ApplyImpactShudder(Vector3 strikeDirection, float impulseMagnitude)
        {
            Vector3 localStrike = transform.InverseTransformDirection(strikeDirection.normalized);
            // Push backward along depth plus slight strike deflection
            impactOffset = (-Vector3.forward * impulseMagnitude) + (localStrike * (impulseMagnitude * 0.4f));
            positionVelocity += impactOffset * 15f;
        }

        /// <summary>
        /// Deducts durability from the shield.
        /// </summary>
        public void ApplyDurabilityDamage(float amount)
        {
            currentDurability = Mathf.Max(0f, currentDurability - amount);
            OnDurabilityChanged?.Invoke(currentDurability);

            if (currentDurability <= 0f)
            {
                OnShieldBroken?.Invoke();
            }
        }

        public void RestoreDurability(float amount)
        {
            currentDurability = Mathf.Min(maxDurability, currentDurability + amount);
            OnDurabilityChanged?.Invoke(currentDurability);
        }

        private void OnDrawGizmosSelected()
        {
            if (shieldTransform == null) return;

            Gizmos.color = Color.green;
            Vector3 stanceVec = GetShieldStanceVector();
            Gizmos.DrawRay(shieldTransform.position, stanceVec * 0.6f);
        }
    }
}
