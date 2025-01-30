using UnityEditor;
using UnityEngine;

public class BatchMeshSaver : MonoBehaviour
{
    public int id = 0;

    [ContextMenu("Save All Meshes and Materials in Children")]
    public void SaveAllMeshesAndMaterials()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        int num = 0;
        foreach (var meshFilter in meshFilters)
        {
            Renderer renderer = meshFilter.GetComponent<Renderer>();

            // Skip objects without a mesh or material
            if (meshFilter.sharedMesh == null)
            {
                Debug.LogWarning($"Skipping {meshFilter.gameObject.name}: No mesh found.");
                continue;
            }
            if (renderer == null || renderer.sharedMaterial == null)
            {
                Debug.LogWarning($"Skipping {meshFilter.gameObject.name}: No renderer or material found.");
                continue;
            }

            // Save the mesh
            string meshPath = $"Assets/SavedMeshes/{meshFilter.gameObject.name}_{id}-{num}_Mesh.asset";
            AssetDatabase.CreateAsset(meshFilter.sharedMesh, meshPath);

            // Iterate through all materials and create new material assets
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                Material originalMaterial = renderer.sharedMaterials[i];
                Material newMaterial = new Material(originalMaterial.shader);
                newMaterial.CopyPropertiesFromMaterial(originalMaterial);

                // Copy color properties explicitly
                if (originalMaterial.HasProperty("_Color"))
                {
                    newMaterial.SetColor("_Color", originalMaterial.GetColor("_Color"));
                }
                if (originalMaterial.HasProperty("_EmissionColor"))
                {
                    newMaterial.SetColor("_EmissionColor", originalMaterial.GetColor("_EmissionColor"));
                }

                string materialPath = $"Assets/SavedMeshes/{meshFilter.gameObject.name}_{id}-{num}_Material_{i}.mat";
                AssetDatabase.CreateAsset(newMaterial, materialPath);

                Debug.Log($"Saved material for {meshFilter.gameObject.name}:");
                Debug.Log($"  Material: {materialPath}");
            }

            Debug.Log($"Saved mesh for {meshFilter.gameObject.name}:");
            Debug.Log($"  Mesh: {meshPath}");

            num += 1;
            Debug.Log($"new num: {num}");
        }

        // Save all assets to disk
        AssetDatabase.SaveAssets();

        Debug.Log("All meshes and materials have been saved!");
    }
}
