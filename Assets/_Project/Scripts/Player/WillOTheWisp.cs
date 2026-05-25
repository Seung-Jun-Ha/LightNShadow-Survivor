using UnityEngine;

namespace LightNShadowSurvivor
{
    public class WillOTheWisp : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float orbitSpeed = 100f;
        [SerializeField] private float orbitDistance = 2f;
        
        private Transform target;
        private float angle;

        public void Initialize(Transform target, float startAngle, float distance, float dmg)
        {
            this.target = target;
            this.angle = startAngle;
            this.orbitDistance = distance;
            this.damage = dmg;
        }

        void Update()
        {
            if (target == null) return;

            angle += orbitSpeed * Time.deltaTime;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0.5f, Mathf.Sin(rad)) * orbitDistance;
            transform.position = target.position + offset;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<MonsterBase>(out var monster))
                {
                    monster.ModifyHealth(-damage);
                }
            }
        }
    }
}
