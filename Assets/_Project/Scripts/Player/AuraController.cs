using UnityEngine;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class AuraController : MonoBehaviour
    {
        public static AuraController Instance { get; private set; }

        [SerializeField] private GameObject wispPrefab;
        [SerializeField] private float baseDamage = 15f;
        [SerializeField] private float orbitDistance = 2.5f;
        
        private List<WillOTheWisp> activeWisps = new List<WillOTheWisp>();
        private int wispCount = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddWisp()
        {
            wispCount++;
            RefreshWisps();
        }

        public void IncreaseDamage(float amount)
        {
            baseDamage += amount;
            RefreshWisps();
        }

        private void RefreshWisps()
        {
            // Clear existing
            foreach (var w in activeWisps) if (w != null) Destroy(w.gameObject);
            activeWisps.Clear();

            if (wispPrefab == null) return;

            // Spawn new set with equal spacing
            for (int i = 0; i < wispCount; i++)
            {
                float angle = (360f / wispCount) * i;
                GameObject go = Instantiate(wispPrefab);
                if (go.TryGetComponent<WillOTheWisp>(out var wisp))
                {
                    wisp.Initialize(transform, angle, orbitDistance, baseDamage);
                    activeWisps.Add(wisp);
                }
            }
        }
    }
}
