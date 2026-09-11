#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crossguard.Combat.Editor
{
    public static class CrossguardEditorSetup
    {
        [MenuItem("Crossguard/Setup Greybox Duel Arena in Active Scene", false, 1)]
        public static void SetupArenaMenu()
        {
            GameObject spawnerObj = new GameObject("Crossguard_Arena_Spawner");
            DuelArenaSpawner spawner = spawnerObj.AddComponent<DuelArenaSpawner>();
            spawner.SetupArenaIfEmpty();
            Object.DestroyImmediate(spawnerObj);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            );

            Debug.Log("<color=cyan>[Crossguard] Setup complete! You can press Play in Unity to test the combat immediately.</color>");
        }
    }
}
#endif
