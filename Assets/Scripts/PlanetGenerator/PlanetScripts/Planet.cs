using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    [HideInInspector]
    public int planet_id;
    [HideInInspector]
    public Vector3 origin;

    [Range(2,256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.SetParent(transform);
                meshObj.transform.localPosition = Vector3.zero; 
                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                
                meshObj.AddComponent<MeshCollider>();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i], meshFilters[i].gameObject);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }

        // Add the PlanetGravity script
        gameObject.AddComponent<PlanetGravity>();

        gameObject.tag = "Planet"; // Set the tag to "Planet"
    }

    public void GeneratePlanet(int planet_id)
    {
        gameObject.tag = "Planet";
        gameObject.name = $"Planet {planet_id}";
        this.planet_id = planet_id;

        Initialize();
        GenerateMesh();
        GenerateColours();
        UpdateColliders(); 
    }

    private void UpdateColliders()
    {
        for (int i = 0; i < 6; i++)
        {
            MeshCollider meshCollider = meshFilters[i].GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = meshFilters[i].sharedMesh;
            }
        }
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours()
    {
        colourGenerator.UpdateColours();
    }

    public float GetSurfaceHeight(Vector3 point, int id)
    {
        Vector3 direction = point.normalized;
        float planetRadius = shapeSettings.planetRadius;

        Ray ray = new Ray(direction * (planetRadius * 10), -direction);
        RaycastHit hit;

        foreach (var meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.GetComponent<MeshCollider>();
            if (meshCollider.Raycast(ray, out hit, planetRadius * 10))
            {
                Debug.Log($"Surface height at point {point} on for the plant id {id}, planet id {planet_id}: {hit.point.magnitude}");
                return hit.point.magnitude;
            }
        }

        return planetRadius;
    }
}