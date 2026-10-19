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

        RocketSpawner._instance.isPaused = false;

        StartCoroutine(ScaleUpDown(true));
    }

    // Update is called once per frame
    void Update() {
        if (currentHp <= 0 && !isDestroyed) {
            RocketSpawner._instance.isPaused = true;

            if (explosionPrefab != null) {
                var g = Instantiate(explosionPrefab);
                g.transform.position = transform.position;
            }

            GetComponent<Destructible2D.D2dDestructible>().enabled = false;
            GetComponent<Destructible2D.D2dPolygonCollider>().enabled = false;

            StartCoroutine(ScaleUpDown(false));
            LevelManager._instance.LevelCompleted();

            isDestroyed = true;

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

        Ship._instance.Shoot();

        var textAnim = ClickAnimation._instance.GetObjectFromPool();
        textAnim.SetActive(true);
        textAnim.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        float tapDmg = Upgrades._instance != null ? Mathf.Max(1f, Upgrades._instance.damagePerClick) : 1f;
        int displayTap = Mathf.Max(1, Mathf.RoundToInt(tapDmg));
        textAnim.GetComponent<Text>().text = "+" + (displayTap >= 10000 ? CurrencyManager.GetSuffix(displayTap) : displayTap.ToString());
        textAnim.GetComponent<Text>().color = Color.white;
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
        int level = LevelManager._instance.currentLevel;

        maxHp = 130 + 70 * level + 2.5f * level * level + level / 6 * 150;
    }
}
