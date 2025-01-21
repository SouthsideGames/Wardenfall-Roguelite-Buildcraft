using System.Collections;
using UnityEngine;

public class MoltenTrail : MonoBehaviour
{
    
    [Tooltip("Detection layer for enemies.")]
    [SerializeField] private LayerMask enemyMask;

    private int damage;
    private float duration;

    private void Start()
    {
        Destroy(gameObject, duration);
        StartCoroutine(SpawnTrail());
    }

    public void Initialize(CardSO _card)
    {
        damage = _card.EffectValue;
        duration = _card.ActiveTime;
    }

    private IEnumerator SpawnTrail()
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            GameObject fireZone = new GameObject("FireZone");
            fireZone.transform.position = CharacterManager.Instance.transform.position;

            FireZone fireZoneScript = fireZone.AddComponent<FireZone>();
            fireZoneScript.Initialize(damage, enemyMask);

            yield return new WaitForSeconds(0.5f); 
        }
    }
}
