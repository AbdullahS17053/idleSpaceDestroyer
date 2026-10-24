using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipObject : MonoBehaviour {

    public int id;

    public Text bonusText;
    public string bonusString;

    public ShipShop.ShipUnlockCondition condition;
    public int conditionValue;

    public Image shipImage;
    public GameObject questionMark;

    public GameObject bulletPrefab;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    public bool CheckIfUnlocked() {
        // Default ship (id 0) is always unlocked
        if (id == 0) return true;

        // Check if unlocked by daily reward claim flag
        if (PlayerPrefs.GetInt("SHIP_UNLOCKED_" + id, 0) == 1)
            return true;

        switch (condition) {
            case ShipShop.ShipUnlockCondition.level:
                if (LevelManager._instance != null && LevelManager._instance.currentLevel >= conditionValue)
                    return true;
                break;
            case ShipShop.ShipUnlockCondition.damage:
                if (PlayerPrefs.GetFloat("ALL_DAMAGE", 0f) >= conditionValue)
                    return true;
                break;
            case ShipShop.ShipUnlockCondition.day:
                // Unlocks when the player reaches or claims the required daily login day
                int streak = PlayerPrefs.GetInt("DAILY_REWARD_CURRENT_STREAK", 0);
                int totalClaims = PlayerPrefs.GetInt("DAILY_REWARD_TOTAL_CLAIMS", 0);
                bool claimedDay = PlayerPrefs.GetInt("DAILY_CLAIMED_DAY_" + conditionValue, 0) == 1;

                if (claimedDay || streak >= conditionValue || totalClaims >= conditionValue)
                    return true;

                break;
        }

        return false;
    }

    public void Unlock() {
        var btn = GetComponent<Button>();
        if (btn != null) btn.interactable = true;
        if (bonusText != null) bonusText.text = bonusString;

        if (questionMark != null)
            questionMark.SetActive(false);
        if (shipImage != null)
            shipImage.color = Color.white;
    }

    public void Lock() {
        if (id == 0) {
            Unlock();
            return;
        }

        var btn = GetComponent<Button>();
        if (btn != null) btn.interactable = false;

        if (questionMark != null)
            questionMark.SetActive(true);
        if (shipImage != null)
            shipImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        if (bonusText != null) {
            if (condition == ShipShop.ShipUnlockCondition.damage) {
                bonusText.text = "DEAL " + CurrencyManager.GetSuffix((int)Mathf.Max(0, conditionValue - PlayerPrefs.GetFloat("ALL_DAMAGE", 0f))) + " DAMAGE";
            } else if (condition == ShipShop.ShipUnlockCondition.level) {
                bonusText.text = "UNLOCK LVL " + conditionValue;
            } else if (condition == ShipShop.ShipUnlockCondition.day) {
                bonusText.text = "LOGIN ON " + conditionValue + " DAYS";
            }
        }
    }

    /// <summary>
    /// Every time it gets enabled check if the object is unlocked
    /// </summary>
    private void OnEnable() {
        if (CheckIfUnlocked()) {
            Unlock();
        } else {
            Lock();
        }
    }

    /// <summary>
    /// Clicking on a ship object in the shop
    /// </summary>
    public void OnClick(bool overrideSave = true) {
        if (Ship._instance != null && shipImage != null)
            Ship._instance.GetComponent<Image>().sprite = shipImage.sprite;

        if (overrideSave)
            PlayerPrefs.SetInt("CURRENT_SHIP", id);

        // Setting the current ship object background color to a different one
        if (ShipShop._instance != null && ShipShop._instance.ships != null) {
            foreach (ShipObject sO in ShipShop._instance.ships) {
                if (sO != null) {
                    var img = sO.GetComponent<Image>();
                    if (img != null) img.color = Color.white;
                }
            }
        }
        var currentImg = GetComponent<Image>();
        if (currentImg != null)
            currentImg.color = new Color(95f / 255f, 230f / 255f, 1f);

        if (ShipShop._instance != null)
            ShipShop._instance.currentShip = this;
    }
}
