using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickAnimation : MonoBehaviour {

    public static ClickAnimation _instance;

    List<GameObject> pool;
    public GameObject poolObject;
    public Transform parent;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        pool = new List<GameObject>();

        for (int i = 0; i < 30; i++) {
            var g = Instantiate(poolObject, parent);
            g.SetActive(false);


            pool.Add(g);
        }
    }

    public GameObject GetObjectFromPool() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                StartCoroutine(DisableObject(.75f, pool[i]));
                return pool[i];
            }
        }

        StartCoroutine(DisableObject(.75f, pool[0]));
        return pool[0];
    }

    IEnumerator DisableObject(float time, GameObject g) {
        yield return new WaitForSeconds(time);

        g.SetActive(false);
    }

    public static void SetText(Transform transform, float damage, Color color) {
        var textAnim = _instance.GetObjectFromPool();
        textAnim.SetActive(true);
        textAnim.transform.position = transform.position + Vector3.up / 3.3f;
        textAnim.GetComponent<Text>().text = damage + "";
        textAnim.GetComponent<Text>().color = color;
    }
}
