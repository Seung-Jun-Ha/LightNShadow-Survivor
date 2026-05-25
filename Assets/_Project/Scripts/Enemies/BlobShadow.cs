using UnityEngine;

namespace LightNShadowSurvivor
{
    public class BlobShadow : MonoBehaviour
    {
        [SerializeField] private float shadowSize = 1.2f;
        [SerializeField] private float heightOffset = 0.05f;
        [SerializeField] private Texture2D shadowTexture;
        
        private GameObject shadowQuad;
        private Transform shadowTransform;

        void Start()
        {
            CreateShadowQuad();
        }

        private void CreateShadowQuad()
        {
            shadowQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            shadowQuad.name = "BlobShadow";
            shadowQuad.transform.SetParent(transform);
            
            // Remove collider
            Destroy(shadowQuad.GetComponent<Collider>());
            
            shadowTransform = shadowQuad.transform;
            shadowTransform.localScale = new Vector3(shadowSize, shadowSize, 1f);
            shadowTransform.localRotation = Quaternion.Euler(90, 0, 0);
            
            // Set Material
            Renderer renderer = shadowQuad.GetComponent<Renderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            
            // Use URP Unlit shader
            Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
            Material mat = new Material(unlitShader);
            
            if (shadowTexture != null)
            {
                mat.mainTexture = shadowTexture;
            }
            else
            {
                // Fallback to simple color if texture not assigned
                mat.color = new Color(0, 0, 0, 0.4f);
            }

            // Set to Transparent mode
            mat.SetFloat("_Surface", 1); // 1 = Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            
            renderer.material = mat;
        }

        void LateUpdate()
        {
            if (shadowTransform == null) return;

            // Keep shadow on ground
            // We can raycast down from the monster's position
            // We ignore the monster's own collider by using a layer mask
            int groundLayer = 1 << LayerMask.NameToLayer("Ground");
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f, groundLayer))
            {
                shadowTransform.position = hit.point + Vector3.up * heightOffset;
            }
            else
            {
                // Fallback: place it on a flat plane at y=0 if no ground hit
                shadowTransform.position = new Vector3(transform.position.x, heightOffset, transform.position.z);
            }
            
            // Keep rotation flat facing up
            shadowTransform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
