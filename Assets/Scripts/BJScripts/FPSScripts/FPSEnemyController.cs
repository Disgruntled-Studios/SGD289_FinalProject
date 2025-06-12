using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FPSEnemyController : MonoBehaviour
{
    [SerializeField] private Material[] _materials;
    [SerializeField] private SkinnedMeshRenderer _skm;
    
    public Material CurrentMaterial { get; set; }
    
    public void Initialize()
    {
        ChangeColor();
        StartCoroutine(ChangeColorRoutine());
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

        var mats = _skm.materials;
        mats[0] = CurrentMaterial;
        _skm.materials = mats;
    }
    
    private IEnumerator ChangeColorRoutine()
    {
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        ChangeColor();
    }
}
