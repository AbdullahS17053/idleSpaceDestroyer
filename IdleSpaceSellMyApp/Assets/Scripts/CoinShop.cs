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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable() {
        SetTexts();
    }

    public void SetTexts() {
        for (int i = 0; i < coinTexts.Length; i++) {
            coinTexts[i].text = CurrencyManager.GetSuffix((int)(LevelManager._instance.currentPlanet.maxHp * 3 * multipliers[i])) + " COINS";
        }
    }

    public void BuyCoinSuccessful(int id) {
        CurrencyManager._instance.AddCoins((LevelManager._instance.currentPlanet.maxHp * 3 * multipliers[id]));
    }
}
