using UnityEngine;

namespace LightNShadowSurvivor
{
    public enum UpgradeType
    {
        MoveSpeed,
        Durability,
        LightIntensity,
        LightRadius,
        AuraWisp
    }

    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "Survivor/UpgradeData")]
    public class UpgradeData : ScriptableObject
    {
        public string upgradeName;
        [TextArea]
        public string description;
        public Sprite icon;
        public UpgradeType upgradeType;
        public float increaseValue;
    }
}
