using UnityEngine;
using SouthsideGames.SaveManager;
using UnityEngine.UI;
using TMPro;

public class CharacterExperience : MonoBehaviour, IWantToBeSaved
{
    [Header("Level Progression")]
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;

    private CharacterStats characterStats;
    private CharacterDataSO characterData;

    [Header("UI Elements")]
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private WaveManager waveManager;

    public int Level => level;
    public int Experience => experience;

    private void Awake()
    {
        characterStats = CharacterStats.Instance;
        characterData = characterStats.CharacterData; // Get the data for the current character

        CharacterSelectionManager.OnCharacterSelected += UpdateCharacterDetails;
        WaveManager.OnWaveCompleted += GainWaveCompletionExperience;
        WaveManager.OnSurvivalCompleted += GainSurvivalCompletionExperience;
    }

    private void Start()
    {
        Load(); // Load saved level and experience
    }

    private void OnDestroy()
    {
        CharacterSelectionManager.OnCharacterSelected -= UpdateCharacterDetails;
        WaveManager.OnWaveCompleted -= GainWaveCompletionExperience;
        WaveManager.OnSurvivalCompleted -= GainSurvivalCompletionExperience;
    }

    public void GainExperience(int amount)
    {
        if (level >= characterData.MaxLevel) // Use data from CharacterDataSO
            return;

        experience += amount;

        while (experience >= ExperienceToLevelUp() && level < characterData.MaxLevel)
        {
            experience -= ExperienceToLevelUp();
            LevelUp();
        }

        if (level >= characterData.MaxLevel)
            experience = ExperienceToLevelUp();

        UpdateUI();
        Save(); // Save progress after gaining experience
    }

    public int ExperienceToLevelUp()
    {
        // Use base experience and growth rate from CharacterDataSO
        return Mathf.FloorToInt(characterData.BaseExperienceRequired * Mathf.Pow(characterData.ExperienceGrowthRate, level - 1));
    }

    private void LevelUp()
    {
        if (level >= characterData.MaxLevel)
            return;

        level++;
        ApplyStatGrowth();

        Debug.Log($"Leveled up to {level}!");
    }

    private void ApplyStatGrowth()
    {
        // Apply stat growth based on CharacterDataSO settings
        foreach (Stat stat in characterStats.CharacterData.BaseStats.Keys)
        {
            float growth = characterStats.CharacterData.BaseStats[stat] * characterData.StatGrowthMultiplier;
            characterStats.AddStat(stat, growth);
        }
    }

    private void UpdateCharacterDetails(CharacterDataSO characterData)
    {
        this.characterData = characterData; // Update character data if switched
        characterImage.sprite = characterData.Icon;
        characterName.text = characterData.Name;
        UpdateUI();
    }

    private void UpdateUI()
    {
        levelText.text = $"{level}";
        float progress = (level >= characterData.MaxLevel) ? 1f : (float)experience / ExperienceToLevelUp();
        experienceSlider.value = progress;
    }

    private void GainWaveCompletionExperience()
    {
        int waveExperience = 100; // Set experience for wave completion
        GainExperience(waveExperience);
    }

    private void GainSurvivalCompletionExperience()
    {
        int survivalExperience = Mathf.FloorToInt(waveManager.SurvivalTime * 10); // 10 XP per second survived
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