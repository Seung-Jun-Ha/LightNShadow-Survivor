using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightNShadowSurvivor.Tests
{
    public class GameplayRegressionPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;

            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (obj == null) continue;
                if (obj.name.StartsWith("GameplayRegressionTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            ResetSingleton("LightNShadowSurvivor.GameManager");
            ResetSingleton("LightNShadowSurvivor.RoundManager");
            ResetSingleton("LightNShadowSurvivor.PlayerExperience");
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
        public IEnumerator MonsterWithDeathHandlerDoesNotGrantDirectXP()
        {
            Component playerExperience = new GameObject("GameplayRegressionTest_PlayerExperience").AddComponent(FindType("LightNShadowSurvivor.PlayerExperience"));
            SetFieldValue(playerExperience, "xpToNextLevel", 999f, BindingFlags.Instance | BindingFlags.NonPublic);

            GameObject monster = new GameObject("GameplayRegressionTest_Monster");
            Component monsterBase = monster.AddComponent(FindType("LightNShadowSurvivor.MonsterBase"));
            monster.AddComponent(FindType("LightNShadowSurvivor.MonsterDeathHandler"));
            SetFieldValue(monsterBase, "experienceReward", 50f, BindingFlags.Instance | BindingFlags.NonPublic);
            yield return null;

            Invoke(monsterBase, "ModifyHealth", -200f);
            yield return null;

            Assert.AreEqual(0f, GetFloatProperty(playerExperience, "CurrentXP"));
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
