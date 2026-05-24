/*
2026-05-16 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using System;
using UnityEngine;

namespace LightNShadowSurvivor
{
    public class LightDamageReceiver : MonoBehaviour
    {
        private MonsterBase monsterBase;
        private GhostAI ai;

        void Awake()
        {
            monsterBase = GetComponent<MonsterBase>();
            ai = GetComponent<GhostAI>();
        }

        public void TakeLightDamage(float damage)
        {
            if (monsterBase != null)
                monsterBase.ModifyHealth(-damage);
            
            if (ai != null)
                ai.ApplySlow(); 
        }
    }
}
