using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlanetRotator : MonoBehaviour
{
    [SerializeField] private Transform _light;
    [SerializeField] private Transform _meshChild;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float ownAxisRotationSpeed;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    
    private Transform _orbit;
    private Vector3 _direction;
    private Vector3 _ownAxis;
    private float _radius;
    private float _ownAxisMultiplier;
    
    public void Setup(int index, Transform orbit, float maxRadius)
    {
        _orbit = orbit;
        _direction = _orbit.up;
        _ownAxis = transform.right;
        _radius = Vector3.Distance(orbit.position, transform.position);
        
        var meshes = _meshChild.GetComponentsInChildren<MeshRenderer>();

        var layerMask = LayerMask.NameToLayer($"Planet{index}");
        var cullingMask = LayerMask.GetMask($"Planet{index}");
        _meshChild.gameObject.layer = layerMask;

        var renderingLayer = Utils.GetRenderingLayerMaskFromString("Planet" + index);
        foreach (var mesh in meshes)
        {
            mesh.renderingLayerMask = (uint)renderingLayer;
            mesh.gameObject.layer = layerMask;
            mesh.transform.localScale = Vector3.one;
        }
        
        var lightComponent = _light.GetComponent<Light>();
        lightComponent.renderingLayerMask = renderingLayer;
        lightComponent.cullingMask = cullingMask;
        
        var additionalData = _light.GetComponent<UniversalAdditionalLightData>();
        additionalData.renderingLayers = (uint)renderingLayer;
        additionalData.shadowRenderingLayers = (uint)renderingLayer;
        
        var scale = Random.Range(minScale, maxScale);
        transform.localScale = Vector3.one * scale;

        _ownAxisMultiplier = 0.25f + (maxRadius - _radius) / maxRadius * 0.75f;
    }

    void Update()
    {
        if (_orbit == null)
            return;
        
        var distance = rotationSpeed * Time.deltaTime;
        var angle = distance / _radius * Mathf.Rad2Deg;
        var rotation = transform.rotation;
        transform.RotateAround(_orbit.position, _direction, angle);
        
        transform.rotation = rotation;
        transform.Rotate(_ownAxis, ownAxisRotationSpeed * _ownAxisMultiplier * Time.deltaTime);
        
        var direction = (transform.position - _orbit.position).normalized;
        _light.forward = direction;
    }
}
