using UnityEngine;
using SouthsideGames.SaveManager;

public class UserManager : MonoBehaviour, IWantToBeSaved
{
    public static UserManager Instance { get; private set; }
    private const string USERNAME_KEY = "username";
    private string username;
    public string Username => username;
    private bool hasSeenFirstTimeTutorial;
    public bool HasSeenFirstTimeTutorial => hasSeenFirstTimeTutorial;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Load();
    }

    public bool IsFirstTimePlayer() => string.IsNullOrEmpty(username);

    public void SetUsername(string newUsername)
    {
        username = newUsername;
        Save();
    }

    public void ClearAllData()
    {
        SaveManager.ClearData();

        username = "";
        Save();

        GameManager.Instance.Restart();
    }

    public void Save()
    {
        SaveManager.Save(this, USERNAME_KEY, username);
        SaveManager.Save(this, "firstTimeTutorial", hasSeenFirstTimeTutorial);
    }


   public void Load()
    {
        if (SaveManager.TryLoad(this, USERNAME_KEY, out object usernameObj))
            username = (string)usernameObj;
        else
            username = "";

        if (SaveManager.TryLoad(this, "firstTimeTutorial", out object tutorialObj))
            hasSeenFirstTimeTutorial = (bool)tutorialObj;
        else
            hasSeenFirstTimeTutorial = false;
    }


   public bool NeedsFirstTimeTutorial()
    {
        Debug.Log($"[UserManager] Checking NeedsFirstTimeTutorial: hasSeenFirstTimeTutorial={hasSeenFirstTimeTutorial}");
        return !hasSeenFirstTimeTutorial;
    }


    public void MarkFirstTimeTutorialSeen()
    {
        hasSeenFirstTimeTutorial = true;
        Save();
    }


}