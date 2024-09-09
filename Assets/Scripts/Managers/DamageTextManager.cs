using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private DamageText damageTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [NaughtyAttributes.Button]
    private void InstantiateDamageText()
    {
        Vector3 spawnPosition = Random.insideUnitCircle * Random.Range(1f, 5f);
        DamageText _damageText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, transform);

        _damageText.PlayAnimation();
    }
}

