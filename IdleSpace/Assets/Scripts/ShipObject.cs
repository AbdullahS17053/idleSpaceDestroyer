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
        switch (condition) {
            case ShipShop.ShipUnlockCondition.level:
                if (LevelManager._instance.currentLevel >= conditionValue)
                    return true;
                break;
            case ShipShop.ShipUnlockCondition.damage:
                if (PlayerPrefs.GetFloat("ALL_DAMAGE") >= conditionValue)
                    return true;
                break;
            case ShipShop.ShipUnlockCondition.day:
                if (OfflineEarning._instance.allTimeSinceFirstTime.Hours + 12 >= conditionValue * 24)
                    return true;
                break;
        }

        return false;
    }

    public void Unlock() {
        GetComponent<Button>().interactable = true;
        bonusText.text = bonusString;

        if (questionMark != null)
            questionMark.SetActive(false);
        shipImage.color = Color.white;
    }

    /// <summary>
    /// Every time it gets enabled check if the object is unlocked
    /// </summary>
    private void OnEnable() {
        if (CheckIfUnlocked()) {
            Unlock();
        } else {
            if (condition == ShipShop.ShipUnlockCondition.damage) {
                bonusText.text = "DEAL " + CurrencyManager.GetSuffix((int)(conditionValue - PlayerPrefs.GetFloat("ALL_DAMAGE"))) + " DAMAGE";
            }
        }
    }

    /// <summary>
    /// Clicking on a ship object in the shop
    /// </summary>
    public void OnClick(bool overrideSave = true) {
        Ship._instance.GetComponent<Image>().sprite = shipImage.sprite;

        if (overrideSave)
            PlayerPrefs.SetInt("CURRENT_SHIP", id);

        //Setting the current's ship object background color to a different one
        foreach (ShipObject sO in ShipShop._instance.ships) {
            sO.GetComponent<Image>().color = Color.white;
        }
        GetComponent<Image>().color = new Color(95f / 255f, 230f / 255f, 1);

        ShipShop._instance.currentShip = this;
    }
}
