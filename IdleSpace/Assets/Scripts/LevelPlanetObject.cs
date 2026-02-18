using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPlanetObject : MonoBehaviour {

    public int levelToUnlock;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable() {
        if (levelToUnlock <= LevelManager._instance.currentLevel) {
            transform.GetChild(0).gameObject.SetActive(false); //Disabling question mark
            GetComponent<Image>().color = Color.white;
        }
    }
}
