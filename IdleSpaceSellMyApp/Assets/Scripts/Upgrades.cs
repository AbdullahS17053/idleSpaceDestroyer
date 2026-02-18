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
    void Start () {
        cm = CurrencyManager._instance;

        clickLevel = PlayerPrefs.GetInt("CLICK_LEVEL", 1);
        missileDamageLevel = PlayerPrefs.GetInt("MISSILE_DAMAGE_LEVEL", 1);
        missileSpawnRateLevel = PlayerPrefs.GetInt("MISSILE_SPAWN_RATE_LEVEL", 1);
        offlineEarningLevel = PlayerPrefs.GetInt("OFFLINE_EARNING_LEVEL", 1);
        shipFireRateLevel = PlayerPrefs.GetInt("SHIP_FIRE_RATE_LEVEL", 1);

        SetMissileDamageStats();
        SetMissileSpawnRateStats();
        SetClickStats();
        SetOfflineEarningStats();
        SetShipFireRateStats();
    }

    // Update is called once per frame
    void Update () {
		
	}

    #region ClickUpgrade
    [Header("Click Upgrade")]
    public float moneyPerClick;
    public float damagePerClick;
    int clickLevel;
    public UpgradeObject clickUpgrade;

    public void UpgradeClicks() {
        if (cm.coins >= clickUpgrade.price) {
            cm.AddCoins(-clickUpgrade.price); //Decreasing money
            clickLevel++;

            SetClickStats();

            PlayerPrefs.SetInt("CLICK_LEVEL", clickLevel);
        }
    }

    void SetClickStats() {
        clickUpgrade.price = GetPrice(30, clickLevel * 2.35f, 18, clickLevel);
        clickUpgrade.priceText.text = CurrencyManager.GetSuffix(clickUpgrade.price);

        moneyPerClick = clickLevel / (float)2;
        damagePerClick = .5f + clickLevel / (float)4;
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
            cm.AddCoins(-missileDamageUpgrade.price); //Decreasing money
            missileDamageLevel++;

            SetMissileDamageStats();

            PlayerPrefs.SetInt("MISSILE_DAMAGE_LEVEL", missileDamageLevel);
        }
    }

    void SetMissileDamageStats() {
        missileDamageUpgrade.price = GetPrice(20, missileDamageLevel * 2.5f, 18, missileDamageLevel); ;
        missileDamageUpgrade.priceText.text = CurrencyManager.GetSuffix(missileDamageUpgrade.price);

        missileDamage = missileDamageLevel * 1f;
        missileDamageUpgrade.levelText.text = "LVL " + missileDamageLevel;
    }

    #endregion

    #region MissileSpawnRate
    [Header("Missile Spawn Rate")]
    public float secondsToSpawnMissile; //How many seconds / missiles
    int missileSpawnRateLevel;
    public UpgradeObject missileSpawnRateUpgrade;

    public void UpgradeMissileSpawnRate() {
        if (cm.coins >= missileSpawnRateUpgrade.price) {
            cm.AddCoins(-missileSpawnRateUpgrade.price); //Decreasing money
            missileSpawnRateLevel++;

            SetMissileSpawnRateStats();

            PlayerPrefs.SetInt("MISSILE_SPAWN_RATE_LEVEL", missileSpawnRateLevel);
        }
    }

    void SetMissileSpawnRateStats() {
        missileSpawnRateUpgrade.price = GetPrice(30, missileSpawnRateLevel * 2.5f, 16, missileSpawnRateLevel); ;
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
            cm.AddCoins(-offlineUpgrade.price); //Decreasing money
            offlineEarningLevel++;

            SetOfflineEarningStats();

            PlayerPrefs.SetInt("OFFLINE_EARNING_LEVEL", offlineEarningLevel);
        }
    }

    void SetOfflineEarningStats() {
        offlineUpgrade.price = GetPrice(70, missileSpawnRateLevel * 2, 16, offlineEarningLevel); ;
        offlineUpgrade.priceText.text = CurrencyManager.GetSuffix(offlineUpgrade.price);

        offlinePercent = 1 + offlineEarningLevel;
        offlineUpgrade.levelText.text = "LVL " + offlineEarningLevel;
    }

    #endregion

    #region ShipFireRate
    [Header("Ship Fire Rate")]
    public float secondsToShoot; //How many seconds / missiles
    int shipFireRateLevel;
    public UpgradeObject shipFireRateUpgrade;

    public void UpgradeShipFireRate() {
        if (cm.coins >= shipFireRateUpgrade.price) {
            cm.AddCoins(-shipFireRateUpgrade.price); //Decreasing money
            shipFireRateLevel++;

            SetShipFireRateStats();

            PlayerPrefs.SetInt("SHIP_FIRE_RATE_LEVEL", shipFireRateLevel);
        }
    }

    void SetShipFireRateStats() {
        shipFireRateUpgrade.price = GetPrice(75, shipFireRateLevel * 4.0f, 27, shipFireRateLevel); ;
        shipFireRateUpgrade.priceText.text = CurrencyManager.GetSuffix(shipFireRateUpgrade.price);

        secondsToShoot = Mathf.Max(2.5f - shipFireRateLevel * .05f, .2f);
        shipFireRateUpgrade.levelText.text = "LVL " + shipFireRateLevel;
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
