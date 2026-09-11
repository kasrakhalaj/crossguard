using UnityEngine;

namespace Crossguard.Combat
{
    /// <summary>
    /// Spawns a complete Greybox Duel Arena with:
    /// - High-contrast hero materials (Gleaming cyan blade, golden bronze shield, charcoal knight, crimson opponent).
    /// - Prominent hero weapon scales and postures for maximum readability from third-person.
    /// - CenteredDuelCamera with smooth damping (zero strafe stutter) and manual lock-on toggle.
    /// - TrailRenderer on blade for clear cutting-plane visual feedback.
    /// </summary>
    public class DuelArenaSpawner : MonoBehaviour
    {
        [Header("Auto-Spawn Settings")]
        [SerializeField] private bool autoSpawnOnStart = true;

        private void Start()
        {
            if (autoSpawnOnStart)
            {
                SetupArenaIfEmpty();
            }
        }

        [ContextMenu("Build Greybox Arena Now")]
        public void SetupArenaIfEmpty()
        {
            // Remove previous arena objects if re-building
            GameObject oldPlayer = GameObject.Find("Player_Duelist");
            if (oldPlayer != null) DestroyImmediate(oldPlayer);

            GameObject oldDummy = GameObject.Find("Target_Dummy");
            if (oldDummy != null) DestroyImmediate(oldDummy);

            GameObject oldFloor = GameObject.Find("Arena_Floor");
            if (oldFloor != null) DestroyImmediate(oldFloor);

            GameObject oldUI = GameObject.Find("Combat_Debug_UI");
            if (oldUI != null) DestroyImmediate(oldUI);

            Debug.Log("[Crossguard] Building High-Contrast 3D Duel Arena...");

            // 1. Arena Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Arena_Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(3f, 1f, 3f);
            floor.GetComponent<Renderer>().material = CreateMaterial(new Color(0.45f, 0.47f, 0.5f), "Mat_StoneFloor");

            // 2. Player Duelist (Charcoal Knight)
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Duelist";
            player.transform.position = new Vector3(0f, 1f, -2.5f);
            player.GetComponent<Renderer>().material = CreateMaterial(new Color(0.22f, 0.24f, 0.28f), "Mat_PlayerKnight");
            
            DuelistController duelist = player.AddComponent<DuelistController>();
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = Vector3.zero;

            Collider playerCol = player.GetComponent<CapsuleCollider>();
            if (playerCol != null) DestroyImmediate(playerCol);

            // 2a. Sword Hand & Hero Longsword
            GameObject swordPivot = new GameObject("Sword_Pivot");
            swordPivot.transform.SetParent(player.transform);
            swordPivot.transform.localPosition = new Vector3(0.10f, 0.10f, 0.48f);

            // Grip (Leather wrap)
            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            grip.name = "Grip";
            grip.transform.SetParent(swordPivot.transform);
            grip.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            grip.transform.localScale = new Vector3(0.045f, 0.12f, 0.045f);
            grip.GetComponent<Renderer>().material = CreateMaterial(new Color(0.32f, 0.2f, 0.1f), "Mat_LeatherGrip");
            Collider gripCol = grip.GetComponent<Collider>();
            if (gripCol != null) DestroyImmediate(gripCol);

            // Crossguard Bar (Gunmetal)
            GameObject crossguardBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crossguardBar.name = "Crossguard_Bar";
            crossguardBar.transform.SetParent(swordPivot.transform);
            crossguardBar.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            crossguardBar.transform.localScale = new Vector3(0.42f, 0.05f, 0.08f);
            crossguardBar.GetComponent<Renderer>().material = CreateMaterial(new Color(0.14f, 0.14f, 0.16f), "Mat_Gunmetal");
            Collider barCol = crossguardBar.GetComponent<Collider>();
            if (barCol != null) DestroyImmediate(barCol);

            // Blade Mesh (Hero scale, Gleaming Cyan Steel)
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade_Mesh";
            blade.transform.SetParent(swordPivot.transform);
            blade.transform.localPosition = new Vector3(0f, 0.72f, 0f);
            blade.transform.localScale = new Vector3(0.11f, 1.38f, 0.045f);
            blade.GetComponent<Renderer>().material = CreateMaterial(new Color(0.2f, 0.95f, 1.0f), "Mat_GleamingBlade");

            // Blade Tip
            GameObject tip = new GameObject("Blade_Tip");
            tip.transform.SetParent(blade.transform);
            tip.transform.localPosition = new Vector3(0f, 0.68f, 0f);

            Collider bladeCollider = blade.GetComponent<Collider>();
            bladeCollider.isTrigger = true;
            Rigidbody bladeRb = blade.AddComponent<Rigidbody>();
            bladeRb.isKinematic = true;

            // Visual Blade Trail (Cyan arc)
            TrailRenderer trail = tip.AddComponent<TrailRenderer>();
            trail.time = 0.2f;
            trail.startWidth = 0.22f;
            trail.endWidth = 0f;
            trail.emitting = false;
            Material trailMat = new Material(Shader.Find("Sprites/Default"));
            trailMat.color = new Color(0.1f, 0.95f, 1f, 0.75f);
            trail.material = trailMat;

            SwordPlaneController swordController = player.AddComponent<SwordPlaneController>();
            typeof(SwordPlaneController).GetField("swordPivot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(swordController, swordPivot.transform);
            typeof(SwordPlaneController).GetField("bladeTip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(swordController, tip.transform);
            typeof(SwordPlaneController).GetField("bladeCollider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(swordController, bladeCollider);
            typeof(SwordPlaneController).GetField("bladeTrail", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(swordController, trail);

            blade.AddComponent<ClashDetector>();

            // 2b. Player Shield (Golden Bronze)
            GameObject shieldObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shieldObj.name = "Player_Shield";
            shieldObj.transform.SetParent(player.transform);
            shieldObj.transform.localPosition = new Vector3(-0.45f, 0.12f, 0.5f);
            shieldObj.transform.localScale = new Vector3(0.58f, 0.82f, 0.1f);
            shieldObj.GetComponent<Renderer>().material = CreateMaterial(new Color(0.85f, 0.58f, 0.18f), "Mat_BronzeShield");

            Collider shieldCollider = shieldObj.GetComponent<Collider>();
            shieldCollider.isTrigger = true;

            ShieldStanceManager shieldManager = player.AddComponent<ShieldStanceManager>();
            typeof(ShieldStanceManager).GetField("shieldTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(shieldManager, shieldObj.transform);
            typeof(ShieldStanceManager).GetField("shieldCollider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(shieldManager, shieldCollider);

            if (bladeCollider != null && shieldCollider != null)
            {
                Physics.IgnoreCollision(bladeCollider, shieldCollider);
            }

            typeof(DuelistController).GetField("swordController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(duelist, swordController);
            typeof(DuelistController).GetField("shieldController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(duelist, shieldManager);
            typeof(DuelistController).GetField("characterController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(duelist, cc);

            // 3. Target Dummy Opponent (Crimson Red)
            GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dummy.name = "Target_Dummy";
            dummy.transform.position = new Vector3(0f, 1f, 0.5f);
            dummy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            dummy.GetComponent<Renderer>().material = CreateMaterial(new Color(0.85f, 0.22f, 0.22f), "Mat_CrimsonOpponent");

            typeof(DuelistController).GetField("opponentTarget", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(duelist, dummy.transform);

            // Dummy Shield (Dark Iron Red)
            GameObject dummyShield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummyShield.name = "Dummy_Shield";
            dummyShield.transform.SetParent(dummy.transform);
            dummyShield.transform.localPosition = new Vector3(0.38f, 0.25f, 0.55f);
            dummyShield.transform.localScale = new Vector3(0.6f, 0.85f, 0.12f);
            dummyShield.GetComponent<Renderer>().material = CreateMaterial(new Color(0.45f, 0.14f, 0.14f), "Mat_DummyShield");

            Collider dummyShieldCollider = dummyShield.GetComponent<Collider>();
            dummyShieldCollider.isTrigger = true;

            ShieldStanceManager dummyShieldManager = dummy.AddComponent<ShieldStanceManager>();
            typeof(ShieldStanceManager).GetField("shieldTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(dummyShieldManager, dummyShield.transform);
            typeof(ShieldStanceManager).GetField("shieldCollider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(dummyShieldManager, dummyShieldCollider);
            dummyShieldManager.SetStance(ShieldStance.HighRight);

            // 4. Combat Debug UI
            GameObject uiGo = new GameObject("Combat_Debug_UI");
            uiGo.AddComponent<CombatDebugUI>();

            // 5. Centered Duel Camera (Close proximity, smooth damping, lock-on toggle)
            Camera cam = Camera.main;
            if (cam != null)
            {
                DuelCameraRig oldRig = cam.GetComponent<DuelCameraRig>();
                if (oldRig != null) DestroyImmediate(oldRig);

                CenteredDuelCamera centeredCam = cam.GetComponent<CenteredDuelCamera>();
                if (centeredCam == null) centeredCam = cam.gameObject.AddComponent<CenteredDuelCamera>();
                centeredCam.SetTargets(player.transform, dummy.transform);
            }

            Debug.Log("<color=green>[Crossguard] High-Contrast Arena successfully created!</color>");
        }

        private static Material CreateMaterial(Color color, string name)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Diffuse");

            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            return mat;
        }
    }
}
