using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook;
using Facebook.Unity;
using System;

public class FBLog : MonoBehaviour {

    void Awake() {
        if (FB.IsInitialized) {
            FB.ActivateApp();

            FB.LogAppEvent(Facebook.Unity.AppEventName.ActivatedApp);
        } else {
            //Handle FB.Init
            FB.Init(() => {
                FB.ActivateApp();
                FB.LogAppEvent(Facebook.Unity.AppEventName.ActivatedApp);
            });
        }
    }


    // Unity will call OnApplicationPause(false) when an app is resumed
    // from the background
    void OnApplicationPause(bool pauseStatus) {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus) {
            //app resume
            if (FB.IsInitialized) {
                FB.ActivateApp();
                FB.LogAppEvent(Facebook.Unity.AppEventName.ActivatedApp);

            } else {
                //Handle FB.Init
                FB.Init(() => {
                    FB.ActivateApp();
                    FB.LogAppEvent(Facebook.Unity.AppEventName.ActivatedApp);

                });
            }
        }
    }

    public void Share(GameObject g) {
        FB.FeedShare("",
            new Uri("https://www.facebook.com/FMGamesStudio/"),
    callback: ShareCallback);

        g.SetActive(false);
    }

    public void FacebookShare(int n) {
        FB.ShareLink(new System.Uri("https://play.google.com/store/apps/details?id=com.fmgames.upto13"), "Can you beat my score? - " + n.ToString(),
            "Let's play Make it 13!",
            new System.Uri("https://play.google.com/store/apps/details?id=com.fmgames.upto13"));
    }

    private void ShareCallback(IShareResult result) {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error)) {
            Debug.Log("ShareLink Error: " + result.Error);
        } else {
            PlayerPrefs.SetInt("shared", 1);
        }
    }

}
