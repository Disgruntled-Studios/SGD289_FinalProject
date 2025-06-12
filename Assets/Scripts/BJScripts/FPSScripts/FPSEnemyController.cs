using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FPSEnemyController : MonoBehaviour
{
    [SerializeField] private Material[] _materials;
    
    private UnitHealth _health;

    private MeshRenderer _mr;
    
    public Material CurrentMaterial { get; set; }

    private void Awake()
    {
        _mr = GetComponent<MeshRenderer>();
    }
    
    private void Start()
    {
        _health = new UnitHealth(10);
        ChangeColor();
        StartCoroutine(ChangeColorRoutine());
    }

    public void TakeDamage(float amount)
    {
        _health.Damage(amount);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("L3Bullet"))
        {
            if (other.gameObject.GetComponent<FPSBulletController>().CurrentMat == CurrentMaterial)
            {
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
        CurrentMaterial = _materials[index];
        _mr.material = CurrentMaterial;
    }
    
    private IEnumerator ChangeColorRoutine()
    {
        while (!_health.IsDead)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            ChangeColor();
        }
    }
}
