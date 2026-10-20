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

        if (upgrades != null && timer > upgrades.secondsToSpawnMissile && !isPaused) {
            timer = 0;
            SpawnBaseRocket();
        }
    }

    void SpawnBaseRocket() {
        if (rocketPrefabs == null || rocketPrefabs.Length == 0 || parentTransforms == null || parentTransforms.Length == 0) return;
        int maxIndex = LevelManager._instance != null ? LevelManager._instance.currentLevel / 2 + 1 : 1;
        var rocket = Instantiate(rocketPrefabs[Random.Range(0, Mathf.Min(rocketPrefabs.Length, maxIndex))], parentTransforms[Random.Range(0, parentTransforms.Length)]);
        rocket.transform.localPosition = Vector3.zero;
    }

    public SpecialRocket[] specialRockets;

    public void UnlockSpecialRockets() {
        if (specialRockets == null || LevelManager._instance == null) return;
        foreach (SpecialRocket r in specialRockets) {
            if (r != null) {
                if (r.levelToUnlock <= LevelManager._instance.currentLevel)
                    r.gameObject.SetActive(true);
                else r.gameObject.SetActive(false);
            }
        }
    }

    public void CreateDestroyEffect(Vector3 pos, float radius) {
        if (destroyEffect == null) return;
        var effect = Instantiate(destroyEffect);
        effect.transform.position = pos;

        var exp = effect.GetComponent<Destructible2D.D2dExplosion>();
        if (exp != null) {
            exp.StampSize = new Vector2(radius, radius);
        }
    }
}
