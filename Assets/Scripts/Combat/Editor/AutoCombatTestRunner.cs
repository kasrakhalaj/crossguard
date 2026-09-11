#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crossguard.Combat;
using System.Reflection;

namespace Crossguard.Combat.Editor
{
    public static class AutoCombatTestRunner
    {
        [MenuItem("Crossguard/Run Diagnostic Tests")]
        public static void RunDiagnostics()
        {
            Debug.Log("<color=magenta>====================================================</color>");
            Debug.Log("<color=magenta>🔍 [AUTONOMOUS COMBAT DIAGNOSTIC TEST START] 🔍</color>");
            Debug.Log("<color=magenta>====================================================</color>");

            // 1. Rebuild arena in active scene
            CrossguardEditorSetup.SetupArenaMenu();

            GameObject player = GameObject.Find("Player_Duelist");
            if (player == null)
            {
                Debug.LogError("[TEST ERROR] Player_Duelist not found after setup!");
                return;
            }

            SwordPlaneController sword = player.GetComponent<SwordPlaneController>();
            if (sword == null)
            {
                Debug.LogError("[TEST ERROR] SwordPlaneController missing on player!");
                return;
            }

            Transform swordPivot = (Transform)typeof(SwordPlaneController)
                .GetField("swordPivot", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sword);

            Transform bladeTip = (Transform)typeof(SwordPlaneController)
                .GetField("bladeTip", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sword);

            MethodInfo startMethod = typeof(SwordPlaneController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo updateMethod = typeof(SwordPlaneController).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo changeStateMethod = typeof(SwordPlaneController).GetMethod("ChangeState", BindingFlags.NonPublic | BindingFlags.Instance);

            startMethod.Invoke(sword, null);

            // --- TEST A: Aiming across all 4 Quadrants ---
            Vector2[] testDirections = new Vector2[]
            {
                new Vector2(1f, 1f).normalized,    // High-Right (45°)
                new Vector2(-1f, 1f).normalized,   // High-Left (135°)
                new Vector2(-1f, -1f).normalized,  // Low-Left (-135°)
                new Vector2(1f, -1f).normalized    // Low-Right (-45°)
            };

            string[] quadrantNames = new string[] { "HIGH-RIGHT", "HIGH-LEFT", "LOW-LEFT", "LOW-RIGHT" };

            for (int q = 0; q < testDirections.Length; q++)
            {
                sword.SetInputVector(testDirections[q]);
                // Simulate 25 frames
                for (int f = 0; f < 25; f++)
                {
                    updateMethod.Invoke(sword, null);
                }

                Vector3 tipLocal = player.transform.InverseTransformPoint(bladeTip.position);
                Vector3 hiltLocal = swordPivot.localPosition;

                Debug.Log($"<color=cyan>[GUARD {quadrantNames[q]}]</color> Aim Angle: {sword.ChamberAngle:F0}° | Hilt X: {hiltLocal.x:F3}, Y: {hiltLocal.y:F3} | Tip X: {tipLocal.x:F3}, Y: {tipLocal.y:F3}, Z: {tipLocal.z:F3}");

                // Verification assertions
                if (q == 0) // High-Right
                {
                    if (tipLocal.x > 0.05f && tipLocal.y > 0.1f)
                        Debug.Log("<color=green>  --> PASS: High-Right tip is correctly in UPPER-RIGHT quadrant.</color>");
                    else
                        Debug.LogError($"  --> FAIL: High-Right tip is NOT in upper-right! (Tip Local: {tipLocal})");
                }
                else if (q == 1) // High-Left
                {
                    if (tipLocal.x < -0.05f && tipLocal.y > 0.1f)
                        Debug.Log("<color=green>  --> PASS: High-Left tip is correctly in UPPER-LEFT quadrant.</color>");
                    else
                        Debug.LogError($"  --> FAIL: High-Left tip is NOT in upper-left! (Tip Local: {tipLocal})");
                }
                else if (q == 2) // Low-Left
                {
                    if (tipLocal.x < -0.05f && tipLocal.y < 0.2f)
                        Debug.Log("<color=green>  --> PASS: Low-Left tip is correctly in LOWER-LEFT quadrant.</color>");
                    else
                        Debug.LogError($"  --> FAIL: Low-Left tip is NOT in lower-left! (Tip Local: {tipLocal})");
                }
                else if (q == 3) // Low-Right
                {
                    if (tipLocal.x > 0.05f && tipLocal.y < 0.2f)
                        Debug.Log("<color=green>  --> PASS: Low-Right tip is correctly in LOWER-RIGHT quadrant.</color>");
                    else
                        Debug.LogError($"  --> FAIL: Low-Right tip is NOT in lower-right! (Tip Local: {tipLocal})");
                }
            }

            // --- TEST B: Slashes from High-Right and High-Left ---
            Debug.Log("\n<color=yellow>--- TESTING SLASH FROM HIGH-RIGHT ---</color>");
            sword.SetInputVector(new Vector2(1f, 1f).normalized);
            for (int f = 0; f < 25; f++) updateMethod.Invoke(sword, null);

            sword.TriggerSlash();
            // Step through windup
            float timer = 0f;
            while (sword.State == AttackState.Windup && timer < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                timer += Time.deltaTime;
            }

            Vector3 hrSlashStartTip = player.transform.InverseTransformPoint(bladeTip.position);

            // Step through active slash
            timer = 0f;
            Vector3 hrSlashMidTip = Vector3.zero;
            bool gotMid = false;

            while (sword.State == AttackState.ActiveSlash && timer < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                timer += Time.deltaTime;
                if (timer >= 0.11f && !gotMid)
                {
                    hrSlashMidTip = player.transform.InverseTransformPoint(bladeTip.position);
                    gotMid = true;
                }
            }

            Vector3 hrSlashEndTip = player.transform.InverseTransformPoint(bladeTip.position);

            Debug.Log($"High-Right Slash Trajectory: Start X={hrSlashStartTip.x:F3} -> Mid X={hrSlashMidTip.x:F3}, Z={hrSlashMidTip.z:F3} -> End X={hrSlashEndTip.x:F3}");

            if (hrSlashStartTip.x > 0.1f && hrSlashEndTip.x < -0.1f)
            {
                Debug.Log("<color=green>  --> PASS: High-Right slash sliced from RIGHT to LEFT across player centerline!</color>");
            }
            else
            {
                Debug.LogError($"  --> FAIL: High-Right slash did not cross centerline properly! Start X={hrSlashStartTip.x:F3}, End X={hrSlashEndTip.x:F3}");
            }

            // Recover to ready
            timer = 0f;
            while (sword.State != AttackState.Ready && timer < 2.0f)
            {
                updateMethod.Invoke(sword, null);
                timer += Time.deltaTime;
            }

            Debug.Log("\n<color=yellow>--- TESTING SLASH FROM HIGH-LEFT ---</color>");
            sword.SetInputVector(new Vector2(-1f, 1f).normalized);
            for (int f = 0; f < 30; f++) updateMethod.Invoke(sword, null);

            sword.TriggerSlash();
            timer = 0f;
            while (sword.State == AttackState.Windup && timer < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                timer += Time.deltaTime;
            }

            Vector3 hlSlashStartTip = player.transform.InverseTransformPoint(bladeTip.position);

            timer = 0f;
            while (sword.State == AttackState.ActiveSlash && timer < 1.0f)
            {
                updateMethod.Invoke(sword, null);
                timer += Time.deltaTime;
            }

            Vector3 hlSlashEndTip = player.transform.InverseTransformPoint(bladeTip.position);

            Debug.Log($"High-Left Slash Trajectory: Start X={hlSlashStartTip.x:F3} -> End X={hlSlashEndTip.x:F3}");

            if (hlSlashStartTip.x < -0.1f && hlSlashEndTip.x > 0.1f)
            {
                Debug.Log("<color=green>  --> PASS: High-Left slash sliced from LEFT to RIGHT across player centerline!</color>");
            }
            else
            {
                Debug.LogError($"  --> FAIL: High-Left slash did not cross centerline properly! Start X={hlSlashStartTip.x:F3}, End X={hlSlashEndTip.x:F3}");
            }

            Debug.Log("<color=magenta>====================================================</color>");
            Debug.Log("<color=magenta>🏁 [DIAGNOSTIC TEST COMPLETE] 🏁</color>");
            Debug.Log("<color=magenta>====================================================</color>");
        }
    }
}
#endif
