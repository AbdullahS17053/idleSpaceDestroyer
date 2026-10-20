using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    public static LevelManager _instance;

    public int currentLevel;

    public GameObject[] planets;
    public Transform planetParent;
    public Planet currentPlanet;

    [Header("Level Bar")]
    public LevelCircle[] levelCircles;
    public Image fillImage;
    public Text percentText;
    
    [Header("Camera Effects")]
    public Camera mainCam;
    public float zoomAmount = 1.5f;
    public float zoomDuration = 1f;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 0.2f;

    Vector3 originalCamPos;
    float originalCamSize;

    private int lastPlanetIndex = -1;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        currentLevel = PlayerPrefs.GetInt("CURRENT_LEVEL", 1);
        lastPlanetIndex = PlayerPrefs.GetInt("LAST_PLANET_INDEX", -1);

        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam != null) {
            originalCamPos = mainCam.transform.localPosition;
            originalCamSize = mainCam.orthographicSize;
        }

        CreateNewPlanet();

        if (RocketSpawner._instance != null) {
            RocketSpawner._instance.UnlockSpecialRockets();
        }
    }

    // Update is called once per frame
    void Update() {
        if (currentPlanet != null && fillImage != null && percentText != null && currentPlanet.maxHp > 0) {
            fillImage.fillAmount = 1 - currentPlanet.currentHp / (float)currentPlanet.maxHp;
            percentText.text = (int)(fillImage.fillAmount * 100) + "%";
        }
    }

    /// <summary>
    /// Instantiating new planet for new level
    /// </summary>
    public void CreateNewPlanet() {
        if (AudioManager.Instance != null && AudioManager.Instance.complete != null) {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.complete);
        }

        if (currentPlanet != null) Destroy(currentPlanet.gameObject);

        int planetIndex = GetPlanetIndexForLevel(currentLevel);

        if (planets != null && planets.Length > 0) {
            planetIndex = Mathf.Clamp(planetIndex, 0, planets.Length - 1);
            currentPlanet = Instantiate(planets[planetIndex], planetParent)
                .GetComponent<Planet>();
        }

        if (RocketSpawner._instance != null) {
            RocketSpawner._instance.isPaused = false;
        }

        SetLevelCircles();

        if (CoinShop._instance != null)
            CoinShop._instance.SetTexts();

        StartCoroutine(CameraEffect());
    }

    int GetPlanetIndexForLevel(int level) {
        if (planets == null || planets.Length == 0) return 0;

        // Normal progression up to level 22 (or planets.Length)
        if (level <= planets.Length) {
            int idx = level - 1;
            lastPlanetIndex = idx;
            PlayerPrefs.SetInt("LAST_PLANET_INDEX", lastPlanetIndex);
            PlayerPrefs.SetInt("SAVED_PLANET_INDEX", idx);
            PlayerPrefs.SetInt("SAVED_PLANET_LEVEL", level);
            return idx;
        }

        // Endless progression: check if we already picked a planet for this exact level (e.g. on game reload)
        int savedLevel = PlayerPrefs.GetInt("SAVED_PLANET_LEVEL", -1);
        int savedIndex = PlayerPrefs.GetInt("SAVED_PLANET_INDEX", -1);
        if (savedLevel == level && savedIndex >= 0 && savedIndex < planets.Length) {
            return savedIndex;
        }

        // Otherwise pick any random planet except the last one played
        int excludeIndex = lastPlanetIndex;
        if (excludeIndex < 0 || excludeIndex >= planets.Length) {
            // Default exclude index if not previously set
            excludeIndex = (level - 2) % planets.Length;
        }

        int newIndex = GetRandomPlanetIndexExcluding(excludeIndex);

        lastPlanetIndex = newIndex;
        PlayerPrefs.SetInt("LAST_PLANET_INDEX", lastPlanetIndex);
        PlayerPrefs.SetInt("SAVED_PLANET_INDEX", newIndex);
        PlayerPrefs.SetInt("SAVED_PLANET_LEVEL", level);
        PlayerPrefs.Save();

        return newIndex;
    }

    int GetRandomPlanetIndexExcluding(int excludeIndex) {
        if (planets == null || planets.Length == 0) return 0;
        if (planets.Length == 1) return 0;

        // Generate a random candidate among the other available planets
        int candidate = Random.Range(0, planets.Length - 1);
        if (excludeIndex >= 0 && excludeIndex < planets.Length) {
            if (candidate >= excludeIndex) {
                candidate++;
            }
        }
        return candidate;
    }

    /// <summary>
    /// Method to set the level indicator circles on top of the screen
    /// </summary>
    void SetLevelCircles() {
        if (levelCircles == null) return;
        for (int i = 0; i < levelCircles.Length; i++) {
            if (levelCircles[i] != null && levelCircles[i].levelText != null) {
                Text txt = levelCircles[i].levelText.GetComponent<Text>();
                if (txt != null) {
                    txt.text = (currentLevel - 1 + i).ToString();
                }
                if (levelCircles[i].bossSkull != null) {
                    if ((currentLevel - 1 + i) % 5 == 0 && currentLevel != 1)
                        levelCircles[i].bossSkull.SetActive(true);
                    else {
                        levelCircles[i].bossSkull.SetActive(false);
                    }
                }
            }
        }
    }

    public void LevelCompleted() {
        try {
            if (FB.IsInitialized) {
                FB.LogAppEvent("LevelCompleted", currentLevel);
            }
        } catch (System.Exception e) {
            Debug.LogWarning("FB LogAppEvent error: " + e.Message);
        }

        // Record the current planet index before incrementing level so next level excludes it
        int completedPlanetIndex = PlayerPrefs.GetInt("SAVED_PLANET_INDEX", (currentLevel - 1) % planets.Length);
        lastPlanetIndex = completedPlanetIndex;
        PlayerPrefs.SetInt("LAST_PLANET_INDEX", lastPlanetIndex);

        currentLevel++;
        PlayerPrefs.SetInt("CURRENT_LEVEL", currentLevel);

        if (RocketSpawner._instance != null) {
            RocketSpawner._instance.UnlockSpecialRockets();
        }

        StartCoroutine(StartDelayedLevel(4f));
    }

    IEnumerator StartDelayedLevel(float t) {
        yield return new WaitForSeconds(t);

        CreateNewPlanet();
    }
    
    IEnumerator CameraEffect()
    {
        if (mainCam == null) yield break;

        float elapsed = 0f;

        // Zoom In
        while (elapsed < zoomDuration)
        {
            mainCam.orthographicSize = Mathf.Lerp(originalCamSize, 
                originalCamSize - zoomAmount, 
                elapsed / zoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Shake + Zoom Out
        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            randomOffset.z = 0;

            mainCam.transform.localPosition = originalCamPos + randomOffset;

            mainCam.orthographicSize = Mathf.Lerp(originalCamSize - zoomAmount, 
                originalCamSize, 
                elapsed / shakeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset
        mainCam.transform.localPosition = originalCamPos;
        mainCam.orthographicSize = originalCamSize;
    }
}
