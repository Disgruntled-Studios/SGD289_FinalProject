using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FPSEnemyController : MonoBehaviour
{
    [Header("Materials")] 
    [SerializeField] private Material _redMat;
    [SerializeField] private Material _blueMat;
    [SerializeField] private Material _greenMat;
    
    [Header("References")]
    [SerializeField] private SkinnedMeshRenderer _skm;

    private Material[] _materials;
    private Material _currentMat;
    public Material CurrentMat => _currentMat;
    private EnemyType _type;
    private int _pointValue;

    private FPSManager _manager;
    
    public void Initialize(FPSManager manager, EnemyType type)
    {
        _manager = manager;
        _type = type;

        switch (_type)
        {
            case EnemyType.OneColor:
                _pointValue = 10;
                _materials = new[] { _greenMat, _blueMat, _redMat };
                ChangeColor(); // Just once
                break;
            case EnemyType.TwoColor:
                _pointValue = 20;
                var allMats = new[] { _greenMat, _blueMat, _redMat };
                var firstIndex = Random.Range(0, allMats.Length);

                int secondIndex;
                do
                {
                    secondIndex = Random.Range(0, allMats.Length);
                } while (secondIndex == firstIndex);

                _materials = new[] { allMats[firstIndex], allMats[secondIndex] };
                StartCoroutine(ChangeColorRoutine());
                break;
            case EnemyType.ThreeColor:
                _pointValue = 30;
                _materials = new[] { _greenMat, _redMat, _blueMat };
                StartCoroutine(ChangeColorRoutine());
                break;
        }
        
        FacePlayer();
    }

    private void FacePlayer()
    {
        var player = GameManager.Instance.Player.transform;
        var direction = player.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("L3Bullet"))
        {
            var bulletController = other.gameObject.GetComponent<FPSBulletController>();
            if (bulletController.CurrentMat == _currentMat)
            {
                FPSManager.Instance?.RegisterHit(_pointValue);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Wrong color");
            }
        }
    }

    private void ChangeColor()
    {
        var index = Random.Range(0, _materials.Length);
        _currentMat = _materials[index];

        var mats = _skm.materials;
        mats[0] = _currentMat;
        _skm.materials = mats;
    }
    
    private IEnumerator ChangeColorRoutine()
    {
        ChangeColor();
        yield return new WaitForSeconds(Random.Range(2f, 5f));
    }
}
