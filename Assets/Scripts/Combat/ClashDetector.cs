using System;
using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Evaluates collisions between a sword and a shield, calculating the angle delta and match ratio.
    /// Dispatches graduated hitstop, audio pitch, shield durability damage, and kinetic blade recoil.
    /// Prevents self-collision between a combatant's own weapons and body.
    /// </summary>
    public class ClashDetector : MonoBehaviour
    {
        [Header("Tuning Parameters")]
        [Tooltip("Maximum angle difference (in degrees) to register any block. Angles beyond this result in a clean hit.")]
        [Range(30f, 90f)]
        [SerializeField] private float thetaMaxDegrees = 65f;

        [Tooltip("Threshold above which a block is classified as Perfect (typically 0.75 - 0.8).")]
        [Range(0.6f, 0.95f)]
        [SerializeField] private float perfectMatchThreshold = 0.75f;

        [Tooltip("Threshold below which a block is classified as a Clean Hit (typically 0.2 - 0.3).")]
        [Range(0.05f, 0.4f)]
        [SerializeField] private float cleanHitThreshold = 0.25f;

        [Header("Recoil Intensities (Degrees)")]
        [SerializeField] private float perfectBlockRecoil = 45f;
        [SerializeField] private float weakBlockRecoil = 25f;
        [SerializeField] private float cleanHitRecoil = 8f;

        [Header("Hitstop Frames (at 60 Hz)")]
        [SerializeField] private int perfectBlockHitstop = 7;
        [SerializeField] private int weakBlockHitstop = 4;
        [SerializeField] private int cleanHitHitstop = 2;

        [Header("Durability Damage")]
        [SerializeField] private float weakBlockDamage = 30f;
        [SerializeField] private float perfectBlockDamage = 3f;

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clashSound;
        [SerializeField] private AudioClip hitSound;

        [Header("Debounce / Cooldown")]
        [SerializeField] private float minTimeBetweenHits = 0.15f;
        private float lastClashTime = -1f;

        public event Action<ClashResult> OnClashOccurred;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollisionOrTrigger(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollisionOrTrigger(collision.collider);
        }

        private void HandleCollisionOrTrigger(Collider other)
        {
            if (Time.time - lastClashTime < minTimeBetweenHits) return;

            // Check if this object is a blade colliding with a shield
            SwordPlaneController sword = GetComponentInParent<SwordPlaneController>();
            ShieldStanceManager shield = other.GetComponentInParent<ShieldStanceManager>();

            // If this object is the shield colliding with a sword
            if (sword == null && shield == null)
            {
                shield = GetComponentInParent<ShieldStanceManager>();
                sword = other.GetComponentInParent<SwordPlaneController>();
            }

            // ONLY trigger clashes if the sword is actively swinging or thrusting!
            if (sword != null && !sword.IsAttacking)
            {
                return;
            }

            // SELF-COLLISION PROTECTION: Never allow a duelist to clash against their own shield or gear!
            if (sword != null && shield != null)
            {
                if (sword.transform.root == shield.transform.root)
                {
                    return;
                }

                ResolveClash(sword, shield);
                return;
            }

            // Self-body hit protection
            if (sword != null)
            {
                if (other.transform.root == sword.transform.root)
                {
                    return; // Ignore hitting own body or limbs
                }
            }
        }

        /// <summary>
        /// Executes the core angle-matching formula and applies graduated feedback.
        /// </summary>
        public ClashResult ResolveClash(SwordPlaneController sword, ShieldStanceManager shield)
        {
            lastClashTime = Time.time;

            Vector3 strikeDir = sword.StrikeDirection;
            Vector3 shieldVec = shield.GetShieldStanceVector();

            // Default fallback if blade speed was negligible
            if (strikeDir.sqrMagnitude < 0.001f)
            {
                strikeDir = sword.transform.forward;
            }

            // Dot product between strike vector and shield stance vector
            float dot = Mathf.Clamp(Vector3.Dot(strikeDir.normalized, shieldVec.normalized), -1f, 1f);
            
            // Absolute dot product: alignment matters, regardless of positive/negative direction along stance axis
            float angleDelta = Mathf.Acos(Mathf.Abs(dot)) * Mathf.Rad2Deg;

            // Match ratio M = Clamp01(1 - theta / thetaMax)
            float matchRatio = Mathf.Clamp01(1f - (angleDelta / thetaMaxDegrees));

            ClashResult result = new ClashResult
            {
                AngleDelta = angleDelta,
                MatchRatio = matchRatio
            };

            if (matchRatio >= perfectMatchThreshold)
            {
                // Perfect Block
                result.Type = HitType.PerfectBlock;
                result.RecoilImpulse = perfectBlockRecoil;
                result.DurabilityDamage = perfectBlockDamage;

                sword.ApplyRecoil(perfectBlockRecoil);
                shield.ApplyDurabilityDamage(perfectBlockDamage);
                shield.ApplyImpactShudder(strikeDir, 0.12f);
                HitstopManager.FreezeFrames(perfectBlockHitstop);
                PlayAudio(clashSound, 1.35f);
                CenteredDuelCamera.Instance?.TriggerShake(0.7f);
                DuelCameraRig.Instance?.TriggerShake(0.7f);

                Debug.Log($"<color=cyan>[CLASH] PERFECT BLOCK!</color> Match: {matchRatio:P0} | Delta: {angleDelta:F1}° | Shield Dmg: {perfectBlockDamage}");
            }
            else if (matchRatio >= cleanHitThreshold)
            {
                // Weak / Partial Block
                result.Type = HitType.WeakBlock;
                result.RecoilImpulse = weakBlockRecoil;
                result.DurabilityDamage = weakBlockDamage;

                sword.ApplyRecoil(weakBlockRecoil);
                shield.ApplyDurabilityDamage(weakBlockDamage);
                shield.ApplyImpactShudder(strikeDir, 0.07f);
                HitstopManager.FreezeFrames(weakBlockHitstop);
                PlayAudio(clashSound, 0.85f);
                CenteredDuelCamera.Instance?.TriggerShake(0.35f);
                DuelCameraRig.Instance?.TriggerShake(0.35f);

                Debug.Log($"<color=yellow>[CLASH] WEAK BLOCK!</color> Match: {matchRatio:P0} | Delta: {angleDelta:F1}° | Shield Dmg: {weakBlockDamage}");
            }
            else
            {
                // Clean Hit / Bypass
                result.Type = HitType.CleanHit;
                result.RecoilImpulse = cleanHitRecoil;
                result.DurabilityDamage = 0f;

                sword.ApplyRecoil(cleanHitRecoil);
                HitstopManager.FreezeFrames(cleanHitHitstop);
                PlayAudio(hitSound, 1.0f);
                CenteredDuelCamera.Instance?.TriggerShake(0.15f);
                DuelCameraRig.Instance?.TriggerShake(0.15f);

                Debug.Log($"<color=red>[CLASH] CLEAN HIT / BYPASS!</color> Match: {matchRatio:P0} | Delta: {angleDelta:F1}° | Full Damage Bypasses Shield");
            }

            OnClashOccurred?.Invoke(result);
            return result;
        }

        private void PlayAudio(AudioClip clip, float pitch)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
