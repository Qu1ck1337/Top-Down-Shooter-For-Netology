using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyComponent : UnitComponent
{
    [Space, SerializeField, Header("--- Конфигуратор выбора оружия ---")]
    private WeaponRangePool _weaponPoolConfigurator;

    [Space, SerializeField, Range(0f, 100f), Header("--- Аттрибуты замечания игрока ---")]
    private float _playerIdentificationRadius;
    [SerializeField]
    private float _timeForPlayerPursuitInIdentificationRadius = 2f;
    public float PlayerIdentificationRadius => _playerIdentificationRadius;
    [SerializeField, Range(0f, 100f)]
    private float _radiusOfEnemyView;

    [Space, SerializeField, Header("--- Свойства врага ---")]
    private float _delayTimeAfterFire;
    [SerializeField]
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
    [SerializeField]
    private float _rotateSpeed;

    [Space, SerializeField, Range(0.1f, 10f)]
    private float _distanceToAttack = 1f;

    [Space, SerializeField]
    private bool _canPickUpWeapon;

    [Space, SerializeField, Header("--- Отладка ---")]
    private bool _showGizmos;

    private Transform _target;
    private NavMeshAgent _agent;
    private ProjectilePool _projectilePool;
    private bool _isTargetInIdentificationRadius;
    private bool _inCooldownByShoting;
    private List<EnemyComponent> _nearestEnemies = new List<EnemyComponent>();

    public Enums.EnemyStateType StateType { get; private set; } = Enums.EnemyStateType.Idle;

    protected override void Awake()
    {
        base.Awake();

        _animator.SetBool("IsBot", true);
        _handTrigger = GetComponentInChildren<SphereCollider>();

        if (_weapon != null)
        {
            SpawnWeapon(_weapon);
            _distanceToAttack += _weapon.GetRadiusToFire;
        }
        else if (_weaponPoolConfigurator != null && _weaponPoolConfigurator.Count != 0)
        {
            SpawnWeapon(_weaponPoolConfigurator.GetRandomWeapon());
            _distanceToAttack += _weapon.GetRadiusToFire;
        }

        _agent = GetComponent<NavMeshAgent>();

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

    private void Start()
    {
        _target = GameManager.Self.Player.transform;
        _projectilePool = GameManager.Self.ProjectilePool;
    }

    private void Update()
    {
        //        Определяем угол между целью и таргетом черезе Vector3.SignedAngle относительно оси Y
        //ЕЕсли угол больше / меньше 3 - 5 градусов
        //transform.RotateAround(0f, _rotateSpeed * Time.deltaTime, 0f, transform.up);

        //IK animation для держания оружия
        _nearestEnemies.Clear();
        _nearestEnemies.AddRange(GameManager.Self.GetNearestEnemies(this));
        if (_agent == null || _target == null) return;
        UpdateStatus();
        if (_inCooldownByShoting || _agent.isStopped) return;
        TargetDetection();
        AttackPlayerLogic();
    }

    private void FixedUpdate()
    {
        if (_agent == null) return;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        var velocity = _agent.velocity.normalized;

        if (velocity.magnitude < 0.1f) _animator.SetBool("IsMoving", false);
        else
        {
            _animator.SetBool("IsMoving", true);
        }
    }

    private void UpdateStatus()
    {
        switch (StateType)
        {
            case Enums.EnemyStateType.Idle:
                _agent.destination = transform.position;
                break;
            case Enums.EnemyStateType.Patrolling:
                _agent.speed = _movementSpeed;
                var ind = _currentPartollingPointIndex % _patrollingPoints.Count;
                _agent.destination = _patrollingPoints[ind];
                if (Vector3.Distance(new Vector3(_patrollingPoints[ind].x, transform.position.y, _patrollingPoints[ind].z), transform.position) < _stayOnRadiusPatrollingPoint)
                {
                    StartCoroutine(ChangePatrollingPoint());
                }
                break;
            case Enums.EnemyStateType.Pursuit:
                _agent.destination = _target.position;
                _agent.speed = _pursuitSpeed;
                break;
            case Enums.EnemyStateType.Shoot:
                _weapon.checkAndFire();
                StartCoroutine(StopUnitByShooting(_delayTimeAfterFire));
                break;
            case Enums.EnemyStateType.Punch:
                if (!_lockAttack) 
                {
                    _animator.SetTrigger("HandAttack");
                    _audioSource.PlayOneShot(_punchSound);
                    _lockAttack = true;
                }
                break;
            case Enums.EnemyStateType.DropWeapon:
                if (_weapon != null)
                {
                    _distanceToAttack -= _weapon.GetRadiusToFire;
                    DropWeapon();
                }
                break;
        }
    }

    private IEnumerator ChangePatrollingPoint() 
    {
        StateType = Enums.EnemyStateType.Idle;
        yield return new WaitForSeconds(Mathf.Clamp(_delayStayingOnPoint + UnityEngine.Random.Range(-_delayStayingOnPointOffset, _delayStayingOnPointOffset), 0f, float.MaxValue));
        _currentPartollingPointIndex += 1;
        StateType = Enums.EnemyStateType.Patrolling;
    }

    private void AttackPlayerLogic()
    {
        if (_weapon != null && _weapon.CurrentAllAmmo <= 0 && _weapon.CurrentAmmoInStore <= 0)
        {
            StateType = Enums.EnemyStateType.DropWeapon;
            return;
        }

        if (StateType != Enums.EnemyStateType.Pursuit) return;

        if (Vector3.Distance(transform.position, _target.position) < _distanceToAttack)
        {
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit);
            if (hit.collider != null && hit.collider.GetComponent<PlayerComponent>() != null)
            {
                if (_weapon != null)
                {
                    StateType = Enums.EnemyStateType.Shoot;
                }
                else
                {
                    StateType = Enums.EnemyStateType.Punch;
                }
            }
            else
            {
                float angle = Vector3.SignedAngle((_target.position - transform.position), transform.forward, Vector3.up);
                if (angle < -5.0f)
                    transform.RotateAround(transform.position, Vector3.up, _rotateSpeed * Time.deltaTime);
                else if (angle > 5.0f)
                    transform.RotateAround(transform.position, Vector3.down, _rotateSpeed * Time.deltaTime);
            }
        }
    }

    public bool EnemyOnPursuit;
    private void TargetDetection()
    {
        var distance = Vector3.Distance(transform.position, _target.position);
        if (distance < _radiusOfEnemyView)
        {
            if (EnemyOnPursuit)
            {
                StateType = Enums.EnemyStateType.Pursuit;
                return;
            }

            //todo [для оптимизации] кидать луч, если игрок в зоне палева (посмотреть варианты)
            Physics.Raycast(transform.position, (_target.position - transform.position), out RaycastHit hit);
            if (hit.collider != null && hit.collider.GetComponent<PlayerComponent>())
            {
                //будет продолжать патрулированеие, если игрок зайдёт за стену
                //if (EnemyOnPursuit)
                //{
                //    StateType = Enums.EnemyStateType.Pursuit;
                //    return;
                //}

                if (IsAnyBulletsAround())
                {
                    StateType = Enums.EnemyStateType.Pursuit;
                    EnemyOnPursuit = true;
                    SetAllNearestEnemiesPursuitState();
                }

                if (distance < _playerIdentificationRadius)
                {
                    if (!_isTargetInIdentificationRadius && StateType != Enums.EnemyStateType.Pursuit)
                    {
                        _isTargetInIdentificationRadius = true;
                        StartCoroutine(WaitForTargetStayingInIdentificationRadius());
                    }
                }
                else
                {
                    _isTargetInIdentificationRadius = false;
                }
            }
            //будет продолжать патрулированеие, если игрок зайдёт за стену
            //else if (EnemyOnPursuit)
            //{
            //    StateType = Enums.EnemyStateType.Patrolling;
            //    EnemyOnPursuit = false;
            //}
        }
        else
        {
            //можно как-то упростить Xb
            if (EnemyOnPursuit)
            {
                var ind = _currentPartollingPointIndex % _patrollingPoints.Count;
                if (Vector3.Distance(new Vector3(_patrollingPoints[ind].x, transform.position.y, _patrollingPoints[ind].z), transform.position) >= _stayOnRadiusPatrollingPoint)
                {
                    StateType = Enums.EnemyStateType.Patrolling;
                }
                else
                {
                    StateType = Enums.EnemyStateType.Idle;
                }
                EnemyOnPursuit = false;
            }
        }
    }

    private void SetAllNearestEnemiesPursuitState()
    {
        foreach (EnemyComponent enemy in _nearestEnemies)
        {
            Physics.Raycast(transform.position, (enemy.transform.position - transform.position), out RaycastHit hit);
            if (hit.collider != null && hit.collider.GetComponent<EnemyComponent>())
                enemy.EnemyOnPursuit = true;
        }
    }

    //todo если игрок в зоне видимости, то бот должен бросить луч и проверить есть ли стена

    //todo довернуться до игрока, а не ждать луча

    private IEnumerator StopUnitByShooting(float seconds)
    {
        _inCooldownByShoting = true;
        StateType = Enums.EnemyStateType.Idle;
        yield return new WaitForSeconds(seconds);
        _inCooldownByShoting = false;
    }

    private IEnumerator StopUnit(float seconds)
    {
        if (_agent != null)
            _agent.isStopped = true;
        StateType = Enums.EnemyStateType.Idle;
        yield return new WaitForSeconds(seconds);
        if (_agent != null)
            _agent.isStopped = false;
    }

    public void SetEnemyCooldown(float seconds)
    {
        StartCoroutine(StopUnit(seconds));
    }

    private IEnumerator WaitForTargetStayingInIdentificationRadius()
    {
        yield return new WaitForSeconds(_timeForPlayerPursuitInIdentificationRadius);
        if (Vector3.Distance(transform.position, _target.position) < _playerIdentificationRadius)
        {
            Physics.Raycast(transform.position, (_target.position - transform.position), out RaycastHit hit);
            if (hit.collider != null && hit.collider.GetComponent<PlayerComponent>() != null)
            {
                StateType = Enums.EnemyStateType.Pursuit;
                EnemyOnPursuit = true;
                SetAllNearestEnemiesPursuitState();
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        foreach (var point in _patrollingPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 1f);
        }

        if (!_showGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerIdentificationRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, _radiusOfEnemyView);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_weaponSpawn + transform.position, 0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_canPickUpWeapon || _weapon != null) return;
        SimpleWeapon weaponComponent = collision.gameObject.GetComponent<SimpleWeapon>();
        if (!PickUpWeapon(weaponComponent)) return;
        _distanceToAttack += _weapon.GetRadiusToFire;
    }

    protected override void Dead()
    {
        base.Dead();
        if (_agent != null)
            Destroy(_agent);
    }
}
