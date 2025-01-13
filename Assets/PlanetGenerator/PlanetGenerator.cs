using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    public Shader litShaderGraphics;
    private GameObject currentPlanet;

    private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh) 
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    void Start()
    {
        generateObject(new Vector3(0,-100,0));
        generateObject(new Vector3(Random.Range(0, 200), Random.Range(0, 200), Random.Range(0, 200)));
        generateObject(new Vector3(Random.Range(0, 200), Random.Range(0, 200), Random.Range(0, 200)));
        generateObject(new Vector3(Random.Range(0, 200), Random.Range(0, 200), Random.Range(0, 200)));
    }

    void generateObject(Vector3 position)
    {
        currentPlanet = new GameObject();
        currentPlanet.transform.position = position;
        Debug.Log($"Planet created at position: {position}");

        Planet planetScript = currentPlanet.AddComponent<Planet>();
        planetScript.resolution = Random.Range(50, 151);
        planetScript.origin = position;

        ShapeSettings shapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
        shapeSettings.planetRadius = Random.Range(15f, 45f); // Set a random planet radius
        shapeSettings.noiseLayers = GenerateRandomNoiseLayers(); // Generate random noise layers
        planetScript.shapeSettings = shapeSettings;

        ColourSettings colourSettings = ScriptableObject.CreateInstance<ColourSettings>();
        colourSettings.planetMaterial = new Material(litShaderGraphics); // Assign a default material
        colourSettings.gradient = generateRandomGradient(); // Generate a random gradient
        planetScript.colourSettings = colourSettings;

        planetScript.GeneratePlanet();

        Debug.Log($"Planet position after generation: {currentPlanet.transform.position}");
    }

    private ShapeSettings.NoiseLayer[] GenerateRandomNoiseLayers()
    {
        int numLayers = Random.Range(3, 7); // Generate a random number between 3 and 6
        ShapeSettings.NoiseLayer[] noiseLayers = new ShapeSettings.NoiseLayer[numLayers];

        for (int i = 0; i < numLayers; i++)
        {
            noiseLayers[i] = new ShapeSettings.NoiseLayer();
            noiseLayers[i].enabled = true;
            noiseLayers[i].useFirstLayerAsMask = Random.value > 0.5f;
            noiseLayers[i].noiseSettings = new NoiseSettings();
            noiseLayers[i].noiseSettings.filterType = (NoiseSettings.FilterType)Random.Range(0, 2); // Randomly select between Simple and Ridgid

            float strength = Random.Range(0.1f, 1f);
            float baseRoughness = Random.Range(0.1f, i * 2f + 1f);
            float persistence = Random.Range(0.1f, 0.3f);
            float minMinValue = (strength + persistence) * 0.6f;
            float maxMinValue = (strength + persistence) * 0.8f;

            if (noiseLayers[i].noiseSettings.filterType == NoiseSettings.FilterType.Simple)
            {
                noiseLayers[i].noiseSettings.simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings
                {
                    strength = strength,
                    numLayers = Random.Range(2, 5),
                    baseRoughness = baseRoughness,
                    roughness = Random.Range(0.1f, 5f),
                    persistence = persistence,
                    minValue = Random.Range(minMinValue, maxMinValue),
                    centre = new Vector3(0, 0, 0)
                };
            }
            else
            {
                noiseLayers[i].noiseSettings.ridgidNoiseSettings = new NoiseSettings.RidgidNoiseSettings
                {
                    strength = strength,
                    numLayers = Random.Range(1, 8),
                    baseRoughness = baseRoughness,
                    roughness = Random.Range(0.1f, 5f),
                    persistence = persistence,
                    centre = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)),
                    minValue = Random.Range(minMinValue * 2, maxMinValue * 4),
                    weightMultiplier = Random.Range(0.1f, 0.5f)
                };
            }
        }

        return noiseLayers;
    }

    private Gradient generateRandomGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[5];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[5];

        for (int i = 0; i < 5; i++)
        {
            colorKeys[i].color = new Color(Random.value, Random.value, Random.value);
            colorKeys[i].time = i / 4f; // Spread the color keys evenly across the gradient

            alphaKeys[i].alpha = Random.value;
            alphaKeys[i].time = i / 4f; // Spread the alpha keys evenly across the gradient
        }

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}