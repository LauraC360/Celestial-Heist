using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShip : InputManager
{
    [SerializeField] private float minDistanceThreshold = 50f;
    [SerializeField] private List<RacingPoint> racingPoints;
    private int _currentCheckpointIndex = 0;

    private Vector3 _targetPosition;
    private bool _reachedDestination = false;

    private float _thrust = 6f;
    private float _roll = 0;
    private float _yaw = 0;
    private float _pitch = 0;
    private const float RotationSpeed = 4.0f;

    protected virtual void Start()
    {
        if (racingPoints.Count > 0)
        {
            _targetPosition = racingPoints[_currentCheckpointIndex].transform.position;
        }
        StartCoroutine(Movement());
    }

    private void UpdateTarget()
    {
        _currentCheckpointIndex++;
        if (_currentCheckpointIndex >= racingPoints.Count)
        {
            _currentCheckpointIndex = 1;
        }
        _targetPosition = racingPoints[_currentCheckpointIndex].transform.position;
        _reachedDestination = false;
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 diff = _targetPosition - transform.position;
            _thrust = 0.65f;

            Quaternion targetRotation = Quaternion.LookRotation(diff, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed);

            Vector3 localDiff = transform.InverseTransformDirection(diff);
            _yaw = Mathf.Clamp(localDiff.x * 2f, -1f, 1f);

            if (diff.magnitude < minDistanceThreshold)
            {
                UpdateTarget();
            }
        }
    }

    public override float Roll() => _roll;
    public override float Yaw() => _yaw;
    public override float Pitch() => _pitch;
    public override float Thrust() => _thrust;
    public override float Airbrake() => 0;
    public override bool Fire() => false;
}
