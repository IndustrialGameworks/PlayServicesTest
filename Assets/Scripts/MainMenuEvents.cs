using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuEvents : MonoBehaviour {

    private Text signInButtonText;
    private Text authStatus;
    private Text debugText;
    private Text scoreText;

    private GameObject achievementButton;
    private GameObject leaderboardButton;

    public int score = 0;

    // Use this for initialization
    void Start () {

        signInButtonText = GameObject.Find("signInButton").GetComponentInChildren<Text>();
        authStatus = GameObject.Find("authStatus").GetComponent<Text>();
        debugText = GameObject.Find("Debug").GetComponentInChildren<Text>();
        scoreText = GameObject.Find("scoreText").GetComponent<Text>();

        achievementButton = GameObject.Find("achievementButton");
        leaderboardButton = GameObject.Find("leaderboardButton");

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        // registers a callback to handle game invitations received while the game is not running.
        //.WithInvitationDelegate(< callback method >)
        // registers a callback for turn based match notifications received while the
        // game is not running.
        //.WithMatchDelegate(< callback method >)
        // requests the email address of the player be available.
        // Will bring up a prompt for consent.
        //.RequestEmail()
        // requests a server auth code be generated so it can be passed to an
        //  associated back end server application and exchanged for an OAuth token.
        //.RequestServerAuthCode(false)
        // requests an ID token be generated.  This OAuth token can be used to
        //  identify the player to other services such as Firebase.
        //.RequestIdToken()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

    }
	
	// Update is called once per frame
	void Update () {

        achievementButton.SetActive(Social.localUser.authenticated); //if player authenticated, return true and set button to active
        leaderboardButton.SetActive(Social.localUser.authenticated);

        scoreText.text = ("Score: " + score);
    }

    public void SignIn()
    {
        debugText.text = "Attempting authentication"; //for process reference

        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            Social.localUser.Authenticate((bool success) => {
                SignInCallback(success); //pushes returned bool to be handled by SignInCallback
            }); ;
        }
        else
        {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();

            debugText.text = "Signed out, resetting UI";

            // Reset UI
            signInButtonText.text = "Sign In";
            authStatus.text = "";
        }
    }

    public void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("Signed in!");
            debugText.text = "Authentication succeeded"; //for process reference

            // Change sign-in button text
            signInButtonText.text = "Sign out";

            // Show the user's name
            authStatus.text = "Signed in as: " + Social.localUser.userName;
        }
        else
        {
            Debug.Log("Sign-in failed...");
            debugText.text = "Authentication failed"; //for process reference

            // Show failure message
            signInButtonText.text = "Sign in";
            authStatus.text = "Sign-in failed";
        }
    }

    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Cannot show achievements, as not logged in");
            debugText.text = "Cannot show achievements: not authenticated";
        }
    }

    public void ActionButton()
    {
        if (Social.localUser.authenticated)
        {
            // Unlock the "welcome" achievement, it is OK to
            // unlock multiple times, only the first time matters.
            PlayGamesPlatform.Instance.ReportProgress(
                GPGSIds.achievement_boss_level_5000,
                100.0f, (bool success) =>
                {
                    Debug.Log(" Welcome Unlock: " +
                              success);
                    debugText.text = ("First achievement unlocked" + success);
                });
        }
        PlayGamesPlatform.Instance.IncrementAchievement(
            GPGSIds.achievement_get_a_score_of_five, 1,
            (bool success) =>
            {
                Debug.Log("Score Increment: " + success);
                debugText.text = ("Increment achievement incremented: " + success);
            });

        score++;

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Note: make sure to add 'using GooglePlayGames'
            PlayGamesPlatform.Instance.ReportScore(score,
                GPGSIds.leaderboard_leaderboard_test,
                (bool success) =>
                {
                    Debug.Log("Leaderboard update success: " + success);
                    debugText.text = ("Leaderboard update success: " + success);
                });
        }
    }

    public void ShowLeaderboard()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            Debug.Log("Cannot show leaderboard: not authenticated");
            debugText.text = "Cannot show leaderboard: not authenticated";
        }
    }
}
