using UnityEngine;
using SouthsideGames.SaveManager;
using UnityEngine.UI;
using TMPro;

public class CharacterExperience : MonoBehaviour, IWantToBeSaved
{
    [Header("ELEMENTS:")]
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;
    [SerializeField] private int maxLevel = 15;
    [SerializeField] private int baseExperienceRequired = 100;
    [SerializeField] private float experienceGrowthRate = 1.5f;
    [SerializeField] private float statGrowthMultiplier = 0.05f;


    private CharacterStats characterStats;

    [Header("UI:")]
    [SerializeField] private Image experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private WaveManager waveManager;

    public int Level => level;
    public int Experience => experience;
    public int MaxLevel => maxLevel;
    public bool IsMaxLevel => level >= maxLevel;

    private void Awake()
    {
        characterStats = CharacterStats.Instance;
        CharacterSelectionManager.OnCharacterSelected += UpdateCharacterDetails;
        WaveManager.OnWaveCompleted += GainWaveCompletionExperience;
        WaveManager.OnSurvivalCompleted += GainSurvivalCompletionExperience;
    }

    private void Start()
    {
        Load(); 
    }

    private void OnDestroy()
    {
        CharacterSelectionManager.OnCharacterSelected -= UpdateCharacterDetails;
        WaveManager.OnWaveCompleted -= GainWaveCompletionExperience;
        WaveManager.OnSurvivalCompleted -= GainSurvivalCompletionExperience;
    }

    public void GainExperience(int amount)
    {
        if (IsMaxLevel)
            return;

        experience += amount;

        while (experience >= ExperienceToLevelUp() && level < maxLevel)
        {
            experience -= ExperienceToLevelUp();
            LevelUp();
        }

        if (IsMaxLevel)
            experience = ExperienceToLevelUp();

        UpdateUI();
        Save(); 
    }

    private int ExperienceToLevelUp()
    {
        return Mathf.FloorToInt(baseExperienceRequired * Mathf.Pow(experienceGrowthRate, level - 1));
    }

    private void LevelUp()
    {
        if (level >= maxLevel)
            return;

        level++;
        ApplyStatGrowth();

        Debug.Log($"Leveled up to {level}!");
    }

    private void ApplyStatGrowth()
    {
        foreach (Stat stat in characterStats.CharacterData.BaseStats.Keys)
        {
            float growth = characterStats.CharacterData.BaseStats[stat] * statGrowthMultiplier;
            characterStats.AddStat(stat, growth);
        }
    }

    private void UpdateCharacterDetails(CharacterDataSO characterData)
    {
        characterImage.sprite = characterData.Icon;
        characterName.text = characterData.Name;
        UpdateUI();
    }

    private void UpdateUI()
    {
        levelText.text = $"Level: {level}";
        float progress = IsMaxLevel ? 1f : (float)experience / ExperienceToLevelUp();
        experienceSlider.fillAmount = progress;
    }

    private void GainWaveCompletionExperience()
    {
        int waveExperience = 100; 
        GainExperience(waveExperience);
    }

    private void GainSurvivalCompletionExperience()
    {
        int survivalExperience = Mathf.FloorToInt(waveManager.SurvivalTime * 10); 
        GainExperience(survivalExperience);
    }

    public void Save()
    {
        SaveManager.Save(this, "Level", level);
        SaveManager.Save(this, "Experience", experience);
    }

    public void Load()
    {
        if (SaveManager.TryLoad(this, "Level", out object savedLevel))
            level = (int)savedLevel;

        if (SaveManager.TryLoad(this, "Experience", out object savedExperience))
            experience = (int)savedExperience;

        UpdateUI();
    }
}
