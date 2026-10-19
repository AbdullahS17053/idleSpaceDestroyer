using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rocket : MonoBehaviour {

    private float currentDuration;
    public float duration = 3f;
    Vector3 startPos, endPos, cp;

    float damage;
    public float damageModifier = 1;

    public GameObject explosionPrefab;

    public bool specialRocket = false;
    public Color color;

    public float destructionRadius = .7f;

    // Use this for initialization
    void Start() {
        startPos = transform.position;
        endPos = new Vector3(0, 1, 0);

        if (!specialRocket)
            duration = Random.Range(2.5f, 3.5f);

        if (Mathf.Abs(startPos.x) > 3.1f)
            cp = new Vector3(startPos.x / 2, Random.Range(startPos.y, -startPos.y), 0);
        else
            cp = new Vector3(Random.Range(startPos.x, -startPos.x), startPos.y / 2, 0);

        damage = Upgrades._instance.missileDamage * damageModifier;

    }

    // Update is called once per frame
    void Update() {
        transform.position = CalculateBezierPoint(currentDuration / duration, startPos, endPos, cp);


        currentDuration += Time.deltaTime;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 startPosition, Vector3 endPosition, Vector3 controlPoint) {
        float u = 1 - t;
        float uu = u * u;

        Vector3 point = uu * startPosition;
        point += 2 * u * t * controlPoint;
        point += t * t * endPosition;

        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(point.y - transform.position.y, point.x - transform.position.x) * Mathf.Rad2Deg - 90);

        return point;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Planet") {
            float appliedDamage = 0f;
            if (!specialRocket) {
                appliedDamage = Mathf.Min(damage, LevelManager._instance.currentPlanet.maxHp / 20f / (1 / Upgrades._instance.secondsToSpawnMissile));
                LevelManager._instance.currentPlanet.currentHp -= appliedDamage;
                CurrencyManager._instance.AddCoins(Mathf.Min(damage, LevelManager._instance.currentPlanet.maxHp / 40f / (1 / Upgrades._instance.secondsToSpawnMissile)));
            } else {
                appliedDamage = Mathf.Min(damage, LevelManager._instance.currentPlanet.maxHp / 3);
                LevelManager._instance.currentPlanet.currentHp -= appliedDamage;
                CurrencyManager._instance.AddCoins(appliedDamage);
            }

            if (DPSDisplay.Instance != null)
                DPSDisplay.Instance.RegisterDamage(appliedDamage);

            if (explosionPrefab != null) {
                var g = Instantiate(explosionPrefab, transform.parent);
                g.transform.position = transform.position;
            }

            ClickAnimation.SetText(transform, (int)damage, color);

            var destructible = (LevelManager._instance != null && LevelManager._instance.currentPlanet != null) ? LevelManager._instance.currentPlanet.GetComponent<Destructible2D.D2dDestructible>() : null;
            if (destructible != null && destructible.Damage < 3500 && RocketSpawner._instance != null)
                RocketSpawner._instance.CreateDestroyEffect(transform.position, destructionRadius);

            Destroy(gameObject);
        }
    }
}
