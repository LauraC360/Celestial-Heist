using System;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float thrustMultiplier;
    [SerializeField] private float brakeMultiplier;
    [SerializeField] private float rollMultiplier;

    private Rigidbody _rigidbody;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rigidbody.AddForce(Vector3.right * (inputManager.Thrust() * thrustMultiplier), ForceMode.Acceleration);
        if(_rigidbody.velocity.magnitude > 0)
            _rigidbody.AddForce(-_rigidbody.velocity.normalized * inputManager.Airbrake() * brakeMultiplier, ForceMode.Acceleration);
        
        _rigidbody.AddTorque(Vector3.right * inputManager.Roll() * rollMultiplier, ForceMode.Acceleration);
        _rigidbody.AddTorque(Vector3.up * inputManager.Yaw() * rollMultiplier, ForceMode.Acceleration);
        _rigidbody.AddTorque(Vector3.forward * inputManager.Pitch() * rollMultiplier, ForceMode.Acceleration);
    }
}
