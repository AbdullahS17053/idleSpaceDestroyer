using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
    public GameObject explosionPrefab;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.Translate(Vector3.up * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Planet") {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.explosion);
            LevelManager._instance.currentPlanet.currentHp -= Upgrades._instance.damagePerClick;
            CurrencyManager._instance.AddCoins(Upgrades._instance.damagePerClick);

            if (explosionPrefab != null) {
                var g = Instantiate(explosionPrefab, transform.parent);
                g.transform.position = transform.position;
            }

            if (LevelManager._instance.currentPlanet.GetComponent<Destructible2D.D2dDestructible>().Damage < 3500)
                RocketSpawner._instance.CreateDestroyEffect(transform.position + Vector3.down * .1f, .25f);
        }

        Destroy(gameObject);
    }
}
