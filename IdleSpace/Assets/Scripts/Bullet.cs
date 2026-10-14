using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
    public GameObject explosionPrefab;

    void Start() {

    }

    void Update() {
        transform.Translate(Vector3.up * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Planet") {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.explosion);

            float damage = Upgrades._instance != null ? Upgrades._instance.damagePerClick : 1f;
            bool isCrit = false;
            if (Upgrades._instance != null && Upgrades._instance.RollCritical(out float critMult)) {
                damage *= critMult;
                isCrit = true;
            }

            if (LevelManager._instance != null && LevelManager._instance.currentPlanet != null)
                LevelManager._instance.currentPlanet.currentHp -= damage;

            if (CurrencyManager._instance != null)
                CurrencyManager._instance.AddCoins(damage);

            Color textColor = isCrit ? new Color(1f, 0.25f, 0.15f) : new Color(1f, 0.88f, 0.15f);
            string prefix = isCrit ? "CRIT! " : "";
            ClickAnimation.SetTextWithPrefix(transform.position, damage, textColor, prefix, isCrit ? 1.85f : 1.45f);

            if (DPSDisplay.Instance != null)
                DPSDisplay.Instance.RegisterDamage(damage);

            if (explosionPrefab != null) {
                var g = Instantiate(explosionPrefab, transform.parent);
                g.transform.position = transform.position;
            }

            if (LevelManager._instance != null && LevelManager._instance.currentPlanet != null) {
                var destructible = LevelManager._instance.currentPlanet.GetComponent<Destructible2D.D2dDestructible>();
                if (destructible != null && destructible.Damage < 3500 && RocketSpawner._instance != null)
                    RocketSpawner._instance.CreateDestroyEffect(transform.position + Vector3.down * .1f, .25f);
            }
        }

        Destroy(gameObject);
    }
}
