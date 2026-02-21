using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialRocket : MonoBehaviour {

    public int id;
    public int levelToUnlock;
    public GameObject rocketPrefab;

    public Image fillImage;
    float timer;
    public float reloadTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        fillImage.fillAmount = timer / reloadTime;
	}

    public void LaunchRocket() {
        if (timer < reloadTime) return;

        var rocket = Instantiate(rocketPrefab, RocketSpawner._instance.parentTransforms[Random.Range(0, RocketSpawner._instance.parentTransforms.Length)]);
        rocket.transform.localPosition = Vector3.zero;

        timer = 0;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.spawn);
    }

    public void LaunchMultipleRockets() {

        if (timer < reloadTime) return;

        StartCoroutine(RocketDelay());

        timer = 0;
    }

    IEnumerator RocketDelay() {
        Transform t = RocketSpawner._instance.parentTransforms[Random.Range(0, RocketSpawner._instance.parentTransforms.Length)];

        for (int i = 0; i < 6; i++) {
            var rocket = Instantiate(rocketPrefab, t);
            rocket.transform.localPosition = Vector3.zero;

            yield return new WaitForSeconds(.1f);
        }
    }
}
