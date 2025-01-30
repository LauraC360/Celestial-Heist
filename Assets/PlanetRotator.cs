using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlanetRotator : MonoBehaviour
{
    [SerializeField] private Transform _light;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private float rotationSpeed;
    
    private Transform _orbit;
    private Vector3 _direction;
    private float _radius;
    
    public void Setup(int index, Transform orbit)
    {
        _orbit = orbit;
        _direction = _orbit.up;
        _radius = Vector3.Distance(orbit.position, transform.position);

        var layerMask = LayerMask.NameToLayer($"Planet{index}");
        var cullingMask = LayerMask.GetMask($"Planet{index}");
        _mesh.gameObject.layer = layerMask;

        var renderingLayer = Utils.GetRenderingLayerMaskFromString("Planet" + index);
        _mesh.renderingLayerMask = (uint)renderingLayer;
        
        var lightComponent = _light.GetComponent<Light>();
        lightComponent.renderingLayerMask = renderingLayer;
        lightComponent.cullingMask = cullingMask;
        
        var additionalData = _light.GetComponent<UniversalAdditionalLightData>();
        additionalData.renderingLayers = (uint)renderingLayer;
        additionalData.shadowRenderingLayers = (uint)renderingLayer;
    }

    void Update()
    {
        if (_orbit == null)
            return;
        
        var distance = rotationSpeed * Time.deltaTime;
        var angle = distance / _radius * Mathf.Rad2Deg;
        transform.RotateAround(_orbit.position, _direction, angle);
        
        var direction = (transform.position - _orbit.position).normalized;
        _light.forward = direction;
    }
}
