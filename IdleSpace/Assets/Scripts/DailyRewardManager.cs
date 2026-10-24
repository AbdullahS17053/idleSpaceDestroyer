using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardManager : MonoBehaviour {

    public static DailyRewardManager Instance;

    public Button claimButton;
    public Text claimButtonText;
    public GameObject day1Row;
    public GameObject day3Row;
    public GameObject day7Row;
    public Text statusNoticeText;

    private const string PREF_LAST_CLAIM = "DAILY_REWARD_LAST_CLAIM_DATE";
    private const string PREF_STREAK = "DAILY_REWARD_CURRENT_STREAK";
    private const string PREF_TOTAL_CLAIMS = "DAILY_REWARD_TOTAL_CLAIMS";

    public int currentStreak = 0; // 0 to 7
    public bool isClaimableToday = false;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        AutoFindReferences();
        RefreshState();
    }

    private void OnEnable() {
        AutoFindReferences();
        RefreshState();
    }

    public void AutoFindReferences() {
        // Find Claim button
        if (claimButton == null) {
            var btns = GetComponentsInChildren<Button>(true);
            foreach (var b in btns) {
                var txt = b.GetComponentInChildren<Text>();
                if (txt != null && (txt.text.Contains("CLOSE") || txt.text.Contains("CLAIM") || b.name.Contains("Button"))) {
                    claimButton = b;
                    claimButtonText = txt;
                    break;
                }
            }
        }

        // Find Rows:
        var allTexts = GetComponentsInChildren<Text>(true);
        foreach (var txt in allTexts) {
            string raw = txt.text.ToUpper();
            if (raw.Contains("DAY 1") || raw.Contains("DAY  1")) {
                if (day1Row == null && txt.transform.parent != null)
                    day1Row = txt.transform.parent.gameObject;
            } else if (raw.Contains("DAY 3") || raw.Contains("DAY  3")) {
                if (day3Row == null && txt.transform.parent != null)
                    day3Row = txt.transform.parent.gameObject;
            } else if (raw.Contains("DAY 7") || raw.Contains("DAY  7")) {
                if (day7Row == null && txt.transform.parent != null)
                    day7Row = txt.transform.parent.gameObject;
            }
        }

        if (claimButton != null) {
            claimButton.onClick.RemoveListener(OnClaimButtonClicked);
            claimButton.onClick.AddListener(OnClaimButtonClicked);
        }
    }

    public void RefreshState() {
        currentStreak = PlayerPrefs.GetInt(PREF_STREAK, 0);
        string lastClaimStr = PlayerPrefs.GetString(PREF_LAST_CLAIM, "");

        DateTime today = DateTime.UtcNow.Date;

        if (string.IsNullOrEmpty(lastClaimStr)) {
            // First time ever - available!
            isClaimableToday = true;
        } else {
            DateTime lastDate;
            if (DateTime.TryParse(lastClaimStr, out lastDate)) {
                TimeSpan diff = today - lastDate.Date;
                if (diff.TotalDays >= 1) {
                    isClaimableToday = true;
                    // Reset cycle if completed full 7 days
                    if (currentStreak >= 7) {
                        currentStreak = 0;
                        PlayerPrefs.SetInt(PREF_STREAK, 0);
                    }
                } else {
                    isClaimableToday = false;
                }
            } else {
                isClaimableToday = true;
            }
        }

        UpdateUI();
    }

    void UpdateUI() {
        int nextDay = currentStreak + 1;
        if (nextDay > 7) nextDay = 1;

        if (claimButtonText != null) {
            if (isClaimableToday) {
                claimButtonText.text = "CLAIM (DAY " + nextDay + ")";
                if (claimButton != null) claimButton.interactable = true;
            } else {
                claimButtonText.text = "CLAIMED TODAY";
                if (claimButton != null) claimButton.interactable = true;
            }
        }

        // Highlight rows
        UpdateRowVisual(day1Row, 1);
        UpdateRowVisual(day3Row, 3);
        UpdateRowVisual(day7Row, 7);
    }

    void UpdateRowVisual(GameObject row, int dayTarget) {
        if (row == null) return;

        bool isClaimed = currentStreak >= dayTarget;
        bool isCurrentTarget = (currentStreak + 1) == dayTarget && isClaimableToday;

        Image bg = row.GetComponent<Image>();
        if (bg != null) {
            if (isClaimed) {
                bg.color = new Color(0.18f, 0.55f, 0.25f, 0.9f); // Green for claimed
            } else if (isCurrentTarget) {
                bg.color = new Color(0.2f, 0.5f, 0.9f, 0.9f); // Bright blue for target
            } else {
                bg.color = new Color(0.12f, 0.12f, 0.18f, 0.6f);
            }
        }

        // Adjust badge / text color
        Text[] txts = row.GetComponentsInChildren<Text>(true);
        foreach (var t in txts) {
            if (isClaimed) {
                t.color = new Color(0.7f, 1f, 0.7f);
            } else if (isCurrentTarget) {
                t.color = Color.white;
            }
        }
    }

    public void OnClaimButtonClicked() {
        if (!isClaimableToday) {
            // Already claimed today, clicking acts as close
            gameObject.SetActive(false);
            return;
        }

        ClaimReward();
    }

    public void ClaimReward() {
        currentStreak++;
        if (currentStreak > 7) currentStreak = 1;

        PlayerPrefs.SetInt(PREF_STREAK, currentStreak);
        PlayerPrefs.SetString(PREF_LAST_CLAIM, DateTime.UtcNow.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt(PREF_TOTAL_CLAIMS, PlayerPrefs.GetInt(PREF_TOTAL_CLAIMS, 0) + 1);
        PlayerPrefs.SetInt("DAILY_CLAIMED_DAY_" + currentStreak, 1);

        float planetHp = (LevelManager._instance != null && LevelManager._instance.currentPlanet != null) 
            ? LevelManager._instance.currentPlanet.maxHp 
            : 500f;

        switch (currentStreak) {
            case 1:
                float coinsDay1 = Mathf.Max(5000f, planetHp * 5f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay1);
                ShowFloatingReward("+" + CurrencyManager.GetSuffix((int)coinsDay1) + " COINS!");
                break;
            case 2:
                float coinsDay2 = Mathf.Max(10000f, planetHp * 8f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay2);
                ShowFloatingReward("+" + CurrencyManager.GetSuffix((int)coinsDay2) + " COINS!");
                break;
            case 3:
                // Day 3 unlocks Ship with id 3 (Golden Wings, +10% COINS)
                PlayerPrefs.SetInt("SHIP_UNLOCKED_3", 1);
                PlayerPrefs.SetInt("DAILY_CLAIMED_DAY_3", 1);
                float coinsDay3 = Mathf.Max(15000f, planetHp * 10f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay3);
                ShowFloatingReward("SPACESHIP UNLOCKED: GOLDEN WINGS (+10% COINS)!");
                break;
            case 4:
                float coinsDay4 = Mathf.Max(20000f, planetHp * 12f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay4);
                ShowFloatingReward("+" + CurrencyManager.GetSuffix((int)coinsDay4) + " COINS!");
                break;
            case 5:
                float coinsDay5 = Mathf.Max(25000f, planetHp * 15f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay5);
                ShowFloatingReward("+" + CurrencyManager.GetSuffix((int)coinsDay5) + " COINS!");
                break;
            case 6:
                float coinsDay6 = Mathf.Max(35000f, planetHp * 20f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay6);
                ShowFloatingReward("+" + CurrencyManager.GetSuffix((int)coinsDay6) + " COINS!");
                break;
            case 7:
                // Day 7 unlocks Ship with id 1 (Falcon F9, +15% CLICK DAMAGE)
                PlayerPrefs.SetInt("SHIP_UNLOCKED_1", 1);
                PlayerPrefs.SetInt("DAILY_CLAIMED_DAY_7", 1);
                float coinsDay7 = Mathf.Max(50000f, planetHp * 30f);
                if (CurrencyManager._instance != null) CurrencyManager._instance.AddCoins(coinsDay7);
                ShowFloatingReward("SPECIAL SHIP UNLOCKED: FALCON F9 (+15% CLICK DAMAGE)!");
                break;
        }

        // Refresh ShipShop immediately
        if (ShipShop._instance != null && ShipShop._instance.ships != null) {
            foreach (var ship in ShipShop._instance.ships) {
                if (ship != null) {
                    if (ship.CheckIfUnlocked()) {
                        ship.Unlock();
                    } else {
                        ship.Lock();
                    }
                }
            }
        }

        if (AudioManager.Instance != null && AudioManager.Instance.complete != null) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.complete);
        }

        isClaimableToday = false;
        PlayerPrefs.Save();
        UpdateUI();
    }

    void ShowFloatingReward(string msg) {
        if (ClickAnimation._instance != null) {
            var textAnim = ClickAnimation._instance.GetObjectFromPool();
            if (textAnim != null) {
                textAnim.SetActive(true);
                Vector3 pos = Camera.main != null ? Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 10f)) : Vector3.zero;
                textAnim.transform.position = pos;
                var txt = textAnim.GetComponent<Text>();
                if (txt != null) {
                    txt.text = msg;
                    txt.color = new Color(1f, 0.9f, 0.2f);
                }
            }
        }
    }
}
