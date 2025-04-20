using UnityEngine;

// TutorialData Scriptable Object
[CreateAssetMenu(fileName = "TutorialData", menuName = "Tutorial/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public string PanelID;
    public string[] DialogueLines;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public GameObject TutorialPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckForTutorial(string panelID)
    {
        // Check PlayerPrefs for whether the tutorial has been shown before
        string prefKey = "TutorialShown_" + panelID;
        if (!PlayerPrefs.HasKey(prefKey))
        {
            //Load Tutorial Data
            TutorialData tutorialData = Resources.Load<TutorialData>(panelID);
            if (tutorialData != null)
            {
                //Spawn Tutorial Prefab.
                GameObject tutorialInstance = Instantiate(TutorialPrefab);
                //Pass tutorialData to the prefab for processing.  This will depend on your prefab implementation.
                tutorialInstance.GetComponent<TutorialController>().SetData(tutorialData);
            }
            PlayerPrefs.SetInt(prefKey, 1);
        }
    }
}


// Example Tutorial Controller (Attach to the Tutorial Prefab)
public class TutorialController : MonoBehaviour
{
    private TutorialData tutorialData;
    public void SetData(TutorialData data) { tutorialData = data; }

    // Add your logic to display dialogue lines from tutorialData here.
    // This will likely involve UI elements and potentially a dialogue system.
    //  For example, you might use a TextMeshProUGUI to display the text.
}


// Example Usage in Weapon Select Panel Script
public class WeaponSelectPanel : MonoBehaviour
{
    private void OnEnable()
    {
        TutorialManager.Instance.CheckForTutorial("weapon_select");
    }
}