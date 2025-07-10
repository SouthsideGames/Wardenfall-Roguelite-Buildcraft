using System.Collections.Generic;
using UnityEngine;
using SouthsideGames.SaveManager;

public class UserManager : MonoBehaviour, IWantToBeSaved
{
    public static UserManager Instance { get; private set; }

    private const string USERNAME_KEY = "username";
    private const string FIRST_TIME_TUTORIAL_KEY = "firstTimeTutorial";
    private const string FIRST_LAUNCH_KEY = "firstLaunch";
    private const string UNLOCKED_CARDS_KEY = "unlockedCards";

    private string username;
    public string Username => username;

    private bool hasSeenFirstTimeTutorial;
    public bool HasSeenFirstTimeTutorial => hasSeenFirstTimeTutorial;

    private bool isFirstLaunch = true;
    public bool IsFirstLaunch => isFirstLaunch;

    private List<string> unlockedCards = new List<string>();
    public IReadOnlyList<string> UnlockedCards => unlockedCards;

    // Track if we have a username we want to push after login
    private string pendingUsernameForPlayfab = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();

            // Hook PlayfabManager login event
            if (PlayfabManager.Instance != null)
                PlayfabManager.Instance.OnLoginComplete += HandlePlayfabLoginComplete;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (isFirstLaunch)
        {
            GrantStarterRewards();
            isFirstLaunch = false;
            Save();
        }
    }

    private void OnDestroy()
    {
        if (PlayfabManager.Instance != null)
            PlayfabManager.Instance.OnLoginComplete -= HandlePlayfabLoginComplete;
    }

    /// <summary>
    /// User manually sets or changes username
    /// </summary>
    public void SetUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            return;

        username = newUsername;
        Save();

        if (PlayfabManager.Instance != null)
        {
            // If Playfab is already logged in, set immediately
            PlayfabManager.Instance.SetUserDisplayName(newUsername);
        }
        else
        {
            // Otherwise remember it until Playfab logs in
            pendingUsernameForPlayfab = newUsername;
        }
    }

    /// <summary>
    /// Called automatically when Playfab login completes
    /// </summary>
    private void HandlePlayfabLoginComplete(bool success)
    {
        if (!success)
        {
            Debug.LogWarning("[UserManager] Playfab login failed, won't push username.");
            return;
        }

        if (!string.IsNullOrEmpty(username))
        {
            Debug.Log("[UserManager] Playfab login complete, pushing saved username.");
            PlayfabManager.Instance.SetUserDisplayName(username);
        }
        else if (!string.IsNullOrEmpty(pendingUsernameForPlayfab))
        {
            Debug.Log("[UserManager] Pushing pending username to Playfab.");
            PlayfabManager.Instance.SetUserDisplayName(pendingUsernameForPlayfab);
            pendingUsernameForPlayfab = null;
        }
    }

    public bool IsFirstTimePlayer() => string.IsNullOrEmpty(username);

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

    public void UnlockCard(string cardID)
    {
        if (!unlockedCards.Contains(cardID))
        {
            unlockedCards.Add(cardID);
            Save();
        }
    }

    public bool HasCardUnlocked(string cardID)
    {
        return unlockedCards.Contains(cardID);
    }

    public void GrantStarterRewards()
    {
        Debug.Log("[UserManager] Granting starter rewards!");

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.AdjustCardCurrency(20);
        else
            Debug.LogError("[UserManager] CurrencyManager.Instance is null! Card Points not granted.");

        string[] starterCardIDs = new string[]
        {
            "S-007",
            "S-011",
            "S-066",
            "S-071",
            "S-075",
            "S-081"
        };

        foreach (var id in starterCardIDs)
        {
            if (!unlockedCards.Contains(id))
                unlockedCards.Add(id);
        }

        Debug.Log("[UserManager] Starter rewards granted: 20 Card Points and starter cards.");
        Save();
    }

    public void ClearAllData()
    {
        SaveManager.ClearData();

        username = "";
        hasSeenFirstTimeTutorial = false;
        isFirstLaunch = true;
        unlockedCards.Clear();

        Save();

        GameManager.Instance?.Restart();
    }

    public void Save()
    {
        SaveManager.Save(this, USERNAME_KEY, username);
        SaveManager.Save(this, FIRST_TIME_TUTORIAL_KEY, hasSeenFirstTimeTutorial);
        SaveManager.Save(this, FIRST_LAUNCH_KEY, isFirstLaunch);
        SaveManager.Save(this, UNLOCKED_CARDS_KEY, unlockedCards);
    }

    public void Load()
    {
        if (SaveManager.TryLoad(this, USERNAME_KEY, out object usernameObj))
            username = (string)usernameObj;
        else
            username = "";

        if (SaveManager.TryLoad(this, FIRST_TIME_TUTORIAL_KEY, out object tutorialObj))
            hasSeenFirstTimeTutorial = (bool)tutorialObj;
        else
            hasSeenFirstTimeTutorial = false;

        if (SaveManager.TryLoad(this, FIRST_LAUNCH_KEY, out object firstLaunchObj))
            isFirstLaunch = (bool)firstLaunchObj;
        else
            isFirstLaunch = true;

        if (SaveManager.TryLoad(this, UNLOCKED_CARDS_KEY, out object cardsObj))
            unlockedCards = (List<string>)cardsObj;
        else
            unlockedCards = new List<string>();

    }
}
