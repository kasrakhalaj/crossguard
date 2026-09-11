#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mosslight.Editor
{
    public static class MosslightBuilder
    {
        public const string ScenePath = "Assets/Mosslight/Scenes/Mosslight.unity";

        [MenuItem("Mosslight/Open Learning Scene", false, 1)]
        public static void OpenScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) CreateScene();
            else EditorSceneManager.OpenScene(ScenePath);
        }

        // Called by the command-line build too. Does not change Crossguard's scene
        // or the project's global build scene list. The build selects our scene explicitly.
        public static void CreateScene()
        {
            Directory.CreateDirectory("Assets/Mosslight/Scenes");
            Directory.CreateDirectory("Assets/Mosslight/Settings");
            AssetDatabase.Refresh();
            const string tuningPath = "Assets/Mosslight/Settings/LearningTuning.asset";
            var tuning = AssetDatabase.LoadAssetAtPath<MosslightTuning>(tuningPath);
            if (tuning == null)
            {
                tuning = ScriptableObject.CreateInstance<MosslightTuning>();
                AssetDatabase.CreateAsset(tuning, tuningPath);
            }
            const string materialPath = "Assets/Mosslight/Art/Paper.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                var shader = Shader.Find("Mosslight/Paper");
                if (shader == null) throw new InvalidOperationException("Mosslight/Paper shader was not imported.");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, materialPath);
            }
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("MOSSLIGHT — press Play to grow the garden");
            var game = root.AddComponent<MosslightGame>(); game.tuning = tuning; game.spriteMaterial = material;
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Mosslight/Build Windows Learning Game", false, 2)]
        public static void Build()
        {
            try
            {
                if (!File.Exists(ScenePath)) CreateScene();
                Directory.CreateDirectory("output/Mosslight");
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { ScenePath },
                    locationPathName = "output/Mosslight/Mosslight.exe",
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.Development
                });
                if (report.summary.result != BuildResult.Succeeded) throw new Exception("Mosslight build failed: " + report.summary.result);
                Debug.Log("MOSSLIGHT_BUILD_OK " + report.summary.totalSize + " bytes");
                if (Application.isBatchMode) EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (Application.isBatchMode) EditorApplication.Exit(1);
                else throw;
            }
        }
    }
}
#endif
