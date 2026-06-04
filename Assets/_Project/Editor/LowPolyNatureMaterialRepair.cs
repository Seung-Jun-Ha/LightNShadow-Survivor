using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace LightNShadow.Editor
{
    internal static class LowPolyNatureMaterialRepair
    {
        private const string AssetRoot =
            "Assets/ThirdParty/Pure Poly/Free Low Poly Nature Pack";
        private const string PalettePath =
            AssetRoot + "/Textures/PP_Color_Palette.png";
        private const string SessionKey =
            "LightNShadow.LowPolyNatureMaterialRepair.V2.Completed";
        private const string UrpLitShaderName =
            "Universal Render Pipeline/Lit";

        [InitializeOnLoadMethod]
        private static void ScheduleAutomaticRepair()
        {
            if (SessionState.GetBool(SessionKey, false))
            {
                return;
            }

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += RepairSilently;
        }

        [MenuItem("Tools/LightNShadow/Repair Free Low Poly Nature Materials")]
        private static void RepairFromMenu()
        {
            Repair(logResult: true);
        }

        private static void RepairSilently()
        {
            Repair(logResult: true);
        }

        private static void Repair(bool logResult)
        {
            if (GraphicsSettings.currentRenderPipeline == null)
            {
                Debug.LogWarning(
                    "Free Low Poly Nature material repair skipped because URP is not active.");
                return;
            }

            Shader urpLit = Shader.Find(UrpLitShaderName);
            if (urpLit == null)
            {
                Debug.LogError(
                    $"Free Low Poly Nature material repair could not find '{UrpLitShaderName}'.");
                return;
            }

            Texture palette = AssetDatabase.LoadAssetAtPath<Texture>(PalettePath);
            string[] materialGuids = AssetDatabase.FindAssets(
                "t:Material",
                new[] { AssetRoot + "/Materials" });

            int repairedCount = 0;
            var repairedPaths = new List<string>();
            foreach (string materialGuid in materialGuids)
            {
                string materialPath = AssetDatabase.GUIDToAssetPath(materialGuid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null)
                {
                    continue;
                }

                material.shader = urpLit;

                if (material.name == "PP_Standard_Material" && palette != null)
                {
                    material.SetTexture("_BaseMap", palette);
                    material.SetTexture("_MainTex", palette);
                }

                EditorUtility.SetDirty(material);
                repairedPaths.Add(materialPath);
                repairedCount++;
            }

            AssetDatabase.SaveAssets();

            foreach (string repairedPath in repairedPaths)
            {
                AssetDatabase.ImportAsset(
                    repairedPath,
                    ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            if (logResult)
            {
                int unsupportedCount = 0;
                foreach (string repairedPath in repairedPaths)
                {
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(repairedPath);
                    if (material == null || material.shader == null || !material.shader.isSupported)
                    {
                        unsupportedCount++;
                    }
                }

                Debug.Log(
                    $"Repaired {repairedCount} Free Low Poly Nature materials for URP. " +
                    $"Unsupported shaders remaining: {unsupportedCount}.");
            }
        }
    }
}
