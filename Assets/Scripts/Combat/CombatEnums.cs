namespace Crossguard.Combat
{
    public enum ShieldStance
    {
        Neutral,
        HighLeft,
        HighRight,
        LowLeft,
        LowRight
    }

    public enum HitType
    {
        CleanHit,
        WeakBlock,
        PerfectBlock
    }

    public enum AttackState
    {
        Ready,
        Windup,
        ActiveSlash,
        ActiveThrust,
        Recoiling,
        Recovery
    }

    public enum AttackType
    {
        None,
        Slash,
        Thrust
    }

    public struct ClashResult
    {
        public HitType Type;
        public float AngleDelta;     // In degrees
        public float MatchRatio;     // 0.0 to 1.0
        public float RecoilImpulse;
        public float DurabilityDamage;
    }
}
