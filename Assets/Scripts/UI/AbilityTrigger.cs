using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button abilityButton;
    [SerializeField] private CanvasGroup cooldownAlphaGroup;
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
        cooldownText.gameObject.SetActive(false);

        if (cooldownAlphaGroup != null)
            cooldownAlphaGroup.alpha = 0f;
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
        AudioManager.Instance.PlayCrowdReaction(CrowdReactionType.Cheer);
        characterAbility.TryUseAbility();
        StartCooldown(characterAbility.GetCooldownTime());
    }

   private void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownRemaining = duration;
        isCoolingDown = true;

        cooldownText.gameObject.SetActive(true);
        abilityButton.interactable = false;

        if (cooldownAlphaGroup != null)
            cooldownAlphaGroup.alpha = 1f;
    }

    private void EndCooldown()
    {
        isCoolingDown = false;
        cooldownText.gameObject.SetActive(false);
        abilityButton.interactable = true;

        if (cooldownAlphaGroup != null)
            cooldownAlphaGroup.alpha = 0f;
    }
}
