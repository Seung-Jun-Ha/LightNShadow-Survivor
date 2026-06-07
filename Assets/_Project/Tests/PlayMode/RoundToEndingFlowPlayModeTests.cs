using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightNShadowSurvivor.Tests
{
    public class RoundToEndingFlowPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;

            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                if (obj == null) continue;
                if (obj.name.StartsWith("RoundFlowTest_", StringComparison.Ordinal) || obj.name == "Player_Main" || obj.name == "Result_UI")
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            ResetSingleton("LightNShadowSurvivor.GameManager");
            ResetSingleton("LightNShadowSurvivor.RoundManager");
            ResetSingleton("LightNShadowSurvivor.UpgradeManager");
            ResetSingleton("LightNShadowSurvivor.PlayerController");
        }

        [UnityTest]
        public IEnumerator RoundEndOpensUpgradeAndUpgradeSelectionStartsNextRound()
        {
            Component gameManager = CreateGameManager();
            Component roundManager = CreateRoundManager(round1: 0.03f, round2: 0.2f, round3: 0.2f);
            Component upgradeManager = CreateUpgradeManager();
            yield return null;

            Invoke(gameManager, "StartRound");
            yield return WaitForState(gameManager, "Upgrade", 1f);

            Assert.AreEqual("Upgrade", GetGameState(gameManager));
            Assert.AreEqual(1, GetIntProperty(roundManager, "CurrentRound"));
            Assert.AreEqual(0f, Time.timeScale);

            Invoke(upgradeManager, "ApplyUpgrade", CreateUpgradeData());
            yield return null;

            Assert.AreEqual("Round", GetGameState(gameManager));
            Assert.AreEqual(2, GetIntProperty(roundManager, "CurrentRound"));
            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator RoundThreeWaitsForBossDefeatBeforeEnding()
        {
            Component gameManager = CreateGameManager();
            Component roundManager = CreateRoundManager(round1: 0.03f, round2: 0.03f, round3: 0.03f);
            Component upgradeManager = CreateUpgradeManager();
            yield return null;

            Invoke(gameManager, "StartRound");

            yield return WaitForState(gameManager, "Upgrade", 1f);
            Assert.AreEqual(1, GetIntProperty(roundManager, "CurrentRound"));
            Invoke(upgradeManager, "ApplyUpgrade", CreateUpgradeData());

            yield return WaitForState(gameManager, "Upgrade", 1f);
            Assert.AreEqual(2, GetIntProperty(roundManager, "CurrentRound"));
            Invoke(upgradeManager, "ApplyUpgrade", CreateUpgradeData());

            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual("Round", GetGameState(gameManager));
            Assert.AreEqual(3, GetIntProperty(roundManager, "CurrentRound"));

            Component boss = new GameObject("RoundFlowTest_Boss").AddComponent(FindType("LightNShadowSurvivor.BossBase"));
            Invoke(boss, "ConfigureBoss", 30f);
            Invoke(boss, "ModifyHealth", -30f);
            yield return WaitForState(gameManager, "Ending", 1f);

            Assert.AreEqual("Ending", GetGameState(gameManager));
            Assert.AreEqual(3, GetIntProperty(roundManager, "CurrentRound"));
            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator NullUpgradeSelectionResumesRoundFlow()
        {
            Component gameManager = CreateGameManager();
            Component roundManager = CreateRoundManager(round1: 0.03f, round2: 0.2f, round3: 0.2f);
            Component upgradeManager = CreateUpgradeManager();
            yield return null;

            Invoke(gameManager, "StartRound");
            yield return WaitForState(gameManager, "Upgrade", 1f);

            Invoke(upgradeManager, "ApplyUpgrade", null);
            yield return null;

            Assert.AreEqual("Round", GetGameState(gameManager));
            Assert.AreEqual(2, GetIntProperty(roundManager, "CurrentRound"));
            Assert.AreEqual(1f, Time.timeScale);
        }

        [Test]
        public void StageManagerCompletesStagesAndStopsAtTotal()
        {
            Component stageManager = new GameObject("RoundFlowTest_StageManager").AddComponent(FindType("LightNShadowSurvivor.StageManager"));
            SetFieldValue(stageManager, "totalStages", 2, BindingFlags.Instance | BindingFlags.NonPublic);

            int completedCount = 0;
            int allClearedCount = 0;
            EventInfo completedEvent = stageManager.GetType().GetEvent("OnStageCompleted", BindingFlags.Instance | BindingFlags.Public);
            EventInfo allClearedEvent = stageManager.GetType().GetEvent("OnAllStagesCleared", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(completedEvent);
            Assert.NotNull(allClearedEvent);
            completedEvent.AddEventHandler(stageManager, new Action<int>(_ => completedCount++));
            allClearedEvent.AddEventHandler(stageManager, new Action(() => allClearedCount++));

            Invoke(stageManager, "CompleteStage");
            Invoke(stageManager, "CompleteStage");
            Invoke(stageManager, "CompleteStage");

            Assert.AreEqual(2, GetIntProperty(stageManager, "CurrentStage"));
            Assert.IsTrue((bool)GetPropertyValue(stageManager, "AllStagesCleared"));
            Assert.AreEqual(2, completedCount);
            Assert.AreEqual(1, allClearedCount);
        }

        [UnityTest]
        public IEnumerator EndingManagerDisablesPlayerActionsAndShowsResultUI()
        {
            Component gameManager = CreateGameManager();
            CreatePlayerMain();
            GameObject resultUI = new GameObject("Result_UI");
            Light sun = new GameObject("RoundFlowTest_Sun").AddComponent<Light>();
            Component endingManager = new GameObject("RoundFlowTest_EndingManager").AddComponent(FindType("LightNShadowSurvivor.EndingManager"));
            SetFieldValue(endingManager, "directionalLight", sun, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(endingManager, "sunriseDuration", 0.02f, BindingFlags.Instance | BindingFlags.NonPublic);
            yield return null;

            Invoke(gameManager, "TriggerEnding");

            float timeout = Time.realtimeSinceStartup + 1.5f;
            while (Time.realtimeSinceStartup < timeout && sun.intensity < 1.4f)
            {
                yield return null;
            }

            GameObject player = GameObject.Find("Player_Main");
            Assert.NotNull(player);
            Assert.IsFalse(((MonoBehaviour)player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"))).enabled);
            Assert.IsFalse(((MonoBehaviour)player.GetComponentInChildren(FindType("LightNShadowSurvivor.FlashLightAttack"))).enabled);
            Assert.IsTrue(resultUI.activeSelf);
            Assert.GreaterOrEqual(sun.intensity, 1.4f);
        }

        private static Component CreateGameManager()
        {
            Component gameManager = new GameObject("RoundFlowTest_GameManager").AddComponent(FindType("LightNShadowSurvivor.GameManager"));
            ((MonoBehaviour)gameManager).enabled = false;
            return gameManager;
        }

        private static Component CreateRoundManager(float round1, float round2, float round3)
        {
            Component roundManager = new GameObject("RoundFlowTest_RoundManager").AddComponent(FindType("LightNShadowSurvivor.RoundManager"));
            SetFieldValue(roundManager, "round1Duration", round1, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(roundManager, "round2Duration", round2, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(roundManager, "round3Duration", round3, BindingFlags.Instance | BindingFlags.NonPublic);
            return roundManager;
        }

        private static Component CreateUpgradeManager()
        {
            return new GameObject("RoundFlowTest_UpgradeManager").AddComponent(FindType("LightNShadowSurvivor.UpgradeManager"));
        }

        private static ScriptableObject CreateUpgradeData()
        {
            Type upgradeDataType = FindType("LightNShadowSurvivor.UpgradeData");
            ScriptableObject data = ScriptableObject.CreateInstance(upgradeDataType);
            upgradeDataType.GetField("upgradeName", BindingFlags.Instance | BindingFlags.Public).SetValue(data, "RoundFlowTest Upgrade");
            upgradeDataType.GetField("increaseValue", BindingFlags.Instance | BindingFlags.Public).SetValue(data, 0f);
            Type upgradeType = FindType("LightNShadowSurvivor.UpgradeType");
            upgradeDataType.GetField("upgradeType", BindingFlags.Instance | BindingFlags.Public).SetValue(data, Enum.Parse(upgradeType, "MoveSpeed"));
            return data;
        }

        private static void CreatePlayerMain()
        {
            GameObject player = new GameObject("Player_Main");
            player.AddComponent<Rigidbody>();
            player.AddComponent(FindType("LightNShadowSurvivor.PlayerController"));

            GameObject flashlight = new GameObject("RoundFlowTest_Flashlight");
            flashlight.transform.SetParent(player.transform);
            Light light = flashlight.AddComponent<Light>();
            Component attack = flashlight.AddComponent(FindType("LightNShadowSurvivor.FlashLightAttack"));
            SetFieldValue(attack, "lightComponent", light, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static IEnumerator WaitForState(Component gameManager, string expectedState, float timeoutSeconds)
        {
            float timeout = Time.realtimeSinceStartup + timeoutSeconds;
            while (Time.realtimeSinceStartup < timeout)
            {
                if (GetGameState(gameManager) == expectedState)
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail($"Timed out waiting for state {expectedState}. Current state: {GetGameState(gameManager)}");
        }

        private static string GetGameState(Component gameManager)
        {
            PropertyInfo property = gameManager.GetType().GetProperty("CurrentState", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            return property.GetValue(gameManager).ToString();
        }

        private static int GetIntProperty(Component component, string propertyName)
        {
            PropertyInfo property = component.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            return (int)property.GetValue(component);
        }

        private static object GetPropertyValue(Component component, string propertyName)
        {
            PropertyInfo property = component.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            return property.GetValue(component);
        }

        private static void Invoke(Component component, string methodName)
        {
            MethodInfo method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(component, null);
        }

        private static void Invoke(Component component, string methodName, object argument)
        {
            MethodInfo method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(component, new[] { argument });
        }

        private static void SetFieldValue(object target, string fieldName, object value, BindingFlags bindingFlags)
        {
            FieldInfo field = target.GetType().GetField(fieldName, bindingFlags);
            Assert.NotNull(field);
            field.SetValue(target, value);
        }

        private static void ResetSingleton(string typeName)
        {
            Type type = FindType(typeName);
            FieldInfo field = type.GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, null);
            }
        }

        private static Type FindType(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail($"Type not found: {typeName}");
            return null;
        }
    }
}

