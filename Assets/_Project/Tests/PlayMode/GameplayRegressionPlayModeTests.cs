using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LightNShadowSurvivor.Tests
{
    public class GameplayRegressionPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;

            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                if (obj == null) continue;
                if (obj.name.StartsWith("GameplayRegressionTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }

                if (obj.name == "RuntimeSkillSceneCanvas" ||
                    obj.name == "RuntimeUpgradeUIController" ||
                    obj.name == "RuntimeEventSystem")
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            ResetSingleton("LightNShadowSurvivor.GameManager");
            ResetSingleton("LightNShadowSurvivor.RoundManager");
            ResetSingleton("LightNShadowSurvivor.PlayerExperience");
            ResetSingleton("LightNShadowSurvivor.GameStatsManager");
            ResetSingleton("LightNShadowSurvivor.PlayerController");
            ResetSingleton("LightNShadowSurvivor.UpgradeManager");
        }

        [UnityTest]
        public IEnumerator MidRoundUpgradeResumeDoesNotResetRoundTimer()
        {
            Component gameManager = CreateGameManager();
            Component roundManager = CreateRoundManager(round1: 10f, round2: 10f, round3: 10f);
            yield return null;

            Invoke(gameManager, "StartRound");
            yield return null;
            yield return null;

            float timeBeforeUpgrade = GetFloatProperty(roundManager, "TimeRemaining");
            Assert.Less(timeBeforeUpgrade, 10f);

            Invoke(gameManager, "OpenUpgrade");
            yield return null;
            Assert.AreEqual(0f, Time.timeScale);

            Invoke(gameManager, "StartRound");
            yield return null;

            float timeAfterResume = GetFloatProperty(roundManager, "TimeRemaining");
            Assert.Less(timeAfterResume, 10f);
            Assert.LessOrEqual(timeAfterResume, timeBeforeUpgrade);
            Assert.AreEqual(1, GetIntProperty(roundManager, "CurrentRound"));
            Assert.AreEqual(1f, Time.timeScale);
        }

        [UnityTest]
        public IEnumerator PlayerDeathEventFiresOnlyOnce()
        {
            GameObject player = new GameObject("GameplayRegressionTest_Player");
            Component health = player.AddComponent(FindType("LightNShadowSurvivor.PlayerHealth"));
            yield return null;

            int deathCount = 0;
            EventInfo deathEvent = health.GetType().GetEvent("OnPlayerDeath", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(deathEvent);
            deathEvent.AddEventHandler(health, new Action(() => deathCount++));

            Invoke(health, "TakeDamage", 200f);
            Invoke(health, "TakeDamage", 200f);
            yield return null;

            Assert.AreEqual(1, deathCount);
            Assert.IsTrue((bool)GetPropertyValue(health, "IsDead"));
            Assert.AreEqual(0f, GetFloatProperty(health, "CurrentHealth"));
        }

        [UnityTest]
        public IEnumerator MonsterDeathHandlerGrantsDirectXpWhenOrbPrefabIsMissing()
        {
            Component playerExperience = new GameObject("GameplayRegressionTest_PlayerExperience").AddComponent(FindType("LightNShadowSurvivor.PlayerExperience"));
            SetFieldValue(playerExperience, "xpToNextLevel", 999f, BindingFlags.Instance | BindingFlags.NonPublic);

            GameObject monster = new GameObject("GameplayRegressionTest_Monster");
            Component monsterBase = monster.AddComponent(FindType("LightNShadowSurvivor.MonsterBase"));
            Component deathHandler = monster.AddComponent(FindType("LightNShadowSurvivor.MonsterDeathHandler"));
            SetFieldValue(deathHandler, "xpValue", 50f, BindingFlags.Instance | BindingFlags.NonPublic);
            yield return null;

            Invoke(monsterBase, "ModifyHealth", -200f);
            yield return null;

            Assert.AreEqual(50f, GetFloatProperty(playerExperience, "CurrentXP"));
        }

        [UnityTest]
        public IEnumerator PlayerDeathHandlerTriggersGameOverAfterDeathRoutine()
        {
            Component gameManager = CreateGameManager();
            GameObject player = new GameObject("GameplayRegressionTest_Player");
            player.AddComponent(FindType("LightNShadowSurvivor.PlayerController"));
            player.AddComponent(FindType("LightNShadowSurvivor.PlayerAim"));
            Component health = player.AddComponent(FindType("LightNShadowSurvivor.PlayerHealth"));
            Component deathHandler = player.AddComponent(FindType("LightNShadowSurvivor.PlayerDeathHandler"));
            SetFieldValue(deathHandler, "dissolveDuration", 0.02f, BindingFlags.Instance | BindingFlags.NonPublic);
            yield return null;

            Invoke(health, "TakeDamage", 200f);
            Assert.AreNotEqual("GameOver", GetPropertyValue(gameManager, "CurrentState").ToString());

            float timeout = Time.realtimeSinceStartup + 1f;
            while (Time.realtimeSinceStartup < timeout && GetPropertyValue(gameManager, "CurrentState").ToString() != "GameOver")
            {
                yield return null;
            }

            Assert.AreEqual("GameOver", GetPropertyValue(gameManager, "CurrentState").ToString());
            Assert.AreEqual(0f, Time.timeScale);
            Assert.IsFalse(((MonoBehaviour)player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"))).enabled);
            Assert.IsFalse(((MonoBehaviour)player.GetComponent(FindType("LightNShadowSurvivor.PlayerAim"))).enabled);
        }

        [Test]
        public void StartRoundFromTerminalStateResetsStatsAndResumesTime()
        {
            Component gameManager = CreateGameManager();
            Component stats = new GameObject("GameplayRegressionTest_Stats").AddComponent(FindType("LightNShadowSurvivor.GameStatsManager"));
            Invoke(stats, "AddKill");
            SetFieldValue(stats, "<TimeSurvived>k__BackingField", 12.5f, BindingFlags.Instance | BindingFlags.NonPublic);

            Invoke(gameManager, "TriggerGameOver");
            Assert.AreEqual(0f, Time.timeScale);

            Invoke(gameManager, "StartRound");

            Assert.AreEqual("Round", GetPropertyValue(gameManager, "CurrentState").ToString());
            Assert.AreEqual(1f, Time.timeScale);
            Assert.AreEqual(0, GetIntProperty(stats, "Kills"));
            Assert.AreEqual(0f, GetFloatProperty(stats, "TimeSurvived"));
        }

        [UnityTest]
        public IEnumerator MonsterDeathHandlerCountsKillAndDropsXpOrb()
        {
            Component stats = new GameObject("GameplayRegressionTest_Stats").AddComponent(FindType("LightNShadowSurvivor.GameStatsManager"));
            Component playerExperience = new GameObject("GameplayRegressionTest_PlayerExperience").AddComponent(FindType("LightNShadowSurvivor.PlayerExperience"));
            SetFieldValue(playerExperience, "xpToNextLevel", 999f, BindingFlags.Instance | BindingFlags.NonPublic);
            GameObject orbPrefab = new GameObject("GameplayRegressionTest_XPOrb");
            orbPrefab.AddComponent(FindType("LightNShadowSurvivor.ExperienceOrb"));

            GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monster.name = "GameplayRegressionTest_Monster";
            Component monsterBase = monster.AddComponent(FindType("LightNShadowSurvivor.MonsterBase"));
            Component deathHandler = monster.AddComponent(FindType("LightNShadowSurvivor.MonsterDeathHandler"));
            SetFieldValue(deathHandler, "xpOrbPrefab", orbPrefab, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(deathHandler, "fadeDuration", 0.01f, BindingFlags.Instance | BindingFlags.NonPublic);
            yield return null;

            Invoke(monsterBase, "ModifyHealth", -200f);
            yield return null;

            Assert.AreEqual(1, GetIntProperty(stats, "Kills"));
            Assert.NotNull(GameObject.Find("GameplayRegressionTest_XPOrb(Clone)"));
            Assert.AreEqual(0f, GetFloatProperty(playerExperience, "CurrentXP"));
            Assert.IsFalse(monster.GetComponent<Collider>().enabled);
        }

        [Test]
        public void PlayerExperienceLevelUpDoesNotRequireGameManager()
        {
            Component playerExperience = new GameObject("GameplayRegressionTest_PlayerExperience").AddComponent(FindType("LightNShadowSurvivor.PlayerExperience"));
            SetFieldValue(playerExperience, "xpToNextLevel", 10f, BindingFlags.Instance | BindingFlags.NonPublic);

            Invoke(playerExperience, "AddXP", 15f);

            Assert.AreEqual(2, GetIntProperty(playerExperience, "CurrentLevel"));
            Assert.AreEqual(5f, GetFloatProperty(playerExperience, "CurrentXP"));
        }

        [UnityTest]
        public IEnumerator SelectingUpgradeCardHidesRuntimeUpgradePanel()
        {
            Component upgradeManager = new GameObject("GameplayRegressionTest_UpgradeManager").AddComponent(FindType("LightNShadowSurvivor.UpgradeManager"));
            ScriptableObject upgrade = ScriptableObject.CreateInstance(FindType("LightNShadowSurvivor.UpgradeData"));
            SetFieldValue(upgradeManager, "autoPopulateUpgrades", false, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(upgradeManager, "allUpgrades", CreateUpgradeList(upgrade), BindingFlags.Instance | BindingFlags.NonPublic);

            Component controller = new GameObject("GameplayRegressionTest_UpgradeUIController").AddComponent(FindType("LightNShadowSurvivor.UpgradeUIController"));
            Invoke(controller, "ShowForCurrentUpgrade");
            yield return null;

            GameObject panel = GameObject.Find("SteampunkSkillPanel");
            Assert.NotNull(panel);
            Assert.IsTrue(panel.activeInHierarchy);
            Assert.AreEqual(0f, Time.timeScale);

            Component card = FindRuntimeComponent("LightNShadowSurvivor.UpgradeCardUI");
            Assert.NotNull(card);
            Button button = (Button)GetFieldValue(card, "selectButton", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(button);

            button.onClick.Invoke();
            yield return null;

            Assert.IsTrue(panel == null || !panel.activeInHierarchy);
            Assert.AreEqual(1f, Time.timeScale);

            UnityEngine.Object.DestroyImmediate(upgrade);
        }

        [Test]
        public void MonsterVisibilityFallbackIsAddedWhenOnlyShadowRendererExists()
        {
            GameObject monster = new GameObject("GameplayRegressionTest_ShadowOnlyMonster");
            GameObject shadow = GameObject.CreatePrimitive(PrimitiveType.Quad);
            shadow.name = "GameplayRegressionTest_ShadowRenderer";
            shadow.transform.SetParent(monster.transform, false);
            shadow.AddComponent(FindType("LightNShadowSurvivor.BlobShadow"));

            InvokeStatic(
                "LightNShadowSurvivor.MonsterSpawner",
                "EnsureMonsterBodyVisible",
                new object[] { monster, Color.white });

            Assert.NotNull(monster.transform.Find("RuntimeVisibleMonsterBody"));
        }

        private static Component CreateGameManager()
        {
            Component gameManager = new GameObject("GameplayRegressionTest_GameManager").AddComponent(FindType("LightNShadowSurvivor.GameManager"));
            ((MonoBehaviour)gameManager).enabled = false;
            return gameManager;
        }

        private static Component CreateRoundManager(float round1, float round2, float round3)
        {
            Component roundManager = new GameObject("GameplayRegressionTest_RoundManager").AddComponent(FindType("LightNShadowSurvivor.RoundManager"));
            SetFieldValue(roundManager, "round1Duration", round1, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(roundManager, "round2Duration", round2, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(roundManager, "round3Duration", round3, BindingFlags.Instance | BindingFlags.NonPublic);
            return roundManager;
        }

        private static float GetFloatProperty(Component component, string propertyName)
        {
            return (float)GetPropertyValue(component, propertyName);
        }

        private static int GetIntProperty(Component component, string propertyName)
        {
            return (int)GetPropertyValue(component, propertyName);
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

        private static void InvokeStatic(string typeName, string methodName, object[] arguments)
        {
            Type type = FindType(typeName);
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(null, arguments);
        }

        private static void SetFieldValue(object target, string fieldName, object value, BindingFlags bindingFlags)
        {
            FieldInfo field = target.GetType().GetField(fieldName, bindingFlags);
            Assert.NotNull(field);
            field.SetValue(target, value);
        }

        private static object GetFieldValue(object target, string fieldName, BindingFlags bindingFlags)
        {
            FieldInfo field = target.GetType().GetField(fieldName, bindingFlags);
            Assert.NotNull(field);
            return field.GetValue(target);
        }

        private static Component FindRuntimeComponent(string typeName)
        {
            Type type = FindType(typeName);
            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                Component component = obj.GetComponent(type);
                if (component != null) return component;
            }

            return null;
        }

        private static object CreateUpgradeList(ScriptableObject upgrade)
        {
            Type upgradeDataType = FindType("LightNShadowSurvivor.UpgradeData");
            Type listType = typeof(System.Collections.Generic.List<>).MakeGenericType(upgradeDataType);
            object list = Activator.CreateInstance(listType);
            listType.GetMethod("Add").Invoke(list, new object[] { upgrade });
            return list;
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
