using System.Collections;
using UnityEngine;
using DG.Tweening;

public class RadarScreen : MonoBehaviour
{
    [SerializeField] private Radar radar;
    [SerializeField] private Transform[] blips;
    [SerializeField] private Material blipMaterial;
    [SerializeField] private float scanInterval = 3;
    [SerializeField] private float range = 2500;
    [SerializeField] private float radarRadius = 35;

    private void Start()
    {
        StartCoroutine(ScanLoop());
    }

    private IEnumerator ScanLoop()
    {
        while (true)
        {
            var ships = radar.spaceships;
            for (int i = 0; i < blips.Length; i++)
            {
                var isActive = i < ships.Count;
                blips[i].gameObject.SetActive(isActive);

                if (!isActive)
                    continue;

                var shipPos = ships[i].transform.position - radar.transform.position;
                blips[i].localPosition = new Vector3(shipPos.z, 0, shipPos.x) / range * radarRadius;
            }
            
            var color = blipMaterial.color;
            color.a = 1;
            blipMaterial.color = color;
            DOTween.To(() => blipMaterial.color.a, x =>
            {
                var color = blipMaterial.color;
                color.a = x;
                blipMaterial.color = color;
            }, 0, scanInterval);
            
            yield return new WaitForSeconds(scanInterval);
        }
    }

    private void Update()
    {
        
    }
}
