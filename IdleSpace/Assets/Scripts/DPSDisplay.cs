using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DPSDisplay : MonoBehaviour {

    public static DPSDisplay Instance;

    [Header("UI Text References")]
    public Text dpsText;
    public Text totalDamageText;

    private float recentClickDamage = 0f;
    private float clickTimer = 0f;
    private float rollingClickDPS = 0f;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        // Auto-detect UI Text components if not linked in Inspector
        if (dpsText == null) {
            var found = transform.Find("DPSText");
            if (found != null) dpsText = found.GetComponent<Text>();
        }
        if (totalDamageText == null) {
            var found = transform.Find("TotalDamageText");
            if (found != null) totalDamageText = found.GetComponent<Text>();
        }

        UpdateDisplay();
    }

    /// <summary>
    /// Register direct clicks / bullet hits from player action
    /// </summary>
    public void RegisterDamage(float damage) {
        recentClickDamage += damage;

        // Keep all-time damage continuously updated for ship unlocks
        float currentAllDamage = PlayerPrefs.GetFloat("ALL_DAMAGE", 0f);
        PlayerPrefs.SetFloat("ALL_DAMAGE", currentAllDamage + damage);
    }

    void Update() {
        clickTimer += Time.deltaTime;
        if (clickTimer >= 1.0f) {
            rollingClickDPS = recentClickDamage / clickTimer;
            recentClickDamage = 0f;
            clickTimer = 0f;
        }

        UpdateDisplay();
    }

    /// <summary>
    /// Accurate Idle Game DPS Logic:
    /// Combines automated Ship Fire Rate + Missile Spawn Rate + Active Boost + Click DPS
    /// </summary>
    public float CalculateTotalDPS() {
        float totalDPS = 0f;

        if (Upgrades._instance != null) {
            float bulletDmg = Upgrades._instance.damagePerClick;
            float missileDmg = Upgrades._instance.missileDamage;

            // 1. Ship Auto-Shoot DPS
            if (Ship._instance != null && Ship._instance.IsAutoShootBoostActive) {
                // Boosted rapid auto-fire (every 0.08s)
                totalDPS += (bulletDmg / 0.08f);
            } else {
                float fireInterval = Mathf.Max(0.05f, Upgrades._instance.secondsToShoot);
                totalDPS += (bulletDmg / fireInterval);
            }

            // 2. Automated Missiles DPS
            float missileInterval = Mathf.Max(0.1f, Upgrades._instance.secondsToSpawnMissile);
            totalDPS += (missileDmg / missileInterval);
        }

        // 3. Player Active Clicking DPS
        totalDPS += rollingClickDPS;

        return totalDPS;
    }

    void UpdateDisplay() {
        float currentDps = CalculateTotalDPS();
        float allDamage = PlayerPrefs.GetFloat("ALL_DAMAGE", 0f);

        if (dpsText != null) {
            dpsText.text = "⚡ DPS: " + CurrencyManager.GetSuffix((int)currentDps);
        }

        if (totalDamageText != null) {
            totalDamageText.text = "💥 DMG: " + CurrencyManager.GetSuffix((int)allDamage);
        }
    }

    public float GetCurrentDPS() => CalculateTotalDPS();
    public float GetTotalDamage() => PlayerPrefs.GetFloat("ALL_DAMAGE", 0f);
}
