using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipShop : MonoBehaviour {

    public static ShipShop _instance;
    public enum ShipUnlockCondition { level, damage, day};

    public ShipObject[] ships;
    public ShipObject currentShip;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        int selectedShipId = PlayerPrefs.GetInt("CURRENT_SHIP", 0);
        ShipObject shipToSelect = null;

        if (ships != null) {
            foreach (ShipObject sO in ships) {
                if (sO == null) continue;

                if (sO.CheckIfUnlocked()) {
                    sO.Unlock();

                    if (sO.id == selectedShipId) {
                        shipToSelect = sO;
                    }
                } else {
                    sO.Lock();
                }
            }
        }

        // Always ensure default ship (ships[0]) is unlocked and has clean bonus text
        if (ships != null && ships.Length > 0 && ships[0] != null) {
            ships[0].Unlock();
            if (ships[0].bonusText != null) {
                ships[0].bonusText.text = ships[0].bonusString;
            }
            if (shipToSelect == null) {
                shipToSelect = ships[0];
            }
        }

        if (shipToSelect != null) {
            shipToSelect.OnClick(false);
        }
    }
}
