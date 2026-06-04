using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightNShadowSurvivor.Tests
{
    public class PlayerAimPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                if (obj != null && obj.name.StartsWith("PlayerAimTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }

        [UnityTest]
        public IEnumerator AimFlashlightRotatesLightTowardTarget()
        {
            GameObject player = new GameObject("PlayerAimTest_Player");
            player.transform.position = Vector3.zero;

            GameObject lightObject = new GameObject("PlayerAimTest_Light");
            lightObject.transform.SetParent(player.transform);
            lightObject.transform.localPosition = Vector3.up;
            Light light = lightObject.AddComponent<Light>();

            Component aim = player.AddComponent(FindType("LightNShadowSurvivor.PlayerAim"));
            SetFieldValue(aim, "aimLight", light, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(aim, "rotationSmoothSpeed", 1000f, BindingFlags.Instance | BindingFlags.NonPublic);
            yield return null;

            Vector3 targetPoint = new Vector3(8f, 1f, 0f);
            SetFieldValue(aim, "currentTargetPoint", targetPoint, BindingFlags.Instance | BindingFlags.NonPublic);
            InvokePrivate(aim, "AimFlashlight");
            yield return null;

            Vector3 expectedDirection = (targetPoint - lightObject.transform.position).normalized;
            Assert.Less(Vector3.Angle(lightObject.transform.forward, expectedDirection), 1f);
        }

        [Test]
        public void VerticalAimTargetIsClampedRelativeToPlayer()
        {
            GameObject player = new GameObject("PlayerAimTest_Player");
            player.transform.position = new Vector3(0f, 5f, 0f);
            Component aim = player.AddComponent(FindType("LightNShadowSurvivor.PlayerAim"));
            SetFieldValue(aim, "verticalLimit", 2f, BindingFlags.Instance | BindingFlags.NonPublic);

            Vector3 highTarget = (Vector3)InvokePrivate(aim, "ClampVerticalTarget", new Vector3(4f, 20f, 3f));
            Vector3 lowTarget = (Vector3)InvokePrivate(aim, "ClampVerticalTarget", new Vector3(4f, -20f, 3f));

            Assert.AreEqual(7f, highTarget.y);
            Assert.AreEqual(3f, lowTarget.y);
            Assert.AreEqual(4f, highTarget.x);
            Assert.AreEqual(3f, highTarget.z);
        }

        private static void InvokePrivate(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(target, null);
        }

        private static object InvokePrivate(object target, string methodName, object argument)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);
            return method.Invoke(target, new[] { argument });
        }

        private static void SetFieldValue(object target, string fieldName, object value, BindingFlags bindingFlags)
        {
            FieldInfo field = target.GetType().GetField(fieldName, bindingFlags);
            Assert.NotNull(field);
            field.SetValue(target, value);
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
