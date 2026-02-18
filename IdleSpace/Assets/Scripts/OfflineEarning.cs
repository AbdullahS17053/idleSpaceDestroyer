using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class OfflineEarning : MonoBehaviour {

    public static OfflineEarning _instance;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(GetDay());
	}

    public Text offlineIncomeText;
    float coinsToAdd;

    public TimeSpan allTimeSinceFirstTime;
    /// <summary>
    /// Getting offline reward
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDay() {
        //WWW www = new WWW("http://leatonm.net/wp-content/uploads/2017/candlepin/getdate.php");
        yield return null;

        //DateTime currentDateTime = DateTime.Parse(www.text.Replace('/', ' '));
        DateTime currentDateTime = DateTime.Now;


        if (!PlayerPrefs.HasKey("FIRST_LOGIN_TIME")) {
            PlayerPrefs.SetString("FIRST_LOGIN_TIME", currentDateTime.ToString());
        }

        if (!PlayerPrefs.HasKey("OLD_LOGIN_TIME")) {
            PlayerPrefs.SetString("OLD_LOGIN_TIME", currentDateTime.ToString());
            yield break;
        }


        DateTime oldDate = DateTime.Parse(PlayerPrefs.GetString("OLD_LOGIN_TIME"));
        DateTime firstDate = DateTime.Parse(PlayerPrefs.GetString("FIRST_LOGIN_TIME"));

        TimeSpan sub = currentDateTime - oldDate;
        allTimeSinceFirstTime = currentDateTime - firstDate;

        

        if (sub.TotalMinutes < 3) yield break;

        offlineIncomeText.transform.parent.parent.parent.gameObject.SetActive(true);

        coinsToAdd = Mathf.Min((int)sub.TotalMinutes, 24 * 60) * LevelManager._instance.currentPlanet.maxHp * 5 / (24 * 60) * (.85f + Upgrades._instance.offlinePercent / 100f);
        CurrencyManager._instance.AddCoins(coinsToAdd);

        offlineIncomeText.text = CurrencyManager.GetSuffix((int)coinsToAdd);
        Debug.Log("OFFLINE REWARD " + (int)sub.TotalMinutes);

        PlayerPrefs.SetString("OLD_LOGIN_TIME", currentDateTime.ToString());
    }

    public void DoubleEarnings() {
        ShowOptions sO = new ShowOptions();
        sO.resultCallback = rewardedCallback;
        Advertisement.Show("rewardedVideo", sO);
    }

    private void rewardedCallback(ShowResult result) {
        if (result == ShowResult.Finished) {
            CurrencyManager._instance.AddCoins(coinsToAdd);
        }
    }
}
