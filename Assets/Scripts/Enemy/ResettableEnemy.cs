using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ResettableEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private bool _wasInitiallyActive;

    private EnemyBehavior _enemyBehavior;
    private EnemyBehavior.BehaviorState _initialAIState;

    private NavMeshAgent _navAgent;
    private CapsuleCollider _collider;
    private Animator _anim;

    private void Awake()
    {
        if (!_enemy) _enemy = gameObject;

        _wasInitiallyActive = _enemy.activeSelf;

        if (_wasInitiallyActive)
        {
            Cache();
        }
        else
        {
            _enemy.SetActive(true);
            Cache();
            _enemy.SetActive(false);
        }
    }

    private void Cache()
    {
        _initialPosition = _enemy.transform.position;
        _initialRotation = _enemy.transform.rotation;

        _enemyBehavior = _enemy.GetComponent<EnemyBehavior>();
        if (_enemyBehavior != null)
        {
            _initialAIState = _enemyBehavior.currentState;
        }

        _navAgent = _enemy.GetComponent<NavMeshAgent>();
        _collider = _enemy.GetComponent<CapsuleCollider>();
        _anim = _enemy.GetComponentInChildren<Animator>();
    }

    public void ResetEnemy()
    {
        _enemy.SetActive(_wasInitiallyActive);
        _enemy.transform.position = _initialPosition;
        _enemy.transform.rotation = _initialRotation;

        if (_enemyBehavior)
        {
            _enemyBehavior.currentState = _initialAIState;
            _enemyBehavior.health?.ResetUnitsHealth();
            _enemyBehavior.StartCoroutine(DelayedInitializeAfterSpawn());
        }

        if (_navAgent)
        {
            _navAgent.enabled = true;
            _navAgent.ResetPath();
            _navAgent.isStopped = false;
        }

        if (_collider) _collider.enabled = true;

        if (_anim)
        {
            _anim.Rebind();
            _anim.Update(0f);
        }
    }

    private IEnumerator DelayedInitializeAfterSpawn()
    {
        yield return null;
        _enemyBehavior.InitializeAfterSpawn();
    }
}
