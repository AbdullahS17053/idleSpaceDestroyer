using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPlanetObject : MonoBehaviour {

    public int levelToUnlock;

    void Start() {
        SyncLevelUI();
    }

    private void OnEnable() {
        SyncLevelUI();
    }

    public void SyncLevelUI() {
        // Automatically sync the circle badge text to match levelToUnlock
        Text levelText = null;
        if (transform.childCount > 1) {
            levelText = transform.GetChild(1).GetComponentInChildren<Text>(true);
        }
        if (levelText == null) {
            levelText = GetComponentInChildren<Text>(true);
        }

        if (levelText != null && levelToUnlock > 0) {
            levelText.text = levelToUnlock.ToString();
        }

        if (LevelManager._instance != null && levelToUnlock <= LevelManager._instance.currentLevel) {
            if (transform.childCount > 0)
                transform.GetChild(0).gameObject.SetActive(false); // Disabling question mark
            
            var img = GetComponent<Image>();
            if (img != null)
                img.color = Color.white;
        }
    }
}