using System;
using System.Collections;
using Leap;
using Leap.PhysicalHands;
using UnityEngine;

public class YokeInput : InputManager
{
    [SerializeField] private PhysicalHandsManager handsManager;
    [SerializeField] private LeapXRServiceProvider leapService;
    [SerializeField] private Transform leftHandle;
    [SerializeField] private Transform rightHandle;
    [SerializeField] private Transform neck;
    [SerializeField] private Transform throttle;
    [SerializeField] private Transform throttleMax;

    private Controller _leapController;
    
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

    private Hand _leftHand;
    private Hand _rightHand;

    private Vector3 _rightGrabPosition;
    private Vector3 _leftGrabPosition;

    private const float SteeringSensitivity = 0.6f;

    private Coroutine _leftHandStopper;
    private Coroutine _rightHandStopper;

    private Hand _throttleGrabber;
    private Vector3 _throttleGrabPosition;
    private Vector3 _throttleGrabbedPosition;
    private Vector3 _throttleMinPosition;
    private Vector3 _throttleMaxPosition;

    private Vector3 _leftHandRelativePosition;
    private Vector3 _rightHandRelativePosition;

    private float neckRotation;

    private Vector3 _previousShipPosition;
    
    private Frame _untransformedFrame;

    private void Start()
    {
        _leapController = leapService.GetLeapController();
        
        _throttleMinPosition = throttle.localPosition;
        _throttleMaxPosition = throttleMax.localPosition;
        
        _leftHandlePosition = leftHandle.localPosition;
        _rightHandlePosition = rightHandle.localPosition;
        _neckPosition = neck.localPosition;

        _leftHand = handsManager.LeftHand.DataHand;
        _rightHand = handsManager.RightHand.DataHand;

        handsManager.onGrab.AddListener((hand, rb) =>
        {
            if(hand.DataHand.GrabStrength < 0.75f)
                return;
            
            if (rb.transform == leftHandle)
            {
                if(_leftHandStopper != null)
                    StopCoroutine(_leftHandStopper);
                
                if (_leftGrabbed)
                    return;
                
                _leftGrabbed = true;
                _leftGrabPosition = _leftHandRelativePosition;
            }

            if (rb.transform == rightHandle)
            {
                if(_rightHandStopper != null)
                    StopCoroutine(_rightHandStopper);
                
                if (_rightGrabbed)
                    return;
                
                _rightGrabbed = true;
                _rightGrabPosition = _rightHandRelativePosition;
            }
            
            if (rb.transform == throttle)
            {
                if (_throttleGrabbed)
                    return;
                
                _throttleGrabbed = true;
                _throttleGrabber = hand.DataHand;
                _throttleGrabPosition = GetRelativeHandPosition(_throttleGrabber);
                _throttleGrabbedPosition = throttle.localPosition;
            }
        });

        handsManager.onGrabExit.AddListener((hand, rb) =>
        {
            if (rb.transform == leftHandle)
                _leftHandStopper = StartCoroutine(OnLeftGrabExit());
            
            if (rb.transform == rightHandle)
                _rightHandStopper = StartCoroutine(OnRightGrabExit());

            if (rb.transform == throttle)
                _throttleGrabbed = false;
        });

        _previousShipPosition = transform.position;
        StartCoroutine(PostPhysicsUpdate());
    }

    private IEnumerator PostPhysicsUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            var delta = transform.position - _previousShipPosition;
            _previousShipPosition = transform.position;

            //handsManager.LeftHand.palmBone.transform.position += delta / 2;
            //handsManager.RightHand.palmBone.transform.position += delta / 2;
            
            //Debug.Log(delta);
        }
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
        return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
    }

    private void LateUpdate()
    {
        leftHandle.localPosition = _leftHandlePosition;
        rightHandle.localPosition = _rightHandlePosition;
        
        leftHandle.localEulerAngles = Vector3.zero;
        rightHandle.localEulerAngles = Vector3.zero;
    }

    private void FixedUpdate()
    {
        _untransformedFrame = new Frame();
        _leapController.Frame(_untransformedFrame);
        
        if(_untransformedFrame.Hands.Count < 2)
            return;

        _leftHandRelativePosition = GetRelativeHandPosition(_leftHand);
        _rightHandRelativePosition = GetRelativeHandPosition(_rightHand);
        
        HandleThrottle();
        
        if (!_leftGrabbed || !_rightGrabbed)
        {
            neck.localPosition = _neckPosition;
            neck.localEulerAngles = Vector3.zero;
            neckRotation = 0;
            return;
        }
        
        var leftDiff = _leftHandRelativePosition - _leftGrabPosition;
        var rightDiff = _rightHandRelativePosition - _rightGrabPosition;
        
        var neckPosition = _neckPosition.x + (leftDiff + rightDiff).z / 2;
        if (neckPosition < NeckMaxPull)
            neckPosition = NeckMaxPull;
        
        if (neckPosition > NeckMinPush)
            neckPosition = NeckMinPush;
        
        var leftAngle = Angle(new Vector2(_leftHandlePosition.y + leftDiff.x, _leftHandlePosition.z + leftDiff.y), Vector2.zero) * SteeringSensitivity;
        var rightAngle = Angle(Vector2.zero, new Vector2(_rightHandlePosition.y + rightDiff.x, _rightHandlePosition.z + rightDiff.y)) * SteeringSensitivity;
        neckRotation = (leftAngle + rightAngle) / 2;
        if (neckRotation < NeckMinRotation)
            neckRotation = NeckMinRotation;
        
        if (neckRotation > NeckMaxRotation)
            neckRotation = NeckMaxRotation;
        
        neck.localPosition = new Vector3(neckPosition, neck.localPosition.y, neck.localPosition.z);
        neck.localEulerAngles = new Vector3(neckRotation, 0, 0);
    }

    private Vector3 GetRelativeHandPosition(Hand hand)
    {
        return Quaternion.Euler(-90f, 180f, 0f) * _untransformedFrame.GetHand(hand.GetChirality()).PalmPosition;
    }

    private void HandleThrottle()
    {
        if(!_throttleGrabbed)
            return;
        
        var diff = _leftHandRelativePosition - _throttleGrabPosition;
        var newPos = _throttleGrabbedPosition.x + diff.z;
        if(newPos > _throttleMaxPosition.x)
            newPos = _throttleMaxPosition.x;
        
        if(newPos < _throttleMinPosition.x)
            newPos = _throttleMinPosition.x;
        
        throttle.localPosition = new Vector3(newPos, _throttleMinPosition.y,
            _throttleMinPosition.z);
    }

    public override float Roll()
    {
        return 0;
    }

    public override float Yaw()
    {
        return neckRotation / -45f;
    }

    public override float Pitch()
    {
        return (neck.localPosition.x - _neckPosition.x) / -0.02f;
    }

    public override float Thrust()
    {
        return 1 - (_throttleMaxPosition.x - throttle.localPosition.x) / (_throttleMaxPosition.x - _throttleMinPosition.x);
    }

    public override float Airbrake()
    {
        return 0;
    }
}
