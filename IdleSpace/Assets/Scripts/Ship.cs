using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ship : MonoBehaviour {

    public static Ship _instance;

    public GameObject bulletPrefab;
    public Transform fireTransform;

    float autoShootTimer = 0;
    float shootTimer = 0;

    float minTimeBetweenTwoShoots = 0.05f;

    [Header("Auto Shoot Boost (Rewarded Ad)")]
    public float autoShootBoostTimer = 0f;
    public float boostedFireRate = 0.08f;
    private float boostShootTimer = 0f;

    public bool IsAutoShootBoostActive => autoShootBoostTimer > 0f;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        shootTimer += Time.deltaTime;
        autoShootTimer += Time.deltaTime;

        HandleMovement();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            Shoot();

        // Handle Auto-Shooting (boosted vs regular)
        if (autoShootBoostTimer > 0f) {
            autoShootBoostTimer -= Time.deltaTime;
            boostShootTimer += Time.deltaTime;

            if (boostShootTimer >= boostedFireRate && RocketSpawner._instance != null && !RocketSpawner._instance.isPaused) {
                boostShootTimer = 0f;
                Shoot();
            }
        } else {
            AutoShoot();
        }
    }

    public void ActivateAutoShootBoost(float durationInSeconds) {
        autoShootBoostTimer = Mathf.Max(autoShootBoostTimer + durationInSeconds, durationInSeconds);
    }

    float offsetX;
    public float xPosLimit;
    float touchTimer = 0;

    void HandleMovement() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 touchPos = Input.mousePosition;

            //normalize the X coord (make it between 0 and 1), Y coord is irrelevant
            offsetX = transform.localPosition.x - Mathf.Lerp(-xPosLimit, xPosLimit, touchPos.x / Screen.width);

            touchTimer = 0;
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            touchTimer += Time.deltaTime;

            if (touchTimer < .2f) return;

            Vector2 touchPos = Input.mousePosition;

            //normalize the X coord (make it between 0 and 1), Y coord is irrelevant
            float normalizedX = touchPos.x / Screen.width;

            //interpolate between -xPosLimit and +xPosLimit by the normalizedX coord
            //no need to clamp it
            float xPos = Mathf.Lerp(-xPosLimit, xPosLimit, normalizedX);

            //set the new position, leave Y and Z alone
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, Mathf.Clamp((xPos + offsetX) * 1.5f, -xPosLimit, xPosLimit), Time.deltaTime * 4), transform.localPosition.y, transform.localPosition.z);
        }
    }

    void AutoShoot() {
        if (Upgrades._instance != null && shootTimer > Upgrades._instance.secondsToShoot && (RocketSpawner._instance == null || !RocketSpawner._instance.isPaused)) {
            Shoot();
            shootTimer = 0;
        }
    }

    public void Shoot() {
        if (shootTimer < minTimeBetweenTwoShoots) return;
        shootTimer = 0;
        CreateBullet();
    }

    void CreateBullet() {
        GameObject bullet;
        if (ShipShop._instance != null && ShipShop._instance.currentShip != null && ShipShop._instance.currentShip.bulletPrefab != null) {
            bullet = Instantiate(ShipShop._instance.currentShip.bulletPrefab);
        } else {
            bullet = Instantiate(bulletPrefab);
        }

        bullet.transform.SetParent(transform.parent);
        bullet.transform.localScale = Vector3.one;
        bullet.transform.position = fireTransform != null ? fireTransform.position : transform.position;
    }
}
