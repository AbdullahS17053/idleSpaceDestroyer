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
        if (coinTexts == null) return;

        for (int i = 0; i < coinTexts.Length; i++) {
            if (coinTexts[i] != null && coinTexts[i].transform.parent != null) {
                // Only update the buy button text inside this specific coin package card,
                // never touch other UI elements under Shop such as ShipShop or default spaceship.
                Text[] texts = coinTexts[i].transform.parent.GetComponentsInChildren<Text>(true);
                foreach (var t in texts) {
                    if (t != null && t != coinTexts[i]) {
                        t.text = (i < prices.Length) ? prices[i] : "$11.99";
                    }
                }
            }
        }
    }

    public void BuyCoinSuccessful(int id) {
        if (LevelManager._instance != null && LevelManager._instance.currentPlanet != null && CurrencyManager._instance != null) {
            CurrencyManager._instance.AddCoins((LevelManager._instance.currentPlanet.maxHp * 3 * multipliers[id]));
        }
    }
}
