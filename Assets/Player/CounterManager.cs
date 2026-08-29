using TMPro;
using Unity.Services.Core;
using UnityEngine;
using System;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;

public class CounterManager : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public GameObject counterParent;
    private double previous_score = 0;

    void Start()
    {
        if (MainMenu.endlessMode)
        {
            counterParent.SetActive(true);
        }
        else
        {
            counterParent.SetActive(false);
        }
    }
    
    async void Awake()
    {
        Debug.Log("Setup!");
        Debug.Log("Initializing Services");
        await UnityServices.InitializeAsync();
        
        
        AuthenticationService.Instance.SignedIn += () => {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
        
        
        try
        {
            if (AuthenticationService.Instance.IsSignedIn) return;
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    async void Update()
    {
        if (MainMenu.endlessMode)
        {
            counterText.text = GameResult.currentLoop.ToString();
        }
    
        if (previous_score != GameResult.currentLoop)
        {
            previous_score = GameResult.currentLoop;
            var scoreResponse =
                await LeaderboardsService.Instance.AddPlayerScoreAsync("woowee_leaderboard", GameResult.currentLoop);

        }
    }
}