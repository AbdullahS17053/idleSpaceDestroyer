using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class OfflineEarning : MonoBehaviour, IUnityAdsShowListener
{
    public static OfflineEarning _instance;

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        CalculateTime();
    }

    void CalculateTime()
    {
        DateTime currentDateTime = DateTime.Now;

        if (!PlayerPrefs.HasKey("FIRST_LOGIN_TIME"))
        {
            PlayerPrefs.SetString("FIRST_LOGIN_TIME", currentDateTime.ToString());
        }

        DateTime firstDate = DateTime.Parse(PlayerPrefs.GetString("FIRST_LOGIN_TIME"));

        allTimeSinceFirstTime = currentDateTime - firstDate;
    }

    public Text offlineIncomeText;
    float coinsToAdd;
    public TimeSpan allTimeSinceFirstTime;

    const string rewardedAdUnitId = "Rewarded_Android"; 
    // Change to Rewarded_iOS for iOS

    public void DoubleEarnings()
    {
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(rewardedAdUnitId, this);
        }
    }

    // Called when ad finishes successfully
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId == rewardedAdUnitId &&
            showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            CurrencyManager._instance.AddCoins(coinsToAdd);
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log("Ad failed: " + message);
    }

    public void OnUnityAdsShowStart(string adUnitId) { }

    public void OnUnityAdsShowClick(string adUnitId) { }
}