using System;
using System.Collections.Generic;
using DefaultNamespace;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayFabManager : MonoBehaviour
{
    private static string _loggedInPlayFabId;

    public enum GameType
    {
        ReactionTime,
        AimTrainer,
        VerbalMemory
    }

    public int positionInLeaderboardRT;
    
    public static PlayFabManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest()
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            // CustomId = "TestingCustomId-00",
            CreateAccount = true
        };
        // Most basic/simple authentication option that creates anonymous accounts.
        // We are not using any credentials here like email, username, or password.
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    public void SendScoreToLeaderboard(int score, GameType gameType)
    {
        string leaderboardName = GetGameType(gameType);
        
        // leaderboard is considered player statistics in playfab
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    public void GetLeaderboard(GameType gameType)
    {
        string leaderboardName = GetGameType(gameType);
        var request = new GetLeaderboardRequest()
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
    }
    
    

    // will get a leaderboard that surrounds the player (lets say his at position 84, we will get table with 80 till 90
    // where we can see the player in the leaderboard.
    public void GetLeaderboardAroundPlayer(GameType gameType)
    {
        string leaderboardName = GetGameType(gameType);

        var request = new GetLeaderboardAroundPlayerRequest()
        {
            MaxResultsCount = 10,
            StatisticName = leaderboardName
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, OnError);
    }

    private string GetGameType(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.ReactionTime:
                return "Reaction Time";
            case GameType.AimTrainer:
                return "AimTrainer";
            case GameType.VerbalMemory:
                return "VerbalMemory";
            default:
                return String.Empty;
        }
    }

    #region CALLBACKS

    void OnLoginSuccess(LoginResult loginResult)
    {
        // Debug.Log($"Success: " + loginResult.AuthenticationContext);
        _loggedInPlayFabId = loginResult.PlayFabId;
        GetLeaderboardAroundPlayer(GameType.ReactionTime);
    }
    
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        foreach (var entry in result.Leaderboard)
        {
            Debug.Log($"Id: {entry.PlayFabId} - Position: {entry.Position} - Score: {entry.StatValue}");
        }
    }
    
    private void OnGetLeaderboardAroundPlayerSuccess(GetLeaderboardAroundPlayerResult result)
    {
        Debug.Log(nameof(OnGetLeaderboardAroundPlayerSuccess));
        foreach (var entry in result.Leaderboard)
        {
            Debug.Log($"Id: {entry.PlayFabId} - Position: {entry.Position} - Score: {entry.StatValue}");
            if (entry.PlayFabId == _loggedInPlayFabId)
            {
                positionInLeaderboardRT = entry.Position;
                Debug.Log($"My Position: {positionInLeaderboardRT}");
            }
        }
    }
    
    private void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("-- Leaderboard successfully updated --");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogWarning($"-- Error: {error.ErrorMessage} --");
    }

    #endregion
    
    
}
