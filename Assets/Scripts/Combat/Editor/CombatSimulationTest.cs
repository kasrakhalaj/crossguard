#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crossguard.Combat;
using System.Reflection;

namespace Crossguard.Combat.Editor
{
    public static class CombatSimulationTest
    {
        public static void RunTest()
        {
            Debug.Log("=================================================");
            Debug.Log(">>> [COMBAT SIMULATION TEST START] <<<");
            Debug.Log("=================================================");

            // 1. Setup Arena
            CrossguardEditorSetup.SetupArenaMenu();

            GameObject player = GameObject.Find("Player_Duelist");
            if (player == null)
            {
                Debug.LogError("[TEST FAILED] Player_Duelist not found!");
                EditorApplication.Exit(1);
                return;
            }

            SwordPlaneController sword = player.GetComponent<SwordPlaneController>();
            if (sword == null)
            {
                Debug.LogError("[TEST FAILED] SwordPlaneController not found on player!");
                EditorApplication.Exit(1);
                return;
            }

            Transform swordPivot = (Transform)typeof(SwordPlaneController)
                .GetField("swordPivot", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sword);

            Transform bladeTip = (Transform)typeof(SwordPlaneController)
                .GetField("bladeTip", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sword);

            // Reflection helper to invoke Update
            MethodInfo updateMethod = typeof(SwordPlaneController).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo startMethod = typeof(SwordPlaneController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);

            startMethod.Invoke(sword, null);

            // ==========================================
            // TEST 1: HIGH-RIGHT CHAMBER & SLASH (45°)
            // ==========================================
            Debug.Log("\n--- TEST 1: High-Right Chamber (45°) ---");
            sword.SetInputVector(new Vector2(1f, 1f).normalized);

            // Simulate frames to settle aim
            for (int i = 0; i < 20; i++)
            {
                updateMethod.Invoke(sword, null);
            }

            Debug.Log($"Chamber Angle: {sword.ChamberAngle:F1}°");
            Debug.Log($"Hilt Local Pos: {swordPivot.localPosition}");
            Debug.Log($"Hilt World Pos: {swordPivot.position}");
            Debug.Log($"Blade Tip World Pos: {bladeTip.position}");

            bool tipIsRight = bladeTip.position.x > player.transform.position.x;
            Debug.Log($"Blade Tip is on RIGHT of Player: {tipIsRight} (Tip X: {bladeTip.position.x:F3}, Player X: {player.transform.position.x:F3})");

            if (!tipIsRight)
            {
                Debug.LogError("[FAIL] High-Right chamber blade tip is NOT on the right side of player!");
            }
            else
            {
                Debug.Log("[PASS] High-Right chamber blade tip is correctly on the right side!");
            }

            // Trigger Slash from High-Right
            Debug.Log("\nTriggering Slash from High-Right...");
            sword.TriggerSlash();

            // Step through Windup (0.26s)
            float elapsed = 0f;
            while (sword.State == AttackState.Windup && elapsed < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
            }

            Debug.Log($"Entered state: {sword.State}");
            Vector3 slashStartTip = bladeTip.position;
            Debug.Log($"Slash Active Start Tip Pos: {slashStartTip}");

            // Step through Active Slash (0.22s)
            elapsed = 0f;
            Vector3 midTip = Vector3.zero;
            bool capturedMid = false;

            while (sword.State == AttackState.ActiveSlash && elapsed < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;

                if (elapsed >= 0.10f && !capturedMid)
                {
                    midTip = bladeTip.position;
                    capturedMid = true;
                }
            }

            Vector3 slashEndTip = bladeTip.position;
            Debug.Log($"Slash Active Mid Tip Pos: {midTip}");
            Debug.Log($"Slash Active End Tip Pos: {slashEndTip}");

            bool slashedRightToLeft = (slashStartTip.x > player.transform.position.x) && (slashEndTip.x < player.transform.position.x);
            Debug.Log($"Slash traveled from RIGHT to LEFT across player: {slashedRightToLeft}");
            Debug.Log($"Start Tip X: {slashStartTip.x:F3} -> End Tip X: {slashEndTip.x:F3}");

            if (!slashedRightToLeft)
            {
                Debug.LogError("[FAIL] Slash did NOT travel from right to left!");
            }
            else
            {
                Debug.Log("[PASS] Slash successfully cut from High-Right to Low-Left across centerline!");
            }

            // Step through Recovery to Ready
            elapsed = 0f;
            while (sword.State != AttackState.Ready && elapsed < 2.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
            }

            // ==========================================
            // TEST 2: HIGH-LEFT CHAMBER & SLASH (135°)
            // ==========================================
            Debug.Log("\n--- TEST 2: High-Left Chamber (135°) ---");
            sword.SetInputVector(new Vector2(-1f, 1f).normalized);

            for (int i = 0; i < 30; i++)
            {
                updateMethod.Invoke(sword, null);
            }

            Debug.Log($"Chamber Angle: {sword.ChamberAngle:F1}°");
            Debug.Log($"Hilt Local Pos: {swordPivot.localPosition}");
            Debug.Log($"Hilt World Pos: {swordPivot.position}");
            Debug.Log($"Blade Tip World Pos: {bladeTip.position}");

            bool tipIsLeft = bladeTip.position.x < player.transform.position.x;
            Debug.Log($"Blade Tip is on LEFT of Player: {tipIsLeft} (Tip X: {bladeTip.position.x:F3}, Player X: {player.transform.position.x:F3})");

            if (!tipIsLeft)
            {
                Debug.LogError("[FAIL] High-Left chamber blade tip is NOT on the left side of player!");
            }
            else
            {
                Debug.Log("[PASS] High-Left chamber blade tip is correctly on the left side!");
            }

            // Trigger Slash from High-Left
            Debug.Log("\nTriggering Slash from High-Left...");
            sword.TriggerSlash();

            elapsed = 0f;
            while (sword.State == AttackState.Windup && elapsed < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
            }

            slashStartTip = bladeTip.position;
            Debug.Log($"Slash Active Start Tip Pos: {slashStartTip}");

            elapsed = 0f;
            while (sword.State == AttackState.ActiveSlash && elapsed < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
            }

            slashEndTip = bladeTip.position;
            Debug.Log($"Slash Active End Tip Pos: {slashEndTip}");

            bool slashedLeftToRight = (slashStartTip.x < player.transform.position.x) && (slashEndTip.x > player.transform.position.x);
            Debug.Log($"Slash traveled from LEFT to RIGHT across player: {slashedLeftToRight}");
            Debug.Log($"Start Tip X: {slashStartTip.x:F3} -> End Tip X: {slashEndTip.x:F3}");

            if (!slashedLeftToRight)
            {
                Debug.LogError("[FAIL] Slash did NOT travel from left to right!");
            }
            else
            {
                Debug.Log("[PASS] Slash successfully cut from High-Left to Low-Right across centerline!");
            }

            // ==========================================
            // TEST 3: THRUST REACH & FORWARD DEPTH (+Z)
            // ==========================================
            Debug.Log("\n--- TEST 3: Forward Thrust Reach ---");
            elapsed = 0f;
            while (sword.State != AttackState.Ready && elapsed < 2.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
            }

            Vector3 guardTipBeforeThrust = bladeTip.position;
            sword.TriggerThrust();

            elapsed = 0f;
            while (sword.State == AttackState.Windup && elapsed < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
            }

            float maxZReach = bladeTip.position.z;
            elapsed = 0f;
            while (sword.State == AttackState.ActiveThrust && elapsed < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                elapsed += Time.deltaTime;
                if (bladeTip.position.z > maxZReach)
                    maxZReach = bladeTip.position.z;
            }

            float thrustDepthExtension = maxZReach - guardTipBeforeThrust.z;
            Debug.Log($"Guard Tip Z: {guardTipBeforeThrust.z:F3} -> Max Thrust Tip Z: {maxZReach:F3}");
            Debug.Log($"Thrust Depth Extension: +{thrustDepthExtension:F3}m");

            if (thrustDepthExtension < 0.3f)
            {
                Debug.LogError($"[FAIL] Thrust did not extend forward into depth (Extension was only {thrustDepthExtension:F3}m)!");
            }
            else
            {
                Debug.Log("[PASS] Thrust extended forward into depth toward opponent!");
            }

            Debug.Log("=================================================");
            Debug.Log(">>> [ALL COMBAT SIMULATION TESTS COMPLETED] <<<");
            Debug.Log("=================================================");

            EditorApplication.Exit(0);
        }
    }
}
#endif
