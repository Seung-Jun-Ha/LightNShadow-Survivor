using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightNShadowSurvivor.Tests
{
    public class MapPlayerMonsterCollisionPlayModeTests
    {
        private const int EnemyLayer = 6;
        private const int ObstacleLayer = 7;
        private const int GroundLayer = 8;
        private const int PlayerLayer = 9;

        [SetUp]
        public void SetUp()
        {
            DisableExistingSceneBehaviour("LightNShadowSurvivor.GhostAI");
            DisableExistingSceneBehaviour("LightNShadowSurvivor.PlayerController");
        }

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (obj.name.StartsWith("CollisionTest_", StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
            }
        }

        [Test]
        public void ProjectCollisionLayersAllowCoreInteractions()
        {
            Assert.IsFalse(Physics.GetIgnoreLayerCollision(PlayerLayer, GroundLayer), "Player must collide with Ground.");
            Assert.IsFalse(Physics.GetIgnoreLayerCollision(PlayerLayer, ObstacleLayer), "Player must collide with Obstacle.");
            Assert.IsTrue(Physics.GetIgnoreLayerCollision(PlayerLayer, EnemyLayer), "Ghost enemies are expected to pass through Player physics.");
            Assert.IsFalse(Physics.GetIgnoreLayerCollision(EnemyLayer, ObstacleLayer), "Enemy must collide with Obstacle.");
        }

        [UnityTest]
        public IEnumerator PlayerFallsOntoGroundInsteadOfPassingThroughMap()
        {
            CreateGround(new Vector3(0f, 0f, 0f), new Vector3(8f, 0.5f, 8f));
            GameObject player = CreatePhysicsPlayer(new Vector3(0f, 3f, 0f));

            for (int i = 0; i < 80; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.GreaterOrEqual(player.transform.position.y, 0.49f);
            Assert.Less(player.transform.position.y, 1.35f);
        }

        [UnityTest]
        public IEnumerator PlayerRigidbodyIsBlockedByMapObstacle()
        {
            CreateGround(new Vector3(0f, -0.1f, 0f), new Vector3(8f, 0.2f, 8f));
            GameObject player = CreatePhysicsPlayer(new Vector3(0f, 0.6f, 0f));
            CreateObstacle(new Vector3(1.2f, 0.6f, 0f), new Vector3(0.4f, 1.2f, 2f));
            Rigidbody rb = player.GetComponent<Rigidbody>();

            for (int i = 0; i < 35; i++)
            {
                rb.linearVelocity = Vector3.right * 8f;
                yield return new WaitForFixedUpdate();
            }

            Assert.Less(player.transform.position.x, 1.0f, "Player should be blocked before crossing the obstacle center.");
        }

        [UnityTest]
        public IEnumerator MonsterCloseToPlayerDealsContactRangeDamage()
        {
            GameObject player = CreateLogicPlayer(new Vector3(0f, 0f, 0f));
            Component health = player.GetComponent(FindType("LightNShadowSurvivor.PlayerHealth"));
            CreateGhostMonster(new Vector3(0f, 0f, 1f), attackRange: 2f, damageAmount: 15f, attackCooldown: 0.01f);

            float before = GetCurrentHealth(health);

            yield return null;
            yield return null;

            Assert.Less(GetCurrentHealth(health), before);
        }

        [UnityTest]
        public IEnumerator GhostAwakeCreatesEnemyCollisionBodyForHitAndContactDetection()
        {
            GameObject monster = CreateGhostMonster(new Vector3(0f, 0f, 3f), attackRange: 1f, damageAmount: 5f, attackCooldown: 1f);

            yield return null;

            Assert.AreEqual(EnemyLayer, monster.layer);
            Collider collider = monster.GetComponent<Collider>();
            Rigidbody rb = monster.GetComponent<Rigidbody>();
            Assert.NotNull(collider);
            Assert.IsTrue(collider.enabled);
            Assert.NotNull(rb);
            Assert.IsTrue(rb.isKinematic);
            Assert.IsFalse(rb.useGravity);
        }

        private static GameObject CreatePhysicsPlayer(Vector3 position)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.name = "CollisionTest_PhysicsPlayer";
            obj.layer = PlayerLayer;
            obj.transform.position = position;
            obj.transform.localScale = Vector3.one;

            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            return obj;
        }

        private static GameObject CreateLogicPlayer(Vector3 position)
        {
            GameObject obj = CreatePhysicsPlayer(position);
            obj.name = "CollisionTest_LogicPlayer";
            obj.AddComponent(FindType("LightNShadowSurvivor.PlayerController"));
            obj.AddComponent(FindType("LightNShadowSurvivor.PlayerHealth"));
            return obj;
        }

        private static GameObject CreateGhostMonster(Vector3 position, float attackRange, float damageAmount, float attackCooldown)
        {
            GameObject obj = new GameObject("CollisionTest_GhostMonster");
            obj.transform.position = position;
            obj.layer = EnemyLayer;

            Component ghost = obj.AddComponent(FindType("LightNShadowSurvivor.GhostAI"));
            SetFieldValue(ghost, "attackRange", attackRange, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(ghost, "damageAmount", damageAmount, BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(ghost, "attackCooldown", attackCooldown, BindingFlags.Instance | BindingFlags.NonPublic);
            return obj;
        }

        private static void CreateGround(Vector3 position, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "CollisionTest_Ground";
            obj.layer = GroundLayer;
            obj.transform.position = position;
            obj.transform.localScale = scale;
        }

        private static void CreateObstacle(Vector3 position, Vector3 scale)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "CollisionTest_Obstacle";
            obj.layer = ObstacleLayer;
            obj.transform.position = position;
            obj.transform.localScale = scale;
        }

        private static void DisableExistingSceneBehaviour(string fullTypeName)
        {
            foreach (MonoBehaviour behaviour in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (behaviour != null && behaviour.GetType().FullName == fullTypeName)
                {
                    behaviour.enabled = false;
                }
            }
        }

        private static float GetCurrentHealth(Component playerHealth)
        {
            PropertyInfo property = playerHealth.GetType().GetProperty("CurrentHealth", BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(property);
            return (float)property.GetValue(playerHealth);
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


