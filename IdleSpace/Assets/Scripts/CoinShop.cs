using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinShop : MonoBehaviour {

    public static CoinShop _instance;

    public int[] multipliers;
    public Text[] coinTexts;

    private void Awake() {
        _instance = this;
    }

    void Start () {
        UpdatePriceLabels();
    }

    private void OnEnable() {
        SetTexts();
    }

    public void SetTexts() {
        for (int i = 0; i < coinTexts.Length; i++) {
            if (coinTexts[i] != null && LevelManager._instance != null && LevelManager._instance.currentPlanet != null) {
                coinTexts[i].text = CurrencyManager.GetSuffix((int)(LevelManager._instance.currentPlanet.maxHp * 3 * multipliers[i])) + " COINS";
            }
        }
        UpdatePriceLabels();
    }

    public void UpdatePriceLabels() {
        string[] prices = new string[] { "$0.99", "$2.99", "$4.99", "$9.99" };
        var texts = GetComponentsInChildren<Text>(true);
        int idx = 0;
        foreach (var t in texts) {
            if (t != null && (t.text.Trim() == "345" || t.text.Trim() == "")) {
                t.text = (idx < prices.Length) ? prices[idx] : "$11.99";
                idx++;
            }
        }
    }

    public void BuyCoinSuccessful(int id) {
        if (LevelManager._instance != null && LevelManager._instance.currentPlanet != null && CurrencyManager._instance != null) {
            CurrencyManager._instance.AddCoins((LevelManager._instance.currentPlanet.maxHp * 3 * multipliers[id]));
        }
    }
}