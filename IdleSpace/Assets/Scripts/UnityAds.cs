#pragma warning disable 0414
using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static UnityAds Instance;

    [Header("Game IDs")]
    [SerializeField] private string androidGameId = "6049763";
    [SerializeField] private string iOSGameId = "6049762";

    [Header("Interstitial Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "Interstitial_Android";
    [SerializeField] private string iOSAdUnitId = "Interstitial_iOS";

    [Header("Rewarded Ad Unit IDs")]
    [SerializeField] private string androidRewardedAdUnitId = "Rewarded_Android";
    [SerializeField] private string iOSRewardedAdUnitId = "Rewarded_iOS";

    [Header("Settings")]
    [SerializeField] private bool testMode = true;
    public float timeToShowAds = 150f;

    private string gameId;
    private string adUnitId;
    private string rewardedAdUnitId;
    private float timer = 0f;

    private Action onRewardSuccess;
    private Action onRewardFailure;
    private bool isRewardedLoaded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

#if UNITY_IOS
        gameId = iOSGameId;
        adUnitId = iOSAdUnitId;
        rewardedAdUnitId = iOSRewardedAdUnitId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
        rewardedAdUnitId = androidRewardedAdUnitId;
#elif UNITY_EDITOR
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
        rewardedAdUnitId = androidRewardedAdUnitId;
#else
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
        rewardedAdUnitId = androidRewardedAdUnitId;
#endif
    }

    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
        else
        {
            Debug.LogWarning("Unity Ads is not supported on this platform.");
        }
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

    #region Interstitial Ads
    public void LoadAd()
    {
        if (Advertisement.isInitialized)
        {
            Advertisement.Load(adUnitId, this);
        }
    }

    public void ShowAd()
    {
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(adUnitId, this);
        }
    }
    #endregion

    #region Rewarded Ads
    public void LoadRewardedAd()
    {
        if (Advertisement.isInitialized && !string.IsNullOrEmpty(rewardedAdUnitId))
        {
            Advertisement.Load(rewardedAdUnitId, this);
        }
    }

    public bool IsRewardedAdReady()
    {
        return isRewardedLoaded;
    }

    public void ShowRewardedAd(Action onSuccess, Action onFailure = null)
    {
        onRewardSuccess = onSuccess;
        onRewardFailure = onFailure;

        if (Advertisement.isInitialized && !string.IsNullOrEmpty(rewardedAdUnitId))
        {
            Advertisement.Show(rewardedAdUnitId, this);
        }
        else
        {
            Debug.LogWarning("Rewarded Ad not ready or initialized.");
            onRewardFailure?.Invoke();
        }
    }
    #endregion

    // ---------------- Initialization ----------------

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialization Complete");
        LoadAd();
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Initialization Failed: " + message);
    }

    // ---------------- Load ----------------

    public void OnUnityAdsAdLoaded(string unitId)
    {
        Debug.Log("Ad Loaded: " + unitId);
        if (unitId == adUnitId)
        {
            ShowAd();
        }
        else if (unitId == rewardedAdUnitId)
        {
            isRewardedLoaded = true;
        }
    }

    public void OnUnityAdsFailedToLoad(string unitId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Load Failed: " + unitId + " - " + message);
        if (unitId == rewardedAdUnitId)
        {
            isRewardedLoaded = false;
        }
    }

    // ---------------- Show ----------------

    public void OnUnityAdsShowStart(string unitId) { }

    public void OnUnityAdsShowClick(string unitId) { }

    public void OnUnityAdsShowFailure(string unitId, UnityAdsShowError error, string message)
    {
        Debug.Log("Show Failed: " + unitId + " - " + message);
        if (unitId == rewardedAdUnitId)
        {
            onRewardFailure?.Invoke();
            onRewardSuccess = null;
            onRewardFailure = null;
            LoadRewardedAd();
        }
    }

    public void OnUnityAdsShowComplete(string unitId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Ad Completed: " + unitId + " with state " + showCompletionState);
        if (unitId == rewardedAdUnitId)
        {
            isRewardedLoaded = false;
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                onRewardSuccess?.Invoke();
            }
            else
            {
                onRewardFailure?.Invoke();
            }
            onRewardSuccess = null;
            onRewardFailure = null;

            // Preload next rewarded ad
            LoadRewardedAd();
        }
    }
}

