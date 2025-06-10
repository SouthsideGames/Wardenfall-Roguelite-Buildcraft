using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image overlayImage;
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Header("Cooldown Settings")]
    private float cooldownDuration = 7f;
    private float cooldownRemaining = 0f;
    private bool isCoolingDown = false;

    private CharacterAbility characterAbility;

    private void Start()
    {
        characterAbility = CharacterManager.Instance.ability;
        abilityButton.onClick.AddListener(OnPressed);
        overlayImage.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isCoolingDown)
        {
            cooldownRemaining -= Time.deltaTime;
            if (cooldownRemaining <= 0f)
            {
                EndCooldown();
            }
            else
            {
                cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString();
            }
        }
    }

    private void OnPressed()
    {
        if (isCoolingDown || characterAbility == null) return;

        characterAbility.TryUseAbility();
        StartCooldown(characterAbility.GetCooldownTime());
    }

    private void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownRemaining = duration;
        isCoolingDown = true;

        overlayImage.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);
        abilityButton.interactable = false;
    }

    private void EndCooldown()
    {
        isCoolingDown = false;
        overlayImage.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
        abilityButton.interactable = true;
    }
}
