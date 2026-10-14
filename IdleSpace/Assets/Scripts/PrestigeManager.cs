using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeManager : MonoBehaviour {

    public static PrestigeManager Instance;

    [Header("Prestige Settings")]
    public int minLevelToPrestige = 3; // Set to 3 for quick testing
    public float damageBonusPerOrb = 0.10f; // +10% base damage per unspent orb

    [Header("UI References")]
    public GameObject prestigePanel;
    public Text currentOrbsText;
    public Text orbsToGainText;
    public Text bonusMultiplierText;
    public Button prestigeButton;
    public Text statusMessageText;

    [Header("Cosmic Tech Upgrades")]
    public Text cosmicPowerText;
    public Button cosmicPowerButton;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        UpdateUI();
    }

    void OnEnable() {
        UpdateUI();
    }

    public int GetCurrentOrbs() {
        return PlayerPrefs.GetInt("PRESTIGE_ORBS", 0);
    }

    public int CalculateOrbsToGain() {
        int currentLevel = LevelManager._instance != null ? LevelManager._instance.currentLevel : 1;
        if (currentLevel < minLevelToPrestige) return 0;
        return Mathf.Max(1, (currentLevel - minLevelToPrestige + 1) * 2);
    }

    public float GetTotalPrestigeMultiplier() {
        int cosmicPowerLevel = PlayerPrefs.GetInt("PRESTIGE_COSMIC_POWER", 0);
        // Base bonus: +10% per orb, plus +25% per Cosmic Power upgrade
        return 1f + (GetCurrentOrbs() * damageBonusPerOrb) + (cosmicPowerLevel * 0.25f);
    }

    public void DoPrestige() {
        int orbsToGain = CalculateOrbsToGain();
        int currentLevel = LevelManager._instance != null ? LevelManager._instance.currentLevel : 1;

        if (orbsToGain <= 0) {
            Debug.LogWarning("Cannot rebirth yet! Must reach Level " + minLevelToPrestige + " (Current: " + currentLevel + ")");
            if (statusMessageText != null) {
                statusMessageText.text = "REACH LEVEL " + minLevelToPrestige + " TO REBIRTH!";
                statusMessageText.color = Color.red;
            }
            return;
        }

        int totalOrbs = GetCurrentOrbs() + orbsToGain;
        int prestigeCount = PlayerPrefs.GetInt("PRESTIGE_COUNT", 0) + 1;

        PlayerPrefs.SetInt("PRESTIGE_ORBS", totalOrbs);
        PlayerPrefs.SetInt("PRESTIGE_COUNT", prestigeCount);

        // Reset level to 1
        PlayerPrefs.SetInt("CURRENT_LEVEL", 1);
        if (LevelManager._instance != null) {
            LevelManager._instance.currentLevel = 1;
            LevelManager._instance.CreateNewPlanet();
        }

        // Reset base upgrades for new rebirth run
        PlayerPrefs.SetInt("CLICK_LEVEL", 1);
        PlayerPrefs.SetInt("MISSILE_DAMAGE_LEVEL", 1);
        PlayerPrefs.SetInt("MISSILE_SPAWN_RATE_LEVEL", 1);
        PlayerPrefs.SetInt("SHIP_FIRE_RATE_LEVEL", 1);
        PlayerPrefs.SetInt("CRIT_LEVEL", 1);

        // Reset coins
        PlayerPrefs.SetInt("COINS", 50);
        if (CurrencyManager._instance != null) {
            CurrencyManager._instance.coins = 50;
        }

        PlayerPrefs.Save();

        // Refresh upgrades with new prestige multiplier
        if (Upgrades._instance != null) {
            Upgrades._instance.RefreshStats();
        }

        if (AudioManager.Instance != null && AudioManager.Instance.complete != null) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.complete);
        }

        Debug.Log("Rebirth successful! Awarded +" + orbsToGain + " Orbs. Total Orbs: " + totalOrbs);

        UpdateUI();

        if (prestigePanel != null)
            prestigePanel.SetActive(false);
    }

    // ---------------- Cosmic Tech Upgrades ----------------
    public void BuyCosmicPower() {
        int currentPower = PlayerPrefs.GetInt("PRESTIGE_COSMIC_POWER", 0);
        int cost = (currentPower + 1) * 2;

        if (GetCurrentOrbs() >= cost) {
            PlayerPrefs.SetInt("PRESTIGE_ORBS", GetCurrentOrbs() - cost);
            PlayerPrefs.SetInt("PRESTIGE_COSMIC_POWER", currentPower + 1);
            PlayerPrefs.Save();
            UpdateUI();
        }
    }

    public void OpenPrestigePanel() {
        if (prestigePanel != null) {
            prestigePanel.SetActive(true);
            UpdateUI();
        }
    }

    public void ClosePrestigePanel() {
        if (prestigePanel != null)
            prestigePanel.SetActive(false);
    }

    public void UpdateUI() {
        int currentOrbs = GetCurrentOrbs();
        int toGain = CalculateOrbsToGain();
        float multiplier = GetTotalPrestigeMultiplier();
        int currentLevel = LevelManager._instance != null ? LevelManager._instance.currentLevel : 1;
        int cosmicPower = PlayerPrefs.GetInt("PRESTIGE_COSMIC_POWER", 0);
        int powerCost = (cosmicPower + 1) * 2;

        if (currentOrbsText != null)
            currentOrbsText.text = "COSMIC ORBS: " + currentOrbs;

        if (orbsToGainText != null) {
            if (toGain > 0)
                orbsToGainText.text = "REBIRTH FOR +" + toGain + " ORBS";
            else
                orbsToGainText.text = "UNLOCKS AT LVL " + minLevelToPrestige + " (NOW: " + currentLevel + ")";
        }

        if (bonusMultiplierText != null)
            bonusMultiplierText.text = "PERMANENT DMG: +" + ((multiplier - 1f) * 100f).ToString("F0") + "%";

        if (statusMessageText != null && toGain > 0) {
            statusMessageText.text = "READY FOR REBIRTH!";
            statusMessageText.color = Color.green;
        }

        if (cosmicPowerText != null) {
            cosmicPowerText.text = "COSMIC POWER (LVL " + cosmicPower + ")\nCost: " + powerCost + " Orbs (+25% DMG)";
        }
        if (cosmicPowerButton != null) {
            cosmicPowerButton.interactable = currentOrbs >= powerCost;
        }

        if (prestigeButton != null) {
            prestigeButton.interactable = toGain > 0;
        }
    }
}
