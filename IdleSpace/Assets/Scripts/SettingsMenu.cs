using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    [Header("UI Panels")]
    public GameObject settingsPanel;

    [Header("Toggle Buttons & Texts")]
    public Button musicToggleButton;
    public Text musicText;
    public Button sfxToggleButton;
    public Text sfxText;

    private float lastMusicToggleTime = 0f;
    private float lastSFXToggleTime = 0f;

    void Start() {
        // Persistent listeners are already configured in GameScene.unity on the buttons.
        // We only call UpdateUI here.
        UpdateUI();
    }

    public void OpenSettings() {
        if (settingsPanel != null) {
            settingsPanel.SetActive(true);
            UpdateUI();
        }
    }

    public void CloseSettings() {
        if (settingsPanel != null) {
            settingsPanel.SetActive(false);
        }
    }

    public void OnMusicToggleClicked() {
        if (Time.unscaledTime - lastMusicToggleTime < 0.2f) return;
        lastMusicToggleTime = Time.unscaledTime;

        if (AudioManager.Instance != null) {
            AudioManager.Instance.ToggleMusic();
            UpdateUI();
        }
    }

    public void OnSFXToggleClicked() {
        if (Time.unscaledTime - lastSFXToggleTime < 0.2f) return;
        lastSFXToggleTime = Time.unscaledTime;

        if (AudioManager.Instance != null) {
            AudioManager.Instance.ToggleSFX();
            UpdateUI();
        }
    }

    public void UpdateUI() {
        if (AudioManager.Instance == null) return;

        if (musicText != null) {
            musicText.text = AudioManager.Instance.isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }

        if (sfxText != null) {
            sfxText.text = AudioManager.Instance.isSFXOn ? "SFX: ON" : "SFX: OFF";
        }
    }
}