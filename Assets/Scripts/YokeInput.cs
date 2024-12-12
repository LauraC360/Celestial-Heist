using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class YokeInput : InputManager
{
    [SerializeField] private ActionBasedController leftInputs;
    [SerializeField] private ActionBasedController rightInputs;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private XRGrabInteractable leftHandle;
    [SerializeField] private XRGrabInteractable rightHandle;
    [SerializeField] private Transform handlesParent;
    [SerializeField] private Transform neck;
    [SerializeField] private XRGrabInteractable throttle;
    [SerializeField] private Transform throttleMax;
    [SerializeField] private float SteeringSensitivity = 1f;
    [SerializeField] private float ThrustSensitivity = 1f;

    private Vector3 _leftHandlePosition;
    private Vector3 _rightHandlePosition;
    private Vector3 _neckPosition;

    private const float NeckMinPush = 0.04f;
    private const float NeckMaxPull = 0;
    
    private const float NeckMinRotation = -45f;
    private const float NeckMaxRotation = 45f;
    
    private bool _leftGrabbed = false;
    private bool _rightGrabbed = false;
    private bool _throttleGrabbed = false;
    
    private Vector3 _rightGrabPosition;
    private Vector3 _leftGrabPosition;

    private Coroutine _leftHandStopper;
    private Coroutine _rightHandStopper;

    private Transform _throttleGrabber;
    private Vector3 _throttleGrabPosition;
    private Vector3 _throttleGrabbedPosition;
    private Vector3 _throttleMinPosition;
    private Vector3 _throttleMaxPosition;

    private Vector3 _leftHandRelativePosition;
    private Vector3 _rightHandRelativePosition;

    private float neckRoll;
    private float previousNeckRotation;
    
    private float neckYaw;
    private float previousNeckYaw;

    private void Start()
    {
        _throttleMinPosition = throttle.transform.localPosition;
        _throttleMaxPosition = throttleMax.localPosition;
        
        _leftHandlePosition = leftHandle.transform.localPosition;
        _rightHandlePosition = rightHandle.transform.localPosition;
        _neckPosition = neck.localPosition;

        throttle.selectEntered.AddListener((e) =>
        {
            if (_throttleGrabbed)
                return;

            _throttleGrabbed = true;
            _throttleGrabber = e.interactorObject.transform.parent;
            _throttleGrabPosition = _throttleGrabber == leftController
                ? _leftHandRelativePosition
                : _rightHandRelativePosition;
            _throttleGrabbedPosition = throttle.transform.localPosition;
        });
            
        throttle.selectExited.AddListener((e) =>
        {
            _throttleGrabbed = false;
        });
        
        leftHandle.selectEntered.AddListener((e) =>
        {
            if(_leftGrabbed)
                return;
            
            _leftGrabbed = true;
            _leftGrabPosition = _leftHandRelativePosition;
        });
        
        leftHandle.selectExited.AddListener((e) =>
        {
            _leftGrabbed = false;
        });
        
        rightHandle.selectEntered.AddListener((e) =>
        {
            if(_rightGrabbed)
                return;
            
            _rightGrabbed = true;
            _rightGrabPosition = _rightHandRelativePosition;
        });
        
        rightHandle.selectExited.AddListener((e) =>
        {
            _rightGrabbed = false;
        });
    }

    private IEnumerator OnLeftGrabExit()
    {
        for(int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();
        
        _leftGrabbed = false;
    }
    
    private IEnumerator OnRightGrabExit()
    {
        for(int i = 0; i < 5; i++)
            yield return new WaitForFixedUpdate();
        
        _rightGrabbed = false;
    }

    private float Angle(Vector2 from, Vector2 to)
    {
        Vector2 diff = (to - from).normalized;
        var angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void LateUpdate()
    {
        _leftHandRelativePosition = leftController.localPosition.ConvertVectorBetweenBases(xrOrigin, transform);
        _rightHandRelativePosition = rightController.localPosition.ConvertVectorBetweenBases(xrOrigin, transform);
        
        HandleThrottle();
        
        if (!_leftGrabbed || !_rightGrabbed)
        {
            neck.localPosition = _neckPosition;
            neck.localEulerAngles = Vector3.zero;
            handlesParent.localEulerAngles = Vector3.zero;
            neckRoll = 0;
            previousNeckRotation = 0;
            neckYaw = 0;
            previousNeckYaw = 0;
            return;
        }
        
        var leftDiff = _leftHandRelativePosition - _leftGrabPosition;
        var rightDiff = _rightHandRelativePosition - _rightGrabPosition;
        
        var neckPosition = _neckPosition.x + (leftDiff + rightDiff).x / 2 * ThrustSensitivity;
        if (neckPosition < NeckMaxPull)
            neckPosition = NeckMaxPull;
        
        if (neckPosition > NeckMinPush)
            neckPosition = NeckMinPush;

        var leftHandleRoll = new Vector2(_leftHandlePosition.y + leftDiff.y, _leftHandlePosition.z + leftDiff.z);
        var rightHandleRoll = new Vector2(_rightHandlePosition.y + rightDiff.y, _rightHandlePosition.z + rightDiff.z);
        neckRoll = Angle(leftHandleRoll, rightHandleRoll);
        
        neckRoll *= SteeringSensitivity;
        if (neckRoll < NeckMinRotation)
            neckRoll = NeckMinRotation;
        
        if (neckYaw < NeckMinRotation)
            neckYaw = NeckMinRotation;
        if (neckRoll > NeckMaxRotation)
            neckRoll = NeckMaxRotation;
        
        var leftHandleYaw = new Vector2(_leftHandlePosition.y + leftDiff.y, _leftHandlePosition.x + leftDiff.x);
        var rightHandleYaw = new Vector2(_rightHandlePosition.y + rightDiff.y, _rightHandlePosition.x + rightDiff.x);
        neckYaw = -Angle(leftHandleYaw, rightHandleYaw);
        neckYaw *= SteeringSensitivity;
        
        if (neckYaw > NeckMaxRotation)
            neckYaw = NeckMaxRotation;
            
        neck.localPosition = new Vector3(neckPosition, neck.localPosition.y, neck.localPosition.z);

        if (Math.Abs(neckRoll - previousNeckRotation) < NeckMaxRotation * 1.5f)
        {
            handlesParent.localEulerAngles = new Vector3(neckRoll, 0, 0);
            previousNeckRotation = neckRoll;
        }

        /*if (Math.Abs(neckYaw - previousNeckYaw) < NeckMaxRotation)
        {
            handlesParent.localEulerAngles = new Vector3(handlesParent.localEulerAngles.x, 0, neckYaw);
            previousNeckYaw = neckYaw;
        }*/
    }

    private void HandleThrottle()
    {
        if(!_throttleGrabbed)
            return;
        
        var diff = (_throttleGrabber == leftController ? _leftHandRelativePosition : _rightHandRelativePosition) - _throttleGrabPosition;
        var newPos = _throttleGrabbedPosition.x + diff.x * ThrustSensitivity;
        if(newPos > _throttleMaxPosition.x)
            newPos = _throttleMaxPosition.x;
        
        if(newPos < _throttleMinPosition.x)
            newPos = _throttleMinPosition.x;
        
        throttle.transform.localPosition = new Vector3(newPos, _throttleMinPosition.y,
            _throttleMinPosition.z);
    }

    public override float Roll()
    {
        return -neckRoll / -45f;
    }

    public override float Yaw()
    {
        return -neckYaw / -45f;
    }

    public override float Pitch()
    {
        return (neck.localPosition.x - _neckPosition.x) / -0.02f;
    }

    public override float Thrust()
    {
        return 1 - (_throttleMaxPosition.x - throttle.transform.localPosition.x) / (_throttleMaxPosition.x - _throttleMinPosition.x);
    }

    public override float Airbrake()
    {
        return 0;
    }

    public override bool Fire()
    {
        return rightInputs.activateAction.action.ReadValue<float>() > 0.5f;
    }
}
