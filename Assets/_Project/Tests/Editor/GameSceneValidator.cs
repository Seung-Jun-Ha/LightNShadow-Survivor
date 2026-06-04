using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace LightNShadowSurvivor.Tests
{
    public static class GameSceneValidator
    {
        private const string GameScenePath = "Assets/_Project/Scenes/GameScene.unity";

        public static void ValidateBatch()
        {
            List<string> errors = Validate();
            ExitWithResults("GameScene core references", errors);
        }

        public static void ValidateEnemyPrefabsBatch()
        {
            List<string> errors = ValidateEnemyPrefabs();
            ExitWithResults("Enemy prefab references", errors);
        }

        public static void ValidateAudioBatch()
        {
            EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);

            var errors = new List<string>();
            RequireSingleComponent<AudioManager>(errors);
            ValidateAudio(errors);
            ExitWithResults("GameScene audio references", errors);
        }

        private static void ExitWithResults(string label, List<string> errors)
        {
            foreach (string error in errors)
            {
                Debug.LogError($"[GameSceneValidator] {error}");
            }

            if (errors.Count == 0)
            {
                Debug.Log($"[GameSceneValidator] {label} are valid.");
            }

            EditorApplication.Exit(errors.Count == 0 ? 0 : 2);
        }

        public static List<string> Validate()
        {
            EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);

            var errors = new List<string>();
            RequireSingleComponent<GameManager>(errors);
            RequireSingleComponent<RoundManager>(errors);
            RequireSingleComponent<MonsterSpawner>(errors);
            RequireSingleComponent<UpgradeManager>(errors);
            RequireSingleComponent<UIManager>(errors);
            RequireSingleComponent<EndingManager>(errors);

            ValidateSpawner(errors);
            ValidateUI(errors);
            ValidateEnding(errors);
            return errors;
        }

        public static List<string> ValidateEnemyPrefabs()
        {
            var errors = new List<string>();
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project/Prefabs/Enemies" });
            if (prefabGuids.Length == 0)
            {
                errors.Add("No enemy prefabs were found.");
                return errors;
            }

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    ValidateEnemyPrefab(path, root, errors);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            return errors;
        }

        private static void ValidateEnemyPrefab(string path, GameObject root, List<string> errors)
        {
            RequirePrefabComponent<MonsterBase>(path, root, errors);
            RequirePrefabComponent<GhostAI>(path, root, errors);
            RequirePrefabComponent<MonsterDeathHandler>(path, root, errors);
            RequirePrefabComponent<Collider>(path, root, errors);
            RequirePrefabComponent<NavMeshAgent>(path, root, errors);

            MonsterDeathHandler deathHandler = root.GetComponentInChildren<MonsterDeathHandler>(true);
            if (deathHandler != null)
            {
                SerializedProperty xpOrb = new SerializedObject(deathHandler).FindProperty("xpOrbPrefab");
                if (xpOrb == null || xpOrb.objectReferenceValue == null)
                {
                    errors.Add($"{path}: MonsterDeathHandler.xpOrbPrefab is not assigned.");
                }
            }
        }

        private static void RequirePrefabComponent<T>(string path, GameObject root, List<string> errors) where T : Component
        {
            if (root.GetComponentInChildren<T>(true) == null)
            {
                errors.Add($"{path}: missing {typeof(T).Name}.");
            }
        }

        private static void RequireSingleComponent<T>(List<string> errors) where T : Component
        {
            T[] components = Object.FindObjectsByType<T>(FindObjectsInactive.Include);
            if (components.Length != 1)
            {
                errors.Add($"Expected exactly one {typeof(T).Name}, found {components.Length}.");
            }
        }

        private static void ValidateSpawner(List<string> errors)
        {
            MonsterSpawner spawner = Object.FindAnyObjectByType<MonsterSpawner>(FindObjectsInactive.Include);
            if (spawner == null)
            {
                return;
            }

            var serialized = new SerializedObject(spawner);
            RequireObjectList(serialized, "round1Monsters", errors);
            RequireObjectList(serialized, "round2Monsters", errors);
            RequireObjectList(serialized, "round3Monsters", errors);
            RequireObjectReference(serialized, "bossPrefab", errors);
        }

        private static void ValidateUI(List<string> errors)
        {
            UIManager uiManager = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
            if (uiManager == null)
            {
                return;
            }

            var serialized = new SerializedObject(uiManager);
            foreach (string propertyName in new[]
                     {
                         "gameUIPanel", "timerText", "hpBar", "expBar", "levelText", "roundText", "levelUpPopup"
                     })
            {
                RequireObjectReference(serialized, propertyName, errors);
            }

            RequireObjectList(serialized, "upgradeButtons", errors);
        }

        private static void ValidateEnding(List<string> errors)
        {
            EndingManager endingManager = Object.FindAnyObjectByType<EndingManager>(FindObjectsInactive.Include);
            if (endingManager == null)
            {
                return;
            }

            RequireObjectReference(new SerializedObject(endingManager), "directionalLight", errors);
        }

        private static void ValidateAudio(List<string> errors)
        {
            AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>(FindObjectsInactive.Include);
            if (audioManager == null)
            {
                return;
            }

            var serialized = new SerializedObject(audioManager);
            foreach (string propertyName in new[]
                     {
                         "bgmSource", "round1Music", "round2Music", "round3Music", "endingMusic",
                         "sfxSource", "monsterDeathR1SFX", "monsterDeathR2SFX"
                     })
            {
                RequireObjectReference(serialized, propertyName, errors);
            }
        }

        private static void RequireObjectReference(SerializedObject serialized, string propertyName, List<string> errors)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                errors.Add($"{serialized.targetObject.GetType().Name}.{propertyName} was not found.");
                return;
            }

            if (property.objectReferenceValue == null)
            {
                errors.Add($"{serialized.targetObject.GetType().Name}.{propertyName} is not assigned.");
            }
        }

        private static void RequireObjectList(SerializedObject serialized, string propertyName, List<string> errors)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null || !property.isArray)
            {
                errors.Add($"{serialized.targetObject.GetType().Name}.{propertyName} is not a serialized list.");
                return;
            }

            if (property.arraySize == 0)
            {
                errors.Add($"{serialized.targetObject.GetType().Name}.{propertyName} is empty.");
                return;
            }

            bool hasMissingReference = Enumerable.Range(0, property.arraySize)
                .Select(property.GetArrayElementAtIndex)
                .Any(element => element.objectReferenceValue == null);
            if (hasMissingReference)
            {
                errors.Add($"{serialized.targetObject.GetType().Name}.{propertyName} contains a missing reference.");
            }
        }
    }
}
