using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Lightweight immediate-mode debug GUI displaying stance state, sword speed, and last clash metrics.
    /// Requires no Canvas or UI setup—draws directly onto the screen for rapid greybox iteration.
    /// </summary>
    public class CombatDebugUI : MonoBehaviour
    {
        [SerializeField] private DuelistController playerDuelist;
        [SerializeField] private ShieldStanceManager shieldManager;
        [SerializeField] private SwordPlaneController swordController;
        [SerializeField] private ClashDetector clashDetector;

        private ClashResult lastClash;
        private bool hasClashResult = false;
        private float lastClashTimestamp = 0f;

        private void Awake()
        {
            if (playerDuelist == null)
                playerDuelist = FindAnyObjectByType<DuelistController>();

            if (shieldManager == null)
                shieldManager = FindAnyObjectByType<ShieldStanceManager>();

            if (swordController == null)
                swordController = FindAnyObjectByType<SwordPlaneController>();

            if (clashDetector == null)
                clashDetector = FindAnyObjectByType<ClashDetector>();
        }

        private void OnEnable()
        {
            if (clashDetector != null)
                clashDetector.OnClashOccurred += HandleClash;
        }

        private void OnDisable()
        {
            if (clashDetector != null)
                clashDetector.OnClashOccurred -= HandleClash;
        }

        private void HandleClash(ClashResult result)
        {
            lastClash = result;
            hasClashResult = true;
            lastClashTimestamp = Time.time;
        }

        private void OnGUI()
        {
            GUIStyle headerStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13
            };

            GUILayout.BeginArea(new Rect(20, 20, 320, 260), GUI.skin.window);
            GUILayout.Label("⚔️ CROSSGUARD COMBAT MONITOR", headerStyle);

            // Stance Display
            if (shieldManager != null)
            {
                GUILayout.Space(5);
                GUILayout.Label($"<b>Shield Stance:</b> <color=cyan>{shieldManager.CurrentStance}</color>", labelStyle);
                GUILayout.Label($"<b>Shield Durability:</b> {shieldManager.Durability:F0} / {shieldManager.MaxDurability:F0}", labelStyle);
            }

            // Lock-On Display
            if (CenteredDuelCamera.Instance != null)
            {
                GUILayout.Space(5);
                string lockStatus = CenteredDuelCamera.Instance.IsLockedOn ? "<color=#00ff88>LOCKED</color>" : "<color=#ffaa00>FREE</color>";
                GUILayout.Label($"<b>Lock-On:</b> {lockStatus} <color=grey>[Tab / M3 / R3]</color>", labelStyle);
            }

            // Sword Tracking
            if (swordController != null)
            {
                GUILayout.Space(5);
                string stateColor = swordController.State switch
                {
                    AttackState.ActiveSlash => "#ff3333",
                    AttackState.ActiveThrust => "#ff8800",
                    AttackState.Recoiling => "#ffff00",
                    AttackState.Windup => "#ffcc88",
                    _ => "#88ff88"
                };

                GUILayout.Label($"<b>Sword State:</b> <color={stateColor}>{swordController.State}</color>", labelStyle);
                GUILayout.Label($"<b>Chamber Angle:</b> {swordController.ChamberAngle:F0}°", labelStyle);
                GUILayout.Label($"<b>Strike Speed:</b> {swordController.StrikeSpeed:F2} m/s", labelStyle);
            }

            // Clash Result
            GUILayout.Space(5);
            if (hasClashResult && (Time.time - lastClashTimestamp < 4.0f))
            {
                string colorHex = lastClash.Type switch
                {
                    HitType.PerfectBlock => "#00ffff",
                    HitType.WeakBlock => "#ffcc00",
                    _ => "#ff4444"
                };

                GUILayout.Label($"<b>Last Clash:</b> <color={colorHex}>{lastClash.Type}</color>", labelStyle);
                GUILayout.Label($"• Match Ratio: <b>{lastClash.MatchRatio:P0}</b>", labelStyle);
                GUILayout.Label($"• Angle Delta: <b>{lastClash.AngleDelta:F1}°</b>", labelStyle);
                GUILayout.Label($"• Recoil Impulse: <b>{lastClash.RecoilImpulse:F0}°</b>", labelStyle);
            }
            else
            {
                GUILayout.Label("<color=grey>Awaiting strike clash...</color>", labelStyle);
            }

            GUILayout.EndArea();

            // Controls Hint Box
            GUILayout.BeginArea(new Rect(Screen.width - 360, 20, 340, 240), GUI.skin.box);
            GUILayout.Label("🎮 UNIFIED CONTROLS & ATTACKS", headerStyle);
            GUILayout.Label("• <b>R-Stick Aim / Mouse XY:</b> Sword Aim Plane", labelStyle);
            GUILayout.Label("• <b>R-Stick Sweep / LMB:</b> 💥 <b>SLASH</b>", labelStyle);
            GUILayout.Label("• <b>R-Stick Push / RMB:</b> 🗡️ <b>THRUST</b>", labelStyle);
            GUILayout.Label("• <b>R3 / Tab / Middle Click:</b> 🎯 <b>Lock-On Toggle</b>", labelStyle);
            GUILayout.Label("• <b>LB / Q:</b> High-Left Shield", labelStyle);
            GUILayout.Label("• <b>RB / E:</b> High-Right Shield", labelStyle);
            GUILayout.Label("• <b>LT / Z:</b> Low-Left Shield", labelStyle);
            GUILayout.Label("• <b>RT / C:</b> Low-Right Shield", labelStyle);
            GUILayout.Label("• <b>Left Stick / WASD:</b> Duel Footwork", labelStyle);
            GUILayout.EndArea();
        }
    }
}
