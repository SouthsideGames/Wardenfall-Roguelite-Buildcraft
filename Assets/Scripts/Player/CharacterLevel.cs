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

    private void Awake()
    {
        Candy.onCollected += CandyCollectedCallback;
    }

    private void OnDestroy() 
    {
        Candy.onCollected -= CandyCollectedCallback;
    }


    // Start is called before the first frame update
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
    private void CandyCollectedCallback(Candy _candy)
    {
        currentXp++;

        if(currentXp >= requiredXp)
          LevelUp();

        UpdateVisuals();
    }

    private void LevelUp()
    {
        level++;
        currentXp = 0;
        levelsEarned++;
        UpdateRequiredXP();
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
