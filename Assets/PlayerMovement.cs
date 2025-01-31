using System.Linq;
using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private Transform _headMount;
    [SerializeField] private XROrigin _XROrigin;
    [SerializeField] private Transform spaceship;
    [SerializeField] private SpaceshipController spaceshipController;
    [SerializeField] private Transform cameraOffset;

    [SerializeField] private InputActionProperty _leftHandMoveAction =
        new InputActionProperty(new InputAction("Left Hand Move", expectedControlType: "Vector2"));

    [SerializeField] private InputActionProperty _rightHandTurnAction =
        new InputActionProperty(new InputAction("Right Hand Turn", expectedControlType: "Vector2"));

    private CapsuleCollider _capsuleCollider;
    
    private Collider[] _colliders = new Collider[64];

    private int _layerMask;

    private Vector3 _initialCameraOffset;
    private bool _overdrive = false;

    private void Start()
    {
        _leftHandMoveAction.EnableDirectAction();
        _rightHandTurnAction.EnableDirectAction();
        
        _capsuleCollider = GetComponent<CapsuleCollider>();
        
        _layerMask = LayerMask.GetMask("Ship");
    }

    private void FixedUpdate()
    { 
        var moveDir = GetMoveInput();
        
        var movement = spaceship.rotation * new Vector3(moveDir.z, 0, moveDir.x) * moveSpeed * Time.fixedDeltaTime;
        
        Move(movement);

        var turnInput = _rightHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

        var turn = turnInput.magnitude * Mathf.Sign(turnInput.x);
        
        _XROrigin.RotateAroundCameraPosition(transform.up, turn * turnSpeed * Time.fixedDeltaTime);

        ShakeToOverdrive();
    }

    private Vector3 GetMoveInput()
    {
        var moveInput = _leftHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
        
        moveInput = moveInput.RotateVector(-_XROrigin.transform.localEulerAngles.y - _headMount.localEulerAngles.y);

        return new Vector3(moveInput.y, 0, moveInput.x);
    }

    public bool Move(Vector3 distance)
    {
        var start = transform.position;
        var target = start + distance;

        var increments = Mathf.Max(1, (target - start).magnitude / _capsuleCollider.radius);

        var height = transform.up * (_capsuleCollider.height - 0.2f) / 2;
        for (int i = 1; i <= increments; i++)
        {
            var next = start + distance / increments * i;

            if(Physics.OverlapCapsuleNonAlloc(next - height, next + height, _capsuleCollider.radius, _colliders, _layerMask) > 0)
                return false;
        }

        transform.position = target;

        return true;
    }

    private void ShakeToOverdrive()
    {
        if (!_overdrive && spaceshipController.OverdriveValue > 0)
        {
            _initialCameraOffset = cameraOffset.localPosition;
            
            _overdrive = true;
            cameraOffset.DOShakePosition(0.1f, 0.002f, 100).SetRelative(true).SetLoops(-1, LoopType.Restart);
        }
        else if (_overdrive && spaceshipController.OverdriveValue <= 0)
        {
            _overdrive = false;
            cameraOffset.DOKill();

            cameraOffset.localPosition = _initialCameraOffset;
        }
    }
}