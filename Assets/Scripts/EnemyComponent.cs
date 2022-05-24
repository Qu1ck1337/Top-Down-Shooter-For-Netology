using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyComponent : UnitComponent
{
    [Space, SerializeField, Range(0f, 100f)]
    private float _playerIdentificationRadius;

    public float GetPlayerIdentificationRadius => _playerIdentificationRadius;

    [SerializeField, Range(0f, 100f)]
    private float _radiusOfEnemyView;

    [Space, SerializeField]
    private float _delayTimeAfterFire;

    [SerializeField, Space]
    private List<Vector3> _patrollingPoints = new List<Vector3>();
    private int _currentPartollingPointIndex;
    [SerializeField]
    private float _stayOnRadiusPatrollingPoint;
    [SerializeField]
    private float _delayStayingOnPoint;
    [SerializeField]
    private float _delayStayingOnPointOffset;

    [Space, SerializeField]
    private float _pursuitSpeed;

    [Space, SerializeField]
    private float _distanceToHandAttack = 0.2f;

    private Transform _target;
    private NavMeshAgent _agent;
    private ProjectilePool _projectilePool;
    private bool _inCooldown;

    public Enums.EnemyStateType StateType { get; private set; } = Enums.EnemyStateType.Idle;

    private void Start()
    {
        _handTrigger = GetComponentInChildren<SphereCollider>();

        if (_weapon != null)
        {
            _weapon = Instantiate(_weapon, transform);
            _weapon.transform.position = _weaponSpawn + transform.localPosition;
        }

        _agent = GetComponent<NavMeshAgent>();
        _target = FindObjectOfType<PlayerComponent>().gameObject.transform;
        _projectilePool = FindObjectOfType<ProjectilePool>();
        if (_patrollingPoints.Count > 0)
        {
            _patrollingPoints.Add(transform.position);
            StateType = Enums.EnemyStateType.Patrolling;
        }
        else
        {
            _patrollingPoints.Add(transform.position);
        }
    }

    private void Update()
    {
        if (_target == null) return;
        if (_inCooldown) return;
        TargetDetection();
        if (!_isMoving) return;
        UpdateStatus();
    }

    private void FixedUpdate()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        var velocity = _agent.velocity.normalized;
        Debug.Log(velocity);

        if (velocity.x < 0.5f && velocity.x > -0.5f && velocity.z < 0.5f && velocity.z > -0.5f) _animator.SetBool("IsMoving", false);
        else
        {
            _animator.SetBool("IsMoving", true);
            _animator.SetFloat("HorizontalMoving", velocity.x);
            _animator.SetFloat("VerticalMoving", velocity.z);
        }
    }

    private bool _isMovingOnPatrolling = true;
    private bool _isMoving = true;
    private void UpdateStatus()
    {
        switch (StateType)
        {
            case Enums.EnemyStateType.Idle:
                _agent.destination = transform.position;
                break;
            case Enums.EnemyStateType.Patrolling:
                if (!_isMovingOnPatrolling) return;
                _agent.speed = _movementSpeed;
                var ind = _currentPartollingPointIndex % _patrollingPoints.Count;
                _agent.destination = _patrollingPoints[ind];
                if (Vector3.Distance(new Vector3(_patrollingPoints[ind].x, transform.position.y, _patrollingPoints[ind].z), transform.position) < _stayOnRadiusPatrollingPoint)
                {
                    _isMovingOnPatrolling = false;
                    StartCoroutine(ChangePatrollingPoint());
                }
                break;
            case Enums.EnemyStateType.Pursuit:
                _agent.destination = _target.position;
                _agent.speed = _pursuitSpeed;
                FireLogic();
                break;
        }
    }

    private void FireLogic()
    {
        if (_weapon != null)
        {
            RaycastHit hit;
            Physics.Raycast(_weapon.transform.position, _weapon.transform.forward, out hit);
            if (Vector3.Distance(transform.position, _target.position) < _weapon.GetRadiusToFire() && hit.collider != null && hit.collider.GetComponent<PlayerComponent>() != null)
            {
                _weapon.checkAndFire();
                _isMoving = false;
                StartCoroutine(StopEnemyAfterFire());
            }
        }
        else
        {
            if (!_inAnimation && Vector3.Distance(transform.position, _target.transform.position) <= _distanceToHandAttack)
            {
                _animator.SetTrigger("HandAttack");
                _inAnimation = true;
            }
        }
    }

    private IEnumerator StopEnemyAfterFire()
    {
        _agent.destination = transform.position;
        yield return new WaitForSeconds(_delayTimeAfterFire);
        if (!(_weapon.CurrentAllAmmo <= 0 && _weapon.CurrentAmmoInStore <= 0))
            _isMoving = true;
    }

    private void TargetDetection()
    {
        if (Vector3.Distance(transform.position, _target.position) < _playerIdentificationRadius || Vector3.Distance(transform.position, _target.position) < _radiusOfEnemyView && IsAnyBulletsAround())
        {
            StateType = Enums.EnemyStateType.Pursuit;
        }
        else if (Vector3.Distance(transform.position, _target.position) >= _radiusOfEnemyView)
        {
            StateType = Enums.EnemyStateType.Patrolling;
        }
    }

    private IEnumerator ChangePatrollingPoint() 
    {
        yield return new WaitForSeconds(Mathf.Clamp(_delayStayingOnPoint + UnityEngine.Random.Range(-_delayStayingOnPointOffset, _delayStayingOnPointOffset), 0f, float.MaxValue));
        _currentPartollingPointIndex += 1;
        _isMovingOnPatrolling = true;
    }

    private bool IsAnyBulletsAround()
    {
        var projectile = _projectilePool.GetNearestProjectileInEnemyRadius(this);
        if (projectile != null && projectile.Owner is PlayerComponent)
        {
            return true;
        }
        return false;
    }

    public void SetEnemyCooldown(float seconds)
    {
        StartCoroutine(SetCooldown(seconds));
    }

    private IEnumerator SetCooldown(float seconds)
    {
        _inCooldown = true;
        yield return new WaitForSeconds(seconds);
        _inCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var point in _patrollingPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 1f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _playerIdentificationRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position, _radiusOfEnemyView);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_weaponSpawn + transform.position, 0.1f);
    }
}
