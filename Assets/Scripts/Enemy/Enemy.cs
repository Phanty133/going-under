using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyType
{
    SUB,
    PATROL,
    DESTROYER
}

public enum EnemyStates
{
    PATROL,
    PURSUIT
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyType type;
    public GameObject radioManagerObject;
    private RadioManager _radioManager => radioManagerObject.GetComponent<RadioManager>();
    public float visibilityRange;
    public float speed;
    
    private Vector2? _lastPlayerPos = null;
    public float reportCooldown = 3f;
    private float _reportTimes = 0f;
    private GameObject _player;
    private Vector2? _destination = null;
    private EnemyStates _state = EnemyStates.PATROL;
    private float _searchCooldown = 0f;
    public float searchCooldown = 10f;
    public int squad = -1;
    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();
    public GameObject torpedoBase;
    private GameObject _sonarManagerObj;
    private SonarManager _sonarManager => _sonarManagerObj.GetComponent<SonarManager>();
    private float _fireCooldown = 0f;
    public float fireCooldown = 10f;
    public float health = 100f;

    public void DamageEnemy(float damage)
    {
        health -= damage;
        if (health <= 0f) Destroy(gameObject);
    }

    void CheckPlayerSight()
    {
        float dist = Vector2.Distance(transform.position, _player.transform.position);
        if (dist > visibilityRange) return;
        // Console.WriteLine("In range!");
        if (Physics2D.RaycastAll(transform.position, _player.transform.position, dist,
                LayerMask.GetMask("Terrain", "Enemy")).Length > 1) return; // If we hit anything other than ourselves
        // We can see it!
        _lastPlayerPos = _player.transform.position;
        StartPursuit();
        if(_fireCooldown <= 0f) ShootTorpedoAt(_player.transform.position);
        if (_reportTimes > 0f) return;
        if (_lastPlayerPos == null) throw new Exception("Player Position null!");
        _radioManager.ReportPlayerPos((Vector2)_lastPlayerPos);
        _reportTimes = reportCooldown;
    }

    void ShootTorpedoAt(Vector2 direction)
    {
        GameObject torpedo = Instantiate(torpedoBase, transform.position, new Quaternion());
        torpedo.transform.LookAt(new Vector3(direction.x, direction.y, torpedo.transform.position.z));
        _sonarManager.CreateSonarTorpedo(torpedo, true);
        _fireCooldown = fireCooldown;
    }

    void AlertStateChange(AlertState state)
    {
        // Change dest
    }

    private void StartPursuit()
    {
        _searchCooldown = searchCooldown;
        _state = EnemyStates.PURSUIT;
    }

    private void StartPatrol()
    {
        _state = EnemyStates.PATROL;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        radioManagerObject = GameObject.FindGameObjectWithTag("RadioManager");
        _sonarManagerObj = GameObject.FindGameObjectWithTag("SonarManager");

        _radioManager.onAlertStateChange += AlertStateChange;
    }

    private void Update()
    {
        CheckPlayerSight();

        if (_reportTimes > 0f) _reportTimes = Mathf.Max(_reportTimes - Time.deltaTime, 0f);
        
        if (_destination == null || Vector2.Distance(transform.position, (Vector2)_destination) < 0.5f)
            _destination = _radioManager.RequestNewPosition(gameObject);
        if (_state == EnemyStates.PATROL) _agent.SetDestination((Vector3)_destination);
        if (_state == EnemyStates.PURSUIT) 
            _agent.SetDestination((Vector3)_lastPlayerPos 
                                  + new Vector3(MathUtils.RandomNegative() * Random.Range(visibilityRange * 0.25f, visibilityRange), 
                                      MathUtils.RandomNegative() * Random.Range(visibilityRange * 0.25f, visibilityRange)));

        if (_searchCooldown > 0f)
        {
            _searchCooldown -= Time.deltaTime;
            if (_searchCooldown <= 0f) StartPatrol();
        }
        
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }
        
        _agent.speed = speed;
    }
    
    void OnDestroy () {
        _radioManager.DeleteEnemy(gameObject);
    }
}