using System;
using UnityEngine;
using UnityEngine.UI;

public class OfflineEarning : MonoBehaviour
{
    public static OfflineEarning _instance;

    public Text offlineIncomeText;
    public float coinsToAdd = 0f;
    public TimeSpan allTimeSinceFirstTime;

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

        // Calculate offline earnings based on time away and offline earnings upgrade
        if (PlayerPrefs.HasKey("LAST_QUIT_TIME"))
        {
            DateTime lastQuit = DateTime.Parse(PlayerPrefs.GetString("LAST_QUIT_TIME"));
            TimeSpan timeAway = currentDateTime - lastQuit;

            float offlineMult = Upgrades._instance != null ? Upgrades._instance.offlinePercent : 1f;
            float minutesAway = (float)Math.Min(timeAway.TotalMinutes, 720); // Cap at 12 hours
            if (minutesAway > 1f)
            {
                coinsToAdd = Mathf.Max(50f, minutesAway * 20f * offlineMult);
                if (offlineIncomeText != null)
                    offlineIncomeText.text = CurrencyManager.GetSuffix((int)coinsToAdd);
                
                gameObject.SetActive(true);
            }
        }

        PlayerPrefs.SetString("LAST_QUIT_TIME", currentDateTime.ToString());
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LAST_QUIT_TIME", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetString("LAST_QUIT_TIME", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }
    }

    public void ClaimNormal()
    {
        if (CurrencyManager._instance != null)
            CurrencyManager._instance.AddCoins(coinsToAdd);

        gameObject.SetActive(false);
    }

    public void DoubleEarnings()
    {
        if (UnityAds.Instance != null)
        {
            UnityAds.Instance.ShowRewardedAd(
                onSuccess: () => {
                    Debug.Log("Rewarded Ad Watched! 2X Offline Earnings Awarded: " + (coinsToAdd * 2f));
                    if (CurrencyManager._instance != null)
                        CurrencyManager._instance.AddCoins(coinsToAdd * 2f);
                    gameObject.SetActive(false);
                },
                onFailure: () => {
                    Debug.LogWarning("Ad failed or was skipped.");
                }
            );
        }
        else
        {
            if (CurrencyManager._instance != null)
                CurrencyManager._instance.AddCoins(coinsToAdd * 2f);
            gameObject.SetActive(false);
        }
    }
}
