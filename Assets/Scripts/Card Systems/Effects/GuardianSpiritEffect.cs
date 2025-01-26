using System.Collections;
using UnityEngine;

public class GuardianSpiritEffect : ICardEffect
{
    private GameObject spiritPrefab;
    private GameObject activeSpirit;
    private Transform playerTransform;
    private float damageAbsorptionPercentage;
    private float activeTime;

    public GuardianSpiritEffect(GameObject _spiritPrefab, Transform _playerTransform, float _damageAbsorptionPercentage)
    {
        spiritPrefab = _spiritPrefab;
        playerTransform = _playerTransform;
        damageAbsorptionPercentage = _damageAbsorptionPercentage;
    }

    public void Activate(float duration)
    {
        activeTime = duration;

        activeSpirit = Object.Instantiate(spiritPrefab, playerTransform.position, Quaternion.identity);
        activeSpirit.transform.SetParent(playerTransform);

        CharacterHealth characterHealth = CharacterManager.Instance.health;
        characterHealth.SetDamageAbsorption((int)damageAbsorptionPercentage);

        CoroutineRunner.Instance.StartCoroutine(DisableAfterDuration(characterHealth));
    }

    public void Disable()
    {
        if (activeSpirit != null)
        {
            Object.Destroy(activeSpirit);
        }

        CharacterHealth characterHealth = CharacterManager.Instance.health;
        characterHealth.SetDamageAbsorption(0); 
    }

    private IEnumerator DisableAfterDuration(CharacterHealth characterHealth)
    {
        yield return new WaitForSeconds(activeTime);
        Disable();
    }
}
