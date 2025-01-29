using System.Collections;
using UnityEngine;

public class SpaceshipGravity : MonoBehaviour
{
    [SerializeField] private Transform _spaceship;
    
    [SerializeField] private float gravityStrength = 9.81f;
    
    [SerializeField] private float terminalVelocity = 50f;

    private PlayerController _controller;

    private float _currentFallSpeed = 0f;

    void Awake()
    {
        _controller = GetComponent<PlayerController>();

        StartCoroutine(PostPhysicsGravity());
    }
    
    private IEnumerator PostPhysicsGravity()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            
            if (_controller != null)
            {
                var gravityDirection = _spaceship.rotation * Vector3.down; 
            
                _currentFallSpeed += gravityStrength * Time.fixedDeltaTime;
                _currentFallSpeed = Mathf.Min(_currentFallSpeed, terminalVelocity);
            
                Vector3 customGravity = gravityDirection.normalized * _currentFallSpeed;
                if (!_controller.Move(customGravity * Time.fixedDeltaTime))
                    _currentFallSpeed = 0.01f;
            }
        }
    }
}
