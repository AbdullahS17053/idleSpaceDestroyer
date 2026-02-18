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
        ships[0].Unlock();
        ships[0].OnClick(false);

		foreach (ShipObject sO in ships) {
            if (sO.CheckIfUnlocked()) {
                sO.GetComponent<Button>().interactable = false;

                sO.Unlock();

                if (sO.id == PlayerPrefs.GetInt("CURRENT_SHIP")) {
                    sO.OnClick();
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
