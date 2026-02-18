using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour {

    public static RocketSpawner _instance;

    public GameObject[] rocketPrefabs;
    public Transform[] parentTransforms;

    float timer;
    Upgrades upgrades;

    public bool isPaused = false;

    public GameObject destroyEffect;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        upgrades = Upgrades._instance;
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;

        if (timer > upgrades.secondsToSpawnMissile && !isPaused) {
            timer = 0;
            SpawnBaseRocket();
        }
    }

    void SpawnBaseRocket() {
        var rocket = Instantiate(rocketPrefabs[Random.Range(0, Mathf.Min(rocketPrefabs.Length, LevelManager._instance.currentLevel / 2 + 1))], parentTransforms[Random.Range(0, parentTransforms.Length)]);
        rocket.transform.localPosition = Vector3.zero;
    }

    public SpecialRocket[] specialRockets;

    public void UnlockSpecialRockets() {
        foreach (SpecialRocket r in specialRockets) {
            if (r.levelToUnlock <= LevelManager._instance.currentLevel)
                r.gameObject.SetActive(true);
            else r.gameObject.SetActive(false);
        }
    }

    public void CreateDestroyEffect(Vector3 pos, float radius) {
        var effect = Instantiate(destroyEffect);
        effect.transform.position = pos;

        effect.GetComponent<Destructible2D.D2dExplosion>().StampSize = new Vector2(radius, radius);
    }
}
