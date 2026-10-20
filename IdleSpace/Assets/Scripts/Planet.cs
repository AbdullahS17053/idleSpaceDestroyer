using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour {

    public float maxHp, currentHp;

    public GameObject explosionPrefab;
    bool isDestroyed = false;

    private void Awake() {
        // Ensure the planet always has a 2D collider tagged Planet for projectile detection
        Collider2D col = GetComponentInChildren<Collider2D>();
        if (col == null) {
            CircleCollider2D circleCol = gameObject.AddComponent<CircleCollider2D>();
            circleCol.isTrigger = true;
            circleCol.radius = 2.2f;
        }
        if (gameObject.tag != "Planet") {
            gameObject.tag = "Planet";
        }
    }

    // Use this for initialization
    void Start() {
        CalculateHp();

        currentHp = maxHp;

        if (RocketSpawner._instance != null) {
            RocketSpawner._instance.isPaused = false;
        }

        StartCoroutine(ScaleUpDown(true));
    }

    // Update is called once per frame
    void Update() {
        if (currentHp <= 0 && !isDestroyed) {
            // Immediately mark destroyed so this block only ever executes once
            isDestroyed = true;

            if (RocketSpawner._instance != null) {
                RocketSpawner._instance.isPaused = true;
            }

            if (explosionPrefab != null) {
                var g = Instantiate(explosionPrefab);
                g.transform.position = transform.position;
            }

            // Safe checks in case planet does not have Destructible2D components
            var destructible = GetComponent<Destructible2D.D2dDestructible>();
            if (destructible != null) destructible.enabled = false;

            var polyCol = GetComponent<Destructible2D.D2dPolygonCollider>();
            if (polyCol != null) polyCol.enabled = false;

            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            StartCoroutine(ScaleUpDown(false));

            if (LevelManager._instance != null) {
                LevelManager._instance.LevelCompleted();
            }

            PlayerPrefs.SetFloat("ALL_DAMAGE", PlayerPrefs.GetFloat("ALL_DAMAGE") + maxHp);
        }
    }


    Coroutine onClickCoroutine;

    /// <summary>
    /// OnClick handler for the planet, manages text animation also
    /// </summary>
    public void OnClick() {
        if (onClickCoroutine != null) StopCoroutine(onClickCoroutine);
        onClickCoroutine = StartCoroutine(ClickAnim());

        if (Ship._instance != null) {
            Ship._instance.Shoot();
        }

        if (ClickAnimation._instance != null) {
            var textAnim = ClickAnimation._instance.GetObjectFromPool();
            textAnim.SetActive(true);
            textAnim.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
            float tapDmg = Upgrades._instance != null ? Mathf.Max(1f, Upgrades._instance.damagePerClick) : 1f;
            int displayTap = Mathf.Max(1, Mathf.RoundToInt(tapDmg));
            textAnim.GetComponent<Text>().text = "+" + (displayTap >= 10000 ? CurrencyManager.GetSuffix(displayTap) : displayTap.ToString());
            textAnim.GetComponent<Text>().color = Color.white;
        }
    }

    IEnumerator ClickAnim() {
        transform.localScale = Vector3.one;

        float t = 0;
        while (t < 1) {
            t += Time.deltaTime * 9;
            transform.localScale += Vector3.one * Time.deltaTime * .15f;

            yield return null;
        }

        t = 0;
        while (t < 1) {
            t += Time.deltaTime * 9;
            transform.localScale -= Vector3.one * Time.deltaTime * .15f;

            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    IEnumerator ScaleUpDown(bool scaleUp) {
        float t = 0;
        while (t < 1) {
            t += Time.deltaTime * 3;

            if (scaleUp) {
                transform.localScale = new Vector3(t, t, t);
            } else {
                transform.localScale = new Vector3(1 - t, 1 - t, 1 - t);
            }

            yield return null;
        }

        if (scaleUp == false) transform.localScale = Vector3.zero;
    }

    void CalculateHp() {
        int level = LevelManager._instance != null ? LevelManager._instance.currentLevel : PlayerPrefs.GetInt("CURRENT_LEVEL", 1);

        float baseHp = 130f + 70f * level + 2.5f * level * level + (level / 6) * 150f;

        if (level > 22) {
            // Endless progression: planets beyond level 22 have boosted health scaling
            int extraLevels = level - 22;
            baseHp = (baseHp + extraLevels * 300f) * Mathf.Pow(1.08f, extraLevels);
        }

        maxHp = Mathf.Round(baseHp);
    }
}
