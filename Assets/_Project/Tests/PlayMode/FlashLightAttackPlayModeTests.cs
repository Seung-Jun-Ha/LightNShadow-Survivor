using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightNShadowSurvivor.Tests
{
    public class FlashLightAttackPlayModeTests
    {
        private const int EnemyLayer = 6;
        private const int ObstacleLayer = 7;

        [SetUp]
        public void SetUp()
        {
            DisableExistingSceneFlashlights();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                if (obj.name.StartsWith("FlashlightTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }

        [UnityTest]
        public IEnumerator DamagesEnemyInsideConeAndRange()
        {
            Component attack = CreateFlashlight(range: 6f, angle: 40f, damagePerSecond: 100f);
            Component monster = CreateMonster(new Vector3(0f, 0f, 3f));
            Physics.SyncTransforms();

            yield return null;
            float before = GetCurrentHealth(monster);

            yield return null;

            Assert.Less(GetCurrentHealth(monster), before);
            Assert.IsTrue(((Light)GetFieldValue(attack, "lightComponent")).enabled);
        }

        [UnityTest]
        public IEnumerator DoesNotDamageEnemyOutsideRange()
        {
            CreateFlashlight(range: 3f, angle: 60f, damagePerSecond: 100f);
            Component monster = CreateMonster(new Vector3(0f, 0f, 6f));
            Physics.SyncTransforms();

            yield return null;
            float before = GetCurrentHealth(monster);

            yield return null;

            Assert.AreEqual(before, GetCurrentHealth(monster), 0.0001f);
        }

        [UnityTest]
        public IEnumerator ObstacleLayerBlocksLightDamage()
        {
            Component attack = CreateFlashlight(range: 6f, angle: 60f, damagePerSecond: 100f);
            SetFieldValue(attack, "obstacleLayer", (LayerMask)(1 << ObstacleLayer), BindingFlags.Instance | BindingFlags.NonPublic);
            Component monster = CreateMonster(new Vector3(0f, 0f, 3f));
            CreateObstacle(new Vector3(0f, 0f, 1.5f));
            Physics.SyncTransforms();

            yield return null;
            float before = GetCurrentHealth(monster);

            yield return null;

            Assert.AreEqual(before, GetCurrentHealth(monster), 0.0001f);
        }

        private static void DisableExistingSceneFlashlights()
        {
            foreach (MonoBehaviour behaviour in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {
                if (behaviour != null && behaviour.GetType().FullName == "LightNShadowSurvivor.FlashLightAttack")
                {
                    behaviour.enabled = false;
                }
            }
        }

        private static Component CreateFlashlight(float range, float angle, float damagePerSecond)
        {
            GameObject obj = new GameObject("FlashlightTest_Attacker");
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;

            Light light = obj.AddComponent<Light>();
            light.type = LightType.Spot;

            Component attack = obj.AddComponent(FindType("LightNShadowSurvivor.FlashLightAttack"));
            SetFieldValue(attack, "lightComponent", light, BindingFlags.Instance | BindingFlags.Public);
            SetFieldValue(attack, "range", range, BindingFlags.Instance | BindingFlags.Public);
            SetFieldValue(attack, "angle", angle, BindingFlags.Instance | BindingFlags.Public);
            SetFieldValue(attack, "damagePerSecond", damagePerSecond, BindingFlags.Instance | BindingFlags.Public);
            SetFieldValue(attack, "targetLayer", (LayerMask)(1 << EnemyLayer), BindingFlags.Instance | BindingFlags.Public);
            return attack;
        }

        private static Component CreateMonster(Vector3 position)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "FlashlightTest_Monster";
            obj.transform.position = position;
            obj.layer = EnemyLayer;

            Component monster = obj.AddComponent(FindType("LightNShadowSurvivor.MonsterBase"));
            obj.AddComponent(FindType("LightNShadowSurvivor.LightDamageReceiver"));
            return monster;
        }

        private static void CreateObstacle(Vector3 position)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "FlashlightTest_Obstacle";
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(2f, 2f, 0.2f);
            obj.layer = ObstacleLayer;
        }

        private static float GetCurrentHealth(Component monster)
        {
            PropertyInfo property = monster.GetType().GetProperty("CurrentHealth", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            return (float)property.GetValue(monster);
        }

        private static object GetFieldValue(Component component, string fieldName)
        {
            FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(field);
            return field.GetValue(component);
        }

        private static void SetFieldValue(Component component, string fieldName, object value, BindingFlags bindingFlags)
        {
            FieldInfo field = component.GetType().GetField(fieldName, bindingFlags);
            Assert.NotNull(field);
            field.SetValue(component, value);
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
