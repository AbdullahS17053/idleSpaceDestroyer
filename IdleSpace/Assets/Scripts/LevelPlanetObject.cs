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
        int currentLevel = LevelManager._instance != null ? LevelManager._instance.currentLevel : PlayerPrefs.GetInt("CURRENT_LEVEL", 1);

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

        GameObject questionMark = transform.childCount > 0 ? transform.GetChild(0).gameObject : null;
        Image img = GetComponent<Image>();

        if (levelToUnlock <= currentLevel) {
            // Unlocked planet
            if (questionMark != null) questionMark.SetActive(false);
            if (img != null) img.color = Color.white;
            if (levelText != null) {
                if (levelToUnlock == currentLevel) {
                    levelText.color = new Color(1f, 0.88f, 0.15f); // Current active planet (gold)
                } else {
                    levelText.color = new Color(0.4f, 1f, 0.4f); // Conquered planet (green)
                }
            }
        } else {
            // Locked planet
            if (questionMark != null) questionMark.SetActive(true);
            if (img != null) img.color = new Color(0.25f, 0.25f, 0.35f, 0.8f);
            if (levelText != null) levelText.color = Color.white;
        }
    }
}