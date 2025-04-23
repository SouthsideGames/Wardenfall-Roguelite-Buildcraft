using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour, ICollectable
{
    protected bool collected;

    private void OnEnable()
    {
        collected = false;
    }

    public void Collect(CharacterManager _character)
    {
        if(collected)
           return;

        collected = true;

        if (_character.cards.HasCard("vital_bloom"))
        {
            float healAmount = _character.health.maxHealth * 0.03f;
            _character.health.Heal((int)healAmount);
        }

        StartCoroutine(MoveTowardsPlayer(_character));
    }

    IEnumerator MoveTowardsPlayer(CharacterManager _character)   
    {
        float timer = 0;
        Vector2 initialPosition = transform.position; 

        while(timer < 1)
        {
            Vector2 targetPosition = _character.GetColliderCenter();

            transform.position = Vector2.Lerp(initialPosition, targetPosition, timer);
            
            timer += Time.deltaTime; 
            yield return null;
        }

        Collected();
    }

    protected abstract void Collected();
}
