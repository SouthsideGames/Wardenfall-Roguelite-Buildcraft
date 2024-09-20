using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterLevel : MonoBehaviour
{   
    [Header("Elements")]
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Settings")]
    private int requiredXp;
    private int currentXp;
    private int level = 1;


    // Start is called before the first frame update
    void Start()
    {
        UpdateRequiredXP();
        UpdateVisuals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateVisuals()
    {
        xpBar.value = (float)currentXp / requiredXp;
        levelText.text = "lvl " + level;
    }

    private void UpdateRequiredXP()
    {

    }
}
