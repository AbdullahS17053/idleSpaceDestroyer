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

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        currentLevel = PlayerPrefs.GetInt("CURRENT_LEVEL", 1);
        if (mainCam == null)
            mainCam = Camera.main;

        originalCamPos = mainCam.transform.localPosition;
        originalCamSize = mainCam.orthographicSize;
        CreateNewPlanet();
        

        RocketSpawner._instance.UnlockSpecialRockets();
    }

    // Update is called once per frame
    void Update() {
        fillImage.fillAmount = 1 - currentPlanet.currentHp / (float)currentPlanet.maxHp;
        percentText.text = (int)(fillImage.fillAmount * 100) + "%";

    }

    /// <summary>
    /// Instantiating new planet for new level
    /// </summary>
    public void CreateNewPlanet() {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.complete);

        if (currentPlanet != null) Destroy(currentPlanet.gameObject);

        currentPlanet = Instantiate(planets[(currentLevel - 1) % (planets.Length)], planetParent)
            .GetComponent<Planet>();

        SetLevelCircles();

        if (CoinShop._instance != null)
            CoinShop._instance.SetTexts();

        StartCoroutine(CameraEffect());
    }

    /// <summary>
    /// Method to set the level indicator circles on top of the screen
    /// </summary>
    void SetLevelCircles() {
        for (int i = 0; i < 3; i++) {
            levelCircles[i].levelText.GetComponent<Text>().text = (currentLevel - 1 + i).ToString();
            if ((currentLevel - 1 + i) % 5 == 0 && currentLevel != 1)
                levelCircles[i].bossSkull.SetActive(true);
            else {
                levelCircles[i].bossSkull.SetActive(false);
            }
        }
    }

    public void LevelCompleted() {
        FB.LogAppEvent("LevelCompleted", currentLevel);

        currentLevel++;
        PlayerPrefs.SetInt("CURRENT_LEVEL", currentLevel);

        RocketSpawner._instance.UnlockSpecialRockets();

        StartCoroutine(StartDelayedLevel(4f));
    }

    IEnumerator StartDelayedLevel(float t) {
        yield return new WaitForSeconds(t);

        CreateNewPlanet();
    }
    
    IEnumerator CameraEffect()
    {
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
