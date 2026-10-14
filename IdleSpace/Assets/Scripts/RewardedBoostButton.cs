using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardedBoostButton : MonoBehaviour {

    public Text buttonLabel;
    public float boostDuration = 60f; // 60 seconds auto-shooting boost
    public GameObject boostActiveEffect;

    private Button button;

    void Awake() {
        button = GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void Update() {
        if (Ship._instance != null && Ship._instance.IsAutoShootBoostActive) {
            if (buttonLabel != null) {
                buttonLabel.text = "BOOST: " + ((int)Ship._instance.autoShootBoostTimer) + "s";
            }
            if (boostActiveEffect != null && !boostActiveEffect.activeSelf) {
                boostActiveEffect.SetActive(true);
            }
        } else {
            if (buttonLabel != null) {
                buttonLabel.text = "FREE BOOST (AD)";
            }
            if (boostActiveEffect != null && boostActiveEffect.activeSelf) {
                boostActiveEffect.SetActive(false);
            }
        }
    }

    public void OnButtonClick() {
        if (UnityAds.Instance != null) {
            UnityAds.Instance.ShowRewardedAd(
                onSuccess: () => {
                    Debug.Log("Rewarded Ad Watched! Activating Auto-Shoot Boost for " + boostDuration + "s");
                    if (Ship._instance != null) {
                        Ship._instance.ActivateAutoShootBoost(boostDuration);
                    }
                },
                onFailure: () => {
                    Debug.LogWarning("Rewarded ad failed or was skipped.");
                }
            );
        } else {
            // Fallback for editor testing when ads might not be loaded
            Debug.Log("UnityAds instance not found, triggering boost directly for testing.");
            if (Ship._instance != null) {
                Ship._instance.ActivateAutoShootBoost(boostDuration);
            }
        }
    }
}
