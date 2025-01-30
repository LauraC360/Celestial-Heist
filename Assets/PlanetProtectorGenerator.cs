using UnityEngine;

public class PlanetProtectorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject spaceship;
    [SerializeField] private int spaceshipCount;
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;

    public void GeneratePlanetProtectors()
    {
        for (int i = 0; i < spaceshipCount; i++)
        {
            GameObject spaceshipClone = World.Instance.Instantiate(spaceship);
            
            float radius = Random.Range(minRadius, maxRadius);
            
            Vector3 randomDirection = Random.onUnitSphere;
            
            spaceshipClone.transform.position = transform.position + randomDirection * radius;
            spaceshipClone.GetComponent<PlanetProtector>().SetDirection(randomDirection);
        }
    }
}
