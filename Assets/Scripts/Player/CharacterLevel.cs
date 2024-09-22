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
        UpdateRequiredXP();
    }
}
