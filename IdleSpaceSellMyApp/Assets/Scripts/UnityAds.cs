using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_IOS
        Advertisement.Initialize("3041496");
#elif UNITY_ANDROID
        Advertisement.Initialize("3041497");
#endif

    }

    public float timeToShowAds = 150;
    float timer = 0;

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;

		if (timer > timeToShowAds) {
            timer = 0;
            Advertisement.Show();
        }
	}
}
