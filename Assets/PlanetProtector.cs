using System.Collections;
using System.Linq;
using UnityEngine;

public class PlanetProtector : AIShip
{
    [SerializeField] private Transform patrolPoints;
    [SerializeField] private float patrolPointDistanceRadius;
    [SerializeField] private float followPointDistanceRadius;
    [SerializeField] private Radar frontRadar;
    [SerializeField] private PlanetProtectorRadar radar;

    private Vector3[] _patrolPoints;
    private int _patrolPointIndex;
    
    private Coroutine _movementCoroutine;
    private Coroutine _attackingCoroutine;

    private bool _attack = false;

    protected override void Start()
    {
        base.Start();

        radar.onPlayerSpaceshipChanged += OnPlayerSpaceshipChanged;
    }

    private void OnPlayerSpaceshipChanged()
    {
        if (radar.playerSpaceship == null)
        {
            if (_attackingCoroutine != null)
                StopCoroutine(_attackingCoroutine);

            _attack = false;
            
            StartPatrolling();
            
            return;
        }
        
        Debug.Log("Detected player");
        
        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);

        _movementCoroutine = StartCoroutine(FollowPlayer());
        _attackingCoroutine = StartCoroutine(AttackPlayer());
    }

    public void SetDirection(Vector3 dir)
    {
        patrolPoints.forward = dir;

        _patrolPoints = new Vector3[patrolPoints.childCount];
        for (int i = 0; i < _patrolPoints.Length; i++)
            _patrolPoints[i] = patrolPoints.GetChild(i).position;
        
        StartPatrolling();
    }

    private void StartPatrolling()
    {
        if (_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);
        
        _patrolPointIndex = GetClosestPatrolPoint();
        SetRandomPatrolTargetPosition();

        _movementCoroutine = StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            yield return new WaitUntil(() => ReachedDestination);
            
            _patrolPointIndex++;
            if (_patrolPointIndex >= _patrolPoints.Length)
                _patrolPointIndex = 0;
            
            SetRandomPatrolTargetPosition();
        }
    }
    
    private IEnumerator FollowPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            
            var playerPosition = radar.playerSpaceship.transform.position;
            var targetPosition = playerPosition + Random.Range(-followPointDistanceRadius, followPointDistanceRadius) * Random.onUnitSphere;
            
            SetTargetPosition(targetPosition);
        }
    }
    
    private IEnumerator AttackPlayer()
    {
        while (true)
        {
            _attack = false;
            
            yield return new WaitForSeconds(3f);

            _attack = frontRadar.spaceships.Count > 0;
            
            yield return new WaitForSeconds(1f);
        }
    }

    private int GetClosestPatrolPoint()
    {
        var minDist = float.MaxValue;
        int closest = -1;

        for (int i = 0; i < _patrolPoints.Length; i++)
        {
            var dist = Vector3.Distance(transform.position, _patrolPoints[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    private void SetRandomPatrolTargetPosition()
    {
        var pos = _patrolPoints[_patrolPointIndex];
        pos += Random.Range(-patrolPointDistanceRadius, patrolPointDistanceRadius) * Random.onUnitSphere;
        SetTargetPosition(pos);
    }

    public override bool Fire()
    {
        return _attack;
    }
}
