using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace LightNShadowSurvivor.Tests
{
    public class CharacterMovementPlayModeTests
    {
        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;

            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (obj != null && obj.name.StartsWith("CharacterMoveTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }

            ResetSingleton("LightNShadowSurvivor.PlayerController");
        }

        [Test]
        public void AwakeConfiguresRigidbodyForCharacterMotion()
        {
            GameObject player = CreatePlayer(Vector3.zero);
            Component controller = player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"));
            Rigidbody rb = player.GetComponent<Rigidbody>();


            Assert.NotNull(controller);
            Assert.AreEqual(RigidbodyConstraints.FreezeRotation, rb.constraints);
            Assert.AreEqual(RigidbodyInterpolation.Interpolate, rb.interpolation);
            Assert.AreSame(controller, GetStaticPropertyValue(controller.GetType(), "Instance"));
        }

        [Test]
        public void AwakeConfiguresCapsuleAboveGround()
        {
            GameObject player = CreatePlayer(Vector3.zero);
            CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();

            Assert.NotNull(capsule);
            Assert.IsFalse(capsule.isTrigger);
            Assert.AreEqual(1, capsule.direction);
            Assert.GreaterOrEqual(capsule.height, 1.8f);
            Assert.GreaterOrEqual(capsule.radius, 0.35f);
            Assert.GreaterOrEqual(capsule.center.y, capsule.height * 0.45f);
        }

        [Test]
        public void ForwardInputAppliesVelocityAlongFacingDirection()
        {
            GameObject player = CreatePlayer(Vector3.zero);
            Component controller = player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"));
            Rigidbody rb = player.GetComponent<Rigidbody>();
            SetPropertyValue(controller, "MoveSpeed", 4f);
            SetFieldValue(controller, "moveInput", new Vector2(0f, 1f), BindingFlags.Instance | BindingFlags.NonPublic);

            InvokePrivate(controller, "FixedUpdate");

            Assert.That(rb.linearVelocity.x, Is.EqualTo(0f).Within(0.05f));
            Assert.That(rb.linearVelocity.z, Is.EqualTo(4f).Within(0.05f));
        }

        [Test]
        public void BackwardInputAppliesVelocityOppositeFacingDirection()
        {
            GameObject player = CreatePlayer(Vector3.zero);
            Component controller = player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"));
            Rigidbody rb = player.GetComponent<Rigidbody>();
            SetPropertyValue(controller, "MoveSpeed", 3f);
            SetFieldValue(controller, "moveInput", new Vector2(0f, -1f), BindingFlags.Instance | BindingFlags.NonPublic);

            InvokePrivate(controller, "FixedUpdate");

            Assert.That(rb.linearVelocity.x, Is.EqualTo(0f).Within(0.05f));
            Assert.That(rb.linearVelocity.z, Is.EqualTo(-3f).Within(0.05f));
        }

        [Test]
        public void HorizontalInputRotatesCharacterWithoutAddingMovementVelocity()
        {
            GameObject player = CreatePlayer(Vector3.zero);
            Component controller = player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"));
            Rigidbody rb = player.GetComponent<Rigidbody>();
            Quaternion before = rb.rotation;
            SetFieldValue(controller, "moveInput", new Vector2(1f, 0f), BindingFlags.Instance | BindingFlags.NonPublic);

            InvokePrivate(controller, "FixedUpdate");
            InvokePrivate(controller, "FixedUpdate");

            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(before.eulerAngles.y, rb.rotation.eulerAngles.y));
            Assert.Greater(yawDelta, 0.5f);
            Assert.That(rb.linearVelocity.x, Is.EqualTo(0f).Within(0.05f));
            Assert.That(rb.linearVelocity.z, Is.EqualTo(0f).Within(0.05f));
            Assert.That(rb.angularVelocity.magnitude, Is.EqualTo(0f).Within(0.05f));
        }

        [Test]
        public void MovementPreservesExistingVerticalVelocity()
        {
            GameObject player = CreatePlayer(Vector3.zero);
            Component controller = player.GetComponent(FindType("LightNShadowSurvivor.PlayerController"));
            Rigidbody rb = player.GetComponent<Rigidbody>();
            SetPropertyValue(controller, "MoveSpeed", 2f);
            rb.linearVelocity = new Vector3(0f, 5f, 0f);
            SetFieldValue(controller, "moveInput", new Vector2(0f, 1f), BindingFlags.Instance | BindingFlags.NonPublic);

            InvokePrivate(controller, "FixedUpdate");

            Assert.That(rb.linearVelocity.y, Is.EqualTo(5f).Within(0.2f));
            Assert.That(rb.linearVelocity.z, Is.EqualTo(2f).Within(0.05f));
        }

        private static GameObject CreatePlayer(Vector3 position)
        {
            GameObject player = new GameObject("CharacterMoveTest_Player");
            player.transform.position = position;
            player.transform.rotation = Quaternion.identity;
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.useGravity = false;
            player.AddComponent(FindType("LightNShadowSurvivor.PlayerController"));
            return player;
        }

        private static void InvokePrivate(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method.Invoke(target, null);
        }

        private static void SetFieldValue(object target, string fieldName, object value, BindingFlags bindingFlags)
        {
            FieldInfo field = target.GetType().GetField(fieldName, bindingFlags);
            Assert.NotNull(field);
            field.SetValue(target, value);
        }

        private static void SetPropertyValue(object target, string propertyName, object value)
        {
            PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            property.SetValue(target, value);
        }

        private static object GetStaticPropertyValue(Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
            Assert.NotNull(property);
            return property.GetValue(null);
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

