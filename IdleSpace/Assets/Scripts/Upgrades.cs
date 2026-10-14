using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour {

    public static Upgrades _instance;

    CurrencyManager cm;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    public void RefreshStats() {
        Start();
    }

    void Start () {
        cm = CurrencyManager._instance;

        clickLevel = PlayerPrefs.GetInt("CLICK_LEVEL", 1);
        missileDamageLevel = PlayerPrefs.GetInt("MISSILE_DAMAGE_LEVEL", 1);
        missileSpawnRateLevel = PlayerPrefs.GetInt("MISSILE_SPAWN_RATE_LEVEL", 1);
        offlineEarningLevel = PlayerPrefs.GetInt("OFFLINE_EARNING_LEVEL", 1);
        shipFireRateLevel = PlayerPrefs.GetInt("SHIP_FIRE_RATE_LEVEL", 1);
        critLevel = PlayerPrefs.GetInt("CRIT_LEVEL", 1);

        SetMissileDamageStats();
        SetMissileSpawnRateStats();
        SetClickStats();
        SetOfflineEarningStats();
        SetShipFireRateStats();
        SetCritStats();
    }

    float GetPrestigeMultiplier() {
        if (PrestigeManager.Instance != null) {
            return PrestigeManager.Instance.GetTotalPrestigeMultiplier();
        }
        return 1f + (PlayerPrefs.GetInt("PRESTIGE_ORBS", 0) * 0.10f);
    }

    #region ClickUpgrade
    [Header("Click Upgrade")]
    public float moneyPerClick;
    public float damagePerClick;
    int clickLevel;
    public UpgradeObject clickUpgrade;

    public void UpgradeClicks() {
        if (cm.coins >= clickUpgrade.price) {
            cm.AddCoins(-clickUpgrade.price);
            clickLevel++;
            SetClickStats();
            PlayerPrefs.SetInt("CLICK_LEVEL", clickLevel);
        }
    }

    void SetClickStats() {
        clickUpgrade.price = GetPrice(30, clickLevel * 2.35f, 18, clickLevel);
        clickUpgrade.priceText.text = CurrencyManager.GetSuffix(clickUpgrade.price);

        moneyPerClick = clickLevel / (float)2;
        damagePerClick = Mathf.Max(1f, (1f + (clickLevel - 1) * 0.5f) * GetPrestigeMultiplier());
        clickUpgrade.levelText.text = "LVL " + clickLevel;
    }
    #endregion

    #region MissileDamage
    [Header("Missile Damage")]
    public float missileDamage;
    int missileDamageLevel;
    public UpgradeObject missileDamageUpgrade;

    public void UpgradeMissileDamage() {
        if (cm.coins >= missileDamageUpgrade.price) {
            cm.AddCoins(-missileDamageUpgrade.price);
            missileDamageLevel++;
            SetMissileDamageStats();
            PlayerPrefs.SetInt("MISSILE_DAMAGE_LEVEL", missileDamageLevel);
        }
    }

    void SetMissileDamageStats() {
        missileDamageUpgrade.price = GetPrice(20, missileDamageLevel * 2.5f, 18, missileDamageLevel);
        missileDamageUpgrade.priceText.text = CurrencyManager.GetSuffix(missileDamageUpgrade.price);

        missileDamage = (missileDamageLevel * 1f) * GetPrestigeMultiplier();
        missileDamageUpgrade.levelText.text = "LVL " + missileDamageLevel;
    }
    #endregion

    #region MissileSpawnRate
    [Header("Missile Spawn Rate")]
    public float secondsToSpawnMissile;
    int missileSpawnRateLevel;
    public UpgradeObject missileSpawnRateUpgrade;

    public void UpgradeMissileSpawnRate() {
        if (cm.coins >= missileSpawnRateUpgrade.price) {
            cm.AddCoins(-missileSpawnRateUpgrade.price);
            missileSpawnRateLevel++;
            SetMissileSpawnRateStats();
            PlayerPrefs.SetInt("MISSILE_SPAWN_RATE_LEVEL", missileSpawnRateLevel);
        }
    }

    void SetMissileSpawnRateStats() {
        missileSpawnRateUpgrade.price = GetPrice(30, missileSpawnRateLevel * 2.5f, 16, missileSpawnRateLevel);
        missileSpawnRateUpgrade.priceText.text = CurrencyManager.GetSuffix(missileSpawnRateUpgrade.price);

        secondsToSpawnMissile = Mathf.Max(2.2f - missileSpawnRateLevel * .04f, .4f);
        missileSpawnRateUpgrade.levelText.text = "LVL " + missileSpawnRateLevel;
    }
    #endregion

    #region OfflineEarning
    [Header("Offline Earning")]
    public float offlinePercent;
    int offlineEarningLevel;
    public UpgradeObject offlineUpgrade;

    public void UpgradeOfflineEarning() {
        if (cm.coins >= offlineUpgrade.price) {
            cm.AddCoins(-offlineUpgrade.price);
            offlineEarningLevel++;
            SetOfflineEarningStats();
            PlayerPrefs.SetInt("OFFLINE_EARNING_LEVEL", offlineEarningLevel);
        }
    }

    void SetOfflineEarningStats() {
        offlineUpgrade.price = GetPrice(70, missileSpawnRateLevel * 2, 16, offlineEarningLevel);
        offlineUpgrade.priceText.text = CurrencyManager.GetSuffix(offlineUpgrade.price);

        offlinePercent = 1 + offlineEarningLevel;
        offlineUpgrade.levelText.text = "LVL " + offlineEarningLevel;
    }
    #endregion

    #region ShipFireRate
    [Header("Ship Fire Rate")]
    public float secondsToShoot;
    int shipFireRateLevel;
    public UpgradeObject shipFireRateUpgrade;

    public void UpgradeShipFireRate() {
        if (cm.coins >= shipFireRateUpgrade.price) {
            cm.AddCoins(-shipFireRateUpgrade.price);
            shipFireRateLevel++;
            SetShipFireRateStats();
            PlayerPrefs.SetInt("SHIP_FIRE_RATE_LEVEL", shipFireRateLevel);
        }
    }

    void SetShipFireRateStats() {
        shipFireRateUpgrade.price = GetPrice(75, shipFireRateLevel * 4.0f, 27, shipFireRateLevel);
        shipFireRateUpgrade.priceText.text = CurrencyManager.GetSuffix(shipFireRateUpgrade.price);

        secondsToShoot = Mathf.Max(2.5f - shipFireRateLevel * .05f, .2f);
        shipFireRateUpgrade.levelText.text = "LVL " + shipFireRateLevel;
    }
    #endregion

    #region CriticalStrike
    [Header("Critical Strike (New Upgrade)")]
    public float critChance = 0.08f;
    public float critMultiplier = 2.5f;
    public int critLevel = 1;
    public UpgradeObject critUpgrade;

    public void UpgradeCriticalStrike() {
        if (critUpgrade != null && cm.coins >= critUpgrade.price) {
            cm.AddCoins(-critUpgrade.price);
            critLevel++;
            SetCritStats();
            PlayerPrefs.SetInt("CRIT_LEVEL", critLevel);
        }
    }

    void SetCritStats() {
        critLevel = PlayerPrefs.GetInt("CRIT_LEVEL", 1);
        critChance = Mathf.Min(0.06f + (critLevel - 1) * 0.02f, 0.50f);
        critMultiplier = 2.0f + (critLevel * 0.25f);

        if (critUpgrade != null) {
            critUpgrade.price = GetPrice(120, critLevel * 3.5f, 25, critLevel);
            critUpgrade.priceText.text = CurrencyManager.GetSuffix(critUpgrade.price);
            critUpgrade.levelText.text = "LVL " + critLevel;
        }
    }

    public bool RollCritical(out float multiplier) {
        if (Random.value < critChance) {
            multiplier = critMultiplier;
            return true;
        }
        multiplier = 1f;
        return false;
    }
    #endregion

    int GetPrice(int basePrice, float scaleFactor, int baseGrowAmount, int level) {
        float price = basePrice;
        for (int i = 1; i <= level; i++) {
            price += baseGrowAmount + scaleFactor;
        }

        return (int)price;
    }
}

