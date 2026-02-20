using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
#if UNITY_IOS
    string gameId = "6049762";
    string adUnitId = "Interstitial_iOS";
#elif UNITY_ANDROID
    string gameId = "6049763";
    string adUnitId = "Interstitial_Android";
#endif

    public float timeToShowAds = 150f;
    float timer = 0f;

    void Start()
    {
        Advertisement.Initialize(gameId, true, this); // true = test mode
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > timeToShowAds)
        {
            timer = 0f;
            LoadAd();
        }
    }

    void LoadAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    void ShowAd()
    {
        Advertisement.Show(adUnitId, this);
    }

    // ---------------- Initialization ----------------

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialization Complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Initialization Failed: " + message);
    }

    // ---------------- Load ----------------

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded");
        ShowAd();
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Load Failed: " + message);
    }

    // ---------------- Show ----------------

    public void OnUnityAdsShowStart(string adUnitId) { }

    public void OnUnityAdsShowClick(string adUnitId) { }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log("Show Failed: " + message);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ad Completed");
    }
}