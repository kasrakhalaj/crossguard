using UnityEngine;

namespace Mosslight
{
    // A ScriptableObject is a saved collection of settings. Change the asset in the
    // Inspector to experiment WITHOUT editing the code or losing changes after Play.
    [CreateAssetMenu(menuName = "Mosslight/Game tuning")]
    public class MosslightTuning : ScriptableObject
    {
        [Header("Movement — change one number at a time")]
        public float runSpeed = 6.8f;
        public float jumpSpeed = 13.5f;
        public float gravity = 34f;
        public float dashSpeed = 19f;
        public float dashDuration = 0.17f;
        public float dashCooldown = 0.7f;
        [Tooltip("How long you can still jump after stepping off an edge.")]
        public float coyoteTime = 0.12f;
        [Tooltip("Remember a jump pressed just before landing.")]
        public float jumpBuffer = 0.12f;
        [Header("Combat")]
        public int playerHealth = 5;
        public float attackCooldown = 0.29f;
        public float attackReach = 1.5f;
        public int bossHealth = 24;
        public float damageInvulnerability = 1.1f;
        [Header("Comfort")]
        [Range(0, 1)] public float soundVolume = 0.35f;
        [Range(0, 1)] public float screenShake = 0.35f;
    }
}
