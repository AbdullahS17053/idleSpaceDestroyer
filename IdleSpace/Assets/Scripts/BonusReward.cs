using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusReward : MonoBehaviour {

    public GameObject rewardWindow;
    public Text rewardText;

    public GameObject[] rewardShips;
    GameObject currentRewardShip;

    float coinsToAdd;
    float timer;

    public GameObject dailyRewardObject; //Showing daily reward window after 15secs
    float dailyObjectTimer;

    // Use this for initialization
    void Start() {
        PlayerPrefs.SetInt("LAUNCH_AMOUNT", PlayerPrefs.GetInt("LAUNCH_AMOUNT") + 1);
        if (dailyRewardObject != null && dailyRewardObject.GetComponent<DailyRewardManager>() == null) {
            dailyRewardObject.AddComponent<DailyRewardManager>();
        }
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        dailyObjectTimer += Time.deltaTime;

        if (timer > 50) {
            currentRewardShip = Instantiate(rewardShips[Random.Range(0, rewardShips.Length)], transform);
            currentRewardShip.SetActive(true);

            timer = 0;
        }

        if (dailyObjectTimer > 15) {
            if (PlayerPrefs.GetInt("LAUNCH_AMOUNT") <= 1)
                dailyRewardObject.SetActive(true);
            dailyObjectTimer = -999999;
        }
    }

    public void OpenRewardWindow() {
        rewardWindow.SetActive(true);

        coinsToAdd = LevelManager._instance.currentPlanet.maxHp / 6;

        rewardText.text = CurrencyManager.GetSuffix((int)coinsToAdd);

        Destroy(currentRewardShip);
    }

    public void CollectMoney() {
        CurrencyManager._instance.AddCoins((int)coinsToAdd);
    }
}
