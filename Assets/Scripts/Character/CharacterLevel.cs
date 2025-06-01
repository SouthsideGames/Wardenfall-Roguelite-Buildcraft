using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterLevel : MonoBehaviour
{   
    [Header("ELEMENTS:")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("SETTINGS:")]
    private int requiredXp;
    private int currentXp;
    private int level = 1;
    private int levelsEarned;

    [Header("DEBUG:")]
    [SerializeField] private bool debug;

    private void Awake() => Meat.OnCollected += MeatCollectedCallback;
    private void OnDestroy() => Meat.OnCollected -= MeatCollectedCallback;

    void Start()
    {
        UpdateRequiredXP();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        xpBar.value = (float)currentXp / requiredXp;
        levelText.text = "lvl " + level;
    }

    private void UpdateRequiredXP() => requiredXp = level * 5;
    private void MeatCollectedCallback(Meat _meat)
    {
        currentXp++;

        if(currentXp >= requiredXp)
          LevelUp();

        UpdateVisuals();
    }

    private void LevelUp()
    {
        StatisticsManager.Instance.TotalLevelUpsInARun();
        level++;
        currentXp = 0;
        levelsEarned++;
        UpdateRequiredXP();

        CrowdReactionType reaction = UnityEngine.Random.value < 0.7f
            ? CrowdReactionType.Boo
            : CrowdReactionType.Cheer;

        AudioManager.Instance?.PlayCrowdReaction(reaction);

        if (level % 5 == 0)
            CardDraftManager.Instance.ShowMiniDraft();
        
    }

    public bool HasLeveledUp()
    {
        if(debug)
          return true;
  
        if(levelsEarned > 0)
        {
            levelsEarned--;
            return true;
        }

        return false;   
    }
}
