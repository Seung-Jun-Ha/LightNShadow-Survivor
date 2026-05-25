using UnityEngine;

namespace LightNShadowSurvivor
{
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField] protected float lifeTime = 15f;
        [SerializeField] protected float magnetDistance = 5f;
        [SerializeField] protected float collectDistance = 1.2f;
        [SerializeField] protected float moveSpeed = 8f;

        protected Transform player;
        protected bool isCollecting = false;

        protected virtual void Start()
        {
            if (PlayerController.Instance != null) player = PlayerController.Instance.transform;
            Destroy(gameObject, lifeTime);
        }

        protected virtual void Update()
        {
            if (player == null)
            {
                if (PlayerController.Instance != null) player = PlayerController.Instance.transform;
                if (player == null) return;
            }

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance < magnetDistance) isCollecting = true;

            if (isCollecting)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                if (distance < collectDistance)
                {
                    OnCollect();
                    Destroy(gameObject);
                }
            }

            // Visual: gentle float/rotation
            transform.Rotate(Vector3.up, 90f * Time.deltaTime);
            transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.002f;
        }

        protected abstract void OnCollect();
    }
}
