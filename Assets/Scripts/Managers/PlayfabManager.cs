using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance;

    [Header("PlayFab Settings")]
    public string leaderboardName = "Survival Legends";
    public string secondLeaderboardName = "All Time Director's";
    public GameObject leaderboardPrefab;

    public event Action<bool> OnLoginComplete;
    public event Action<bool> OnScorePosted;
    public event Action<List<PlayerLeaderboardEntry>> OnLeaderboardReceived;
    public event Action<bool> OnDisplayNameSet;

    [Header("Leaderboard UI")]
    public Transform leaderboard1ContentParent;
    public Transform leaderboard2ContentParent;

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

    private void Start()
    {
        LoginWithDeviceID();
    }

    public void LoginWithDeviceID()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "1B2B36";
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"PlayFab login successful! PlayFab ID: {result.PlayFabId}");

        string serverDisplayName = result?.InfoResultPayload?.PlayerProfile?.DisplayName;
        Debug.Log($"Server has display name: {serverDisplayName}");

        // Store this name locally if it's the first time
        if (!string.IsNullOrEmpty(serverDisplayName))
        {
            // ✅ If our local save doesn't have anything yet, adopt the server name
            if (string.IsNullOrEmpty(UserManager.Instance.Username))
            {
                Debug.Log("[PlayfabManager] Adopting server display name locally.");
                UserManager.Instance.SetUsername(serverDisplayName);
            }
            else if (UserManager.Instance.Username != serverDisplayName)
            {
                // ✅ Names don't match—try updating server to match local
                Debug.Log("[PlayfabManager] Local username differs from server. Attempting update...");
                SetUserDisplayName(UserManager.Instance.Username);
            }
            else
            {
                Debug.Log("[PlayfabManager] Local and server username match. No update needed.");
            }
        }
        else
        {
            // Server has no display name yet. If we have one saved, push it.
            if (!string.IsNullOrEmpty(UserManager.Instance.Username))
            {
                Debug.Log("[PlayfabManager] Server has no name. Setting from local.");
                SetUserDisplayName(UserManager.Instance.Username);
            }
            else
            {
                Debug.Log("[PlayfabManager] No username anywhere yet.");
            }
        }

        OnLoginComplete?.Invoke(true);
    }


    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFab login failed: {error.GenerateErrorReport()}");
        OnLoginComplete?.Invoke(false);
    }

    public void SetUserDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnDisplayNameFailure);
    }

    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"Display name set successfully: {result.DisplayName}");
        OnDisplayNameSet?.Invoke(true);
    }

    private void OnDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to set display name: {error.GenerateErrorReport()}");
        OnDisplayNameSet?.Invoke(false);
    }

    public void PostHighScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScorePostSuccess, OnScorePostFailure);
    }

    private void OnScorePostSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score posted successfully!");
        OnScorePosted?.Invoke(true);
        GetHighScores(100);
    }

    private void OnScorePostFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to post score: {error.GenerateErrorReport()}");
        OnScorePosted?.Invoke(false);
    }

    public void GetHighScores(int maxResults = 100)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = maxResults
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"Retrieved {result.Leaderboard.Count} leaderboard entries");
        OnLeaderboardReceived?.Invoke(result.Leaderboard);

        if (leaderboard1ContentParent == null)
        {
            Debug.LogWarning("Leaderboard content parent not set!");
            return;
        }

        // Clear existing items
        for (int i = leaderboard1ContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(leaderboard1ContentParent.GetChild(i).gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            var playerItem = Instantiate(leaderboardPrefab, leaderboard1ContentParent, false);
            var texts = playerItem.GetComponentsInChildren<TMPro.TMP_Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = (item.Position + 1).ToString();
                texts[1].text = item.DisplayName;
                texts[2].text = FormatSecondsToTimeString(item.StatValue);
            }
        }
    }

    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to get leaderboard: {error.GenerateErrorReport()}");
        OnLeaderboardReceived?.Invoke(null);
    }

    public void GetPlayerLeaderboardPosition()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetPlayerPositionSuccess, OnGetPlayerPositionFailure);
    }

    private void OnGetPlayerPositionSuccess(GetLeaderboardAroundPlayerResult result)
    {
        if (result.Leaderboard.Count > 0)
        {
            var playerEntry = result.Leaderboard[0];
            Debug.Log($"Player position: {playerEntry.Position + 1}, Score: {playerEntry.StatValue}");
        }
    }

    private void OnGetPlayerPositionFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to get player position: {error.GenerateErrorReport()}");
    }

    private string FormatSecondsToTimeString(int seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);

        if (time.TotalHours >= 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
        else
            return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

    public void GetSecondLeaderboard(int maxResults = 100)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = secondLeaderboardName,
            StartPosition = 0,
            MaxResultsCount = maxResults
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetSecondLeaderboardSuccess, OnGetLeaderboardFailure);
    }
    
    private void OnGetSecondLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"Retrieved {result.Leaderboard.Count} entries for SECOND leaderboard");

        if (leaderboard2ContentParent == null)
        {
            Debug.LogWarning("Second leaderboard content parent not set!");
            return;
        }

        // Clear existing items
        for (int i = leaderboard2ContentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(leaderboard2ContentParent.GetChild(i).gameObject);
        }

        foreach (var item in result.Leaderboard)
        {
            var playerItem = Instantiate(leaderboardPrefab, leaderboard2ContentParent, false);
            var texts = playerItem.GetComponentsInChildren<TMPro.TMP_Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = (item.Position + 1).ToString();
                texts[1].text = item.DisplayName;
                texts[2].text = FormatSecondsToTimeString(item.StatValue);
            }
        }
    }



}
