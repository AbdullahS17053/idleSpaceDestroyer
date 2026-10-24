using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class OfflineEarning : MonoBehaviour
{
    public static OfflineEarning _instance;

    [Header("UI References")]
    public Text offlineIncomeText;
    public GameObject offlinePopupPanel;
    public Button claimButton;
    public Button doubleButton;

    [Header("Balance")]
    [Tooltip("Base coins earned per minute (e.g. 5 coins/min). Tunable in Inspector.")]
    public float baseCoinsPerMinute = 5f;

    [Header("Debug / Testing")]
    public bool testInEditorWithShortDelay = true; // Allows testing in editor with >= 5 seconds delay

    public float coinsToAdd = 0f;
    public TimeSpan allTimeSinceFirstTime;

    private const string PREF_FIRST_LOGIN = "FIRST_LOGIN_TIME";
    private const string PREF_LAST_QUIT = "LAST_QUIT_TIME";

    private void Awake()
    {
        _instance = this;
    }

    IEnumerator Start()
    {
        // Wait until end of frame so all other managers (Upgrades, DPSDisplay, LevelManager, CurrencyManager)
        // have completed their Awake() and Start() routines.
        yield return new WaitForEndOfFrame();

        FindPopupPanelReferences();
        CalculateTime();
    }

    void FindPopupPanelReferences()
    {
        if (offlinePopupPanel == null)
        {
            if (offlineIncomeText != null)
            {
                // Traverse up to find OfflineEarnings root container
                Transform t = offlineIncomeText.transform;
                while (t != null && t.parent != null && t.parent.name != "Canvas")
                {
                    t = t.parent;
                }
                if (t != null)
                {
                    offlinePopupPanel = t.gameObject;
                }
            }

            if (offlinePopupPanel == null)
            {
                // Search under Canvas
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    var allTransforms = canvas.GetComponentsInChildren<Transform>(true);
                    foreach (var tr in allTransforms)
                    {
                        if (tr.name == "OfflineEarnings")
                        {
                            offlinePopupPanel = tr.gameObject;
                            break;
                        }
                    }
                }
            }
        }

        // Auto-find offlineIncomeText if missing
        if (offlineIncomeText == null && offlinePopupPanel != null)
        {
            Text[] texts = offlinePopupPanel.GetComponentsInChildren<Text>(true);
            foreach (var t in texts)
            {
                if (t.transform.parent != null && t.transform.parent.name.ToLower().Contains("coins"))
                {
                    offlineIncomeText = t;
                    break;
                }
            }
            if (offlineIncomeText == null)
            {
                foreach (var t in texts)
                {
                    if (t.GetComponentInParent<Button>() == null && !t.gameObject.name.ToLower().Contains("header"))
                    {
                        offlineIncomeText = t;
                        break;
                    }
                }
            }
        }

        // Auto-hook buttons if found in popup panel
        if (offlinePopupPanel != null)
        {
            Button[] buttons = offlinePopupPanel.GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                string bName = b.name.ToLower();

                if (bName.Contains("close"))
                {
                    // Close button claims normal reward and closes popup
                    b.onClick.RemoveListener(ClaimNormal);
                    b.onClick.AddListener(ClaimNormal);
                    continue;
                }

                bool isDoubleBtn = false;
                if (bName.Contains("double"))
                {
                    isDoubleBtn = true;
                }
                else
                {
                    Image[] imgs = b.GetComponentsInChildren<Image>(true);
                    foreach (var img in imgs)
                    {
                        if (img.sprite != null && (img.sprite.name.ToLower().Contains("video") || img.sprite.name.ToLower().Contains("ad") || img.name.ToLower().Contains("ad")))
                        {
                            isDoubleBtn = true;
                            break;
                        }
                    }

                    // If positioned to the right (anchored x > 0), it is the 2X double button
                    RectTransform rt = b.GetComponent<RectTransform>();
                    if (rt != null && rt.anchoredPosition.x > 0)
                    {
                        isDoubleBtn = true;
                    }
                }

                if (isDoubleBtn)
                {
                    doubleButton = b;
                    b.onClick.RemoveListener(DoubleEarnings);
                    b.onClick.AddListener(DoubleEarnings);
                }
                else
                {
                    claimButton = b;
                    b.onClick.RemoveListener(ClaimNormal);
                    b.onClick.AddListener(ClaimNormal);
                }
            }
        }
    }

    /// <summary>
    /// Calculates the actual coins earned based on player progression,
    /// offline earning multiplier, and elapsed time away.
    /// Scaled to a balanced rate (default ~5 coins per minute base).
    /// </summary>
    public float CalculateCoinsEarned(TimeSpan timeAway)
    {
        // 1. Determine player offline multiplier (1 + offline upgrade level)
        float offlineMult = 1f;
        if (Upgrades._instance != null && Upgrades._instance.offlinePercent > 0f)
        {
            offlineMult = Upgrades._instance.offlinePercent;
        }
        else
        {
            int offlineLevel = PlayerPrefs.GetInt("OFFLINE_EARNING_LEVEL", 1);
            offlineMult = 1f + offlineLevel;
        }

        // 2. Current stage level progression
        int currentLevel = LevelManager._instance != null ? LevelManager._instance.currentLevel : PlayerPrefs.GetInt("CURRENT_LEVEL", 1);

        // 3. Balanced rate per minute:
        // Base is ~5 coins/min, scaling gently with player level (+10% per level) and offline upgrade multiplier
        float levelMultiplier = 1f + Mathf.Max(0, currentLevel - 1) * 0.1f;
        float coinsPerMinute = baseCoinsPerMinute * levelMultiplier * offlineMult;
        float coinsPerSecond = coinsPerMinute / 60f;

        // 4. Elapsed time (capped at 12 hours = 43200 seconds)
        float elapsedSeconds = Mathf.Clamp((float)timeAway.TotalSeconds, 0f, 12f * 3600f);

        float calculatedCoins = Mathf.Round(elapsedSeconds * coinsPerSecond);
        return Mathf.Max(1f, calculatedCoins);
    }

    public void CalculateTime()
    {
        DateTime currentDateTime = DateTime.UtcNow;

        if (!PlayerPrefs.HasKey(PREF_FIRST_LOGIN))
        {
            PlayerPrefs.SetString(PREF_FIRST_LOGIN, currentDateTime.ToString("o"));
        }

        string firstLoginStr = PlayerPrefs.GetString(PREF_FIRST_LOGIN, "");
        DateTime firstDate;
        if (DateTime.TryParse(firstLoginStr, null, DateTimeStyles.RoundtripKind, out firstDate) || DateTime.TryParse(firstLoginStr, out firstDate))
        {
            allTimeSinceFirstTime = currentDateTime - firstDate;
        }

        // Calculate offline earnings based on time away and offline earnings upgrade
        if (PlayerPrefs.HasKey(PREF_LAST_QUIT))
        {
            string lastQuitStr = PlayerPrefs.GetString(PREF_LAST_QUIT, "");
            DateTime lastQuit;
            if (DateTime.TryParse(lastQuitStr, null, DateTimeStyles.RoundtripKind, out lastQuit) || DateTime.TryParse(lastQuitStr, out lastQuit))
            {
                TimeSpan timeAway = currentDateTime - lastQuit;

                bool shouldShow = false;
#if UNITY_EDITOR
                if (testInEditorWithShortDelay)
                {
                    // Allow >= 5 seconds in Editor for quick testing
                    shouldShow = timeAway.TotalSeconds >= 5.0;
                }
                else
                {
                    shouldShow = timeAway.TotalMinutes >= 1.0;
                }
#else
                shouldShow = timeAway.TotalMinutes >= 1.0;
#endif

                if (shouldShow)
                {
                    coinsToAdd = CalculateCoinsEarned(timeAway);

                    if (offlineIncomeText != null)
                        offlineIncomeText.text = CurrencyManager.GetSuffix((int)coinsToAdd);

                    if (offlinePopupPanel != null)
                    {
                        offlinePopupPanel.SetActive(true);
                        offlinePopupPanel.transform.SetAsLastSibling();
                        Debug.Log("[OfflineEarning] Showing offline rewards popup: +" + CurrencyManager.GetSuffix((int)coinsToAdd) + " coins (" + coinsToAdd + " exact) for away time: " + (int)timeAway.TotalSeconds + "s");
                    }
                    else
                    {
                        Debug.LogWarning("[OfflineEarning] offlinePopupPanel is null! Could not show popup.");
                    }
                }
                else
                {
                    Debug.Log("[OfflineEarning] Time away: " + (int)timeAway.TotalSeconds + "s (threshold not met).");
                }
            }
        }

        SaveQuitTime();
    }

    public void SaveQuitTime()
    {
        PlayerPrefs.SetString(PREF_LAST_QUIT, DateTime.UtcNow.ToString("o"));
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SaveQuitTime();
    }

    void OnDisable()
    {
        SaveQuitTime();
    }

    void OnDestroy()
    {
        SaveQuitTime();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveQuitTime();
        }
        else
        {
            // App resumed from background
            CalculateTime();
        }
    }

    public void ClaimNormal()
    {
        float toAdd = coinsToAdd;
        if (CurrencyManager._instance != null && toAdd > 0)
            CurrencyManager._instance.AddCoins(toAdd);

        Debug.Log("[OfflineEarning] Normal claim: added " + toAdd + " coins.");
        coinsToAdd = 0f;
        if (offlinePopupPanel != null)
            offlinePopupPanel.SetActive(false);
    }

    public void DoubleEarnings()
    {
        float reward = coinsToAdd * 2f;
        if (UnityAds.Instance != null && UnityAds.Instance.IsRewardedAdReady())
        {
            UnityAds.Instance.ShowRewardedAd(
                onSuccess: () => {
                    Debug.Log("[OfflineEarning] Rewarded ad completed! 2X Offline Earnings: " + reward);
                    if (CurrencyManager._instance != null && reward > 0)
                        CurrencyManager._instance.AddCoins(reward);
                    coinsToAdd = 0f;
                    if (offlinePopupPanel != null)
                        offlinePopupPanel.SetActive(false);
                },
                onFailure: () => {
                    Debug.LogWarning("[OfflineEarning] Ad failed or skipped. Awarding fallback double earnings.");
                    if (CurrencyManager._instance != null && reward > 0)
                        CurrencyManager._instance.AddCoins(reward);
                    coinsToAdd = 0f;
                    if (offlinePopupPanel != null)
                        offlinePopupPanel.SetActive(false);
                }
            );
        }
        else
        {
            // Fallback for editor or when ad is not ready
            Debug.Log("[OfflineEarning] Rewarded ad not ready or in Editor. Awarding 2X Offline Earnings: " + reward);
            if (CurrencyManager._instance != null && reward > 0)
                CurrencyManager._instance.AddCoins(reward);
            coinsToAdd = 0f;
            if (offlinePopupPanel != null)
                offlinePopupPanel.SetActive(false);
        }
    }

    [ContextMenu("Force Show Offline Popup (Test)")]
    public void ForceShowTestPopup()
    {
        FindPopupPanelReferences();
        TimeSpan fakeTime = TimeSpan.FromMinutes(10);
        coinsToAdd = CalculateCoinsEarned(fakeTime);

        if (offlineIncomeText != null)
            offlineIncomeText.text = CurrencyManager.GetSuffix((int)coinsToAdd);
        if (offlinePopupPanel != null)
        {
            offlinePopupPanel.SetActive(true);
            offlinePopupPanel.transform.SetAsLastSibling();
        }
        Debug.Log("[OfflineEarning] Force-showing test offline popup with 10 mins simulated: +" + CurrencyManager.GetSuffix((int)coinsToAdd) + " coins (" + coinsToAdd + ")");
    }
}
