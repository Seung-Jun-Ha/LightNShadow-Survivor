using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace LightNShadowSurvivor.Tests
{
    public class ReleaseBalancePlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                if (obj != null && obj.name.StartsWith("ReleaseBalanceTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            ResetSingleton("LightNShadowSurvivor.PlayerController");
            ResetSingleton("LightNShadowSurvivor.PlayerExperience");
        }

        [Test]
        public void SkillUpgradesUseReleaseValues()
        {
            GameObject player = new GameObject("ReleaseBalanceTest_Player");
            player.AddComponent<Rigidbody>();
            Component controller = player.AddComponent(FindType("LightNShadowSurvivor.PlayerController"));
            Component health = player.AddComponent(FindType("LightNShadowSurvivor.PlayerHealth"));
            Component attack = player.AddComponent(FindType("LightNShadowSurvivor.FlashLightAttack"));

            Assert.AreEqual(1f, GetField<float>(attack, "damagePerSecond"), 0.0001f);
            Assert.AreEqual(4f, (float)controller.GetType().GetProperty("MoveSpeed").GetValue(controller), 0.0001f);

            Invoke(attack, "IncreaseDamage", 0.5f);
            Assert.AreEqual(1.5f, GetField<float>(attack, "damagePerSecond"), 0.0001f);

            float initialRange = GetField<float>(attack, "range");
            Invoke(attack, "IncreaseDiameterPercent", 0.1f);
            Assert.AreEqual(initialRange * 1.1f, GetField<float>(attack, "range"), 0.0001f);

            float initialSpeed = (float)controller.GetType().GetProperty("MoveSpeed").GetValue(controller);
            Invoke(controller, "IncreaseMoveSpeedPercent", 0.1f);
            Assert.AreEqual(initialSpeed * 1.1f, (float)controller.GetType().GetProperty("MoveSpeed").GetValue(controller), 0.0001f);

            Invoke(health, "IncreaseDurability", 0.1f);
            Invoke(health, "TakeDamage", 10f);
            Assert.AreEqual(91f, (float)health.GetType().GetProperty("CurrentHealth").GetValue(health), 0.0001f);
        }

        [Test]
        public void ExperienceRequirementsStopAtLevelFive()
        {
            Component experience = new GameObject("ReleaseBalanceTest_Experience")
                .AddComponent(FindType("LightNShadowSurvivor.PlayerExperience"));

            AssertExperienceState(experience, 1, 10f);
            Invoke(experience, "AddXP", 10f);
            AssertExperienceState(experience, 2, 15f);
            Invoke(experience, "AddXP", 15f);
            AssertExperienceState(experience, 3, 20f);
            Invoke(experience, "AddXP", 20f);
            AssertExperienceState(experience, 4, 25f);
            Invoke(experience, "AddXP", 25f);
            AssertExperienceState(experience, 5, 30f);
            Invoke(experience, "AddXP", 100f);
            AssertExperienceState(experience, 5, 30f);
        }

        [Test]
        public void RoundMonsterStatsAndRemovedBehaviorsAreEnforced()
        {
            Component spawner = new GameObject("ReleaseBalanceTest_Spawner")
                .AddComponent(FindType("LightNShadowSurvivor.MonsterSpawner"));

            GameObject round1Monster = new GameObject("ReleaseBalanceTest_Round1");
            MonoBehaviour teleport = (MonoBehaviour)round1Monster.AddComponent(FindType("LightNShadowSurvivor.TeleportGhost"));
            MonoBehaviour shield = (MonoBehaviour)round1Monster.AddComponent(FindType("LightNShadowSurvivor.ShieldGhost"));
            MonoBehaviour blob = (MonoBehaviour)round1Monster.AddComponent(FindType("LightNShadowSurvivor.BlobShadow"));
            Invoke(spawner, "SetRound", 1);
            Invoke(spawner, "ConfigureRegularMonster", round1Monster);

            Component round1Base = round1Monster.GetComponent(FindType("LightNShadowSurvivor.MonsterBase"));
            Assert.IsFalse(teleport.enabled);
            Assert.IsFalse(shield.enabled);
            Assert.IsFalse(blob.enabled);
            Assert.AreEqual(1f, Invoke<float>(spawner, "GetCurrentSpawnInterval"), 0.0001f);
            Assert.AreEqual(2f, (float)round1Base.GetType().GetProperty("MaxHealth").GetValue(round1Base), 0.0001f);
            Assert.AreEqual(3f, GetField<float>(round1Monster.GetComponent(FindType("LightNShadowSurvivor.MonsterDeathHandler")), "xpValue"), 0.0001f);
            Assert.AreEqual(3.5f, GetField<float>(round1Monster.GetComponent(FindType("LightNShadowSurvivor.GhostAI")), "moveSpeed"), 0.0001f);

            GameObject round2Monster = new GameObject("ReleaseBalanceTest_Round2");
            Invoke(spawner, "SetRound", 2);
            Invoke(spawner, "ConfigureRegularMonster", round2Monster);

            Component round2Base = round2Monster.GetComponent(FindType("LightNShadowSurvivor.MonsterBase"));
            Assert.AreEqual(2f, Invoke<float>(spawner, "GetCurrentSpawnInterval"), 0.0001f);
            Assert.AreEqual(4f, (float)round2Base.GetType().GetProperty("MaxHealth").GetValue(round2Base), 0.0001f);
            Assert.AreEqual(5f, GetField<float>(round2Monster.GetComponent(FindType("LightNShadowSurvivor.MonsterDeathHandler")), "xpValue"), 0.0001f);
            Assert.AreEqual(4f, GetField<float>(round2Monster.GetComponent(FindType("LightNShadowSurvivor.GhostAI")), "moveSpeed"), 0.0001f);
        }

        [Test]
        public void ReleaseBossUsesThirtyHealthAndKeepsPatternsDisabled()
        {
            Component spawner = new GameObject("ReleaseBalanceTest_Spawner")
                .AddComponent(FindType("LightNShadowSurvivor.MonsterSpawner"));
            GameObject bossObject = new GameObject("ReleaseBalanceTest_Boss");
            Component bossAI = bossObject.AddComponent(FindType("LightNShadowSurvivor.GhostAI"));
            Component boss = bossObject.AddComponent(FindType("LightNShadowSurvivor.BossBase"));

            Invoke(spawner, "ConfigureBoss", bossObject);

            Assert.AreEqual(30f, (float)boss.GetType().GetProperty("MaxHealth").GetValue(boss), 0.0001f);
            Assert.AreEqual(0f, (float)boss.GetType().GetProperty("CurrentShield").GetValue(boss), 0.0001f);
            Assert.IsFalse(GetField<bool>(boss, "enableBehavior"));
            Assert.AreEqual(4.5f, GetField<float>(bossAI, "moveSpeed"), 0.0001f);
            Assert.IsTrue(((MonoBehaviour)bossAI).enabled);
        }

        private static void AssertExperienceState(Component experience, int level, float requirement)
        {
            Assert.AreEqual(level, (int)experience.GetType().GetProperty("CurrentLevel").GetValue(experience));
            Assert.AreEqual(requirement, (float)experience.GetType().GetProperty("XPToNextLevel").GetValue(experience), 0.0001f);
        }

        private static T GetField<T>(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(field);
            return (T)field.GetValue(target);
        }

        private static void Invoke(object target, string methodName, object argument)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(target, new[] { argument });
        }

        private static T Invoke<T>(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(method);
            return (T)method.Invoke(target, null);
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
