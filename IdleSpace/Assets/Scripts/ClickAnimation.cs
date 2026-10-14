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

    void Start() {
        pool = new List<GameObject>();

        for (int i = 0; i < 80; i++) {
            var g = Instantiate(poolObject, parent);
            PrepareTextObject(g);
            g.SetActive(false);
            pool.Add(g);
        }
    }

    void PrepareTextObject(GameObject g) {
        if (g == null) return;

        var outline = g.GetComponent<Outline>();
        if (outline == null) {
            outline = g.AddComponent<Outline>();
        }
        outline.effectColor = new Color(0f, 0f, 0f, 1f);
        outline.effectDistance = new Vector2(2.5f, -2.5f);

        var txt = g.GetComponent<Text>();
        if (txt != null) {
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.resizeTextForBestFit = true;
            txt.resizeTextMinSize = 22;
            txt.resizeTextMaxSize = 70;
        }

        var rt = g.GetComponent<RectTransform>();
        if (rt != null) {
            rt.sizeDelta = new Vector2(240f, 80f);
        }
    }

    public GameObject GetObjectFromPool() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                StartCoroutine(DisableObject(.75f, pool[i]));
                return pool[i];
            }
        }

        var newObj = Instantiate(poolObject, parent);
        PrepareTextObject(newObj);
        newObj.SetActive(false);
        pool.Add(newObj);
        StartCoroutine(DisableObject(.75f, newObj));
        return newObj;
    }

    IEnumerator DisableObject(float time, GameObject g) {
        yield return new WaitForSeconds(time);

        if (g != null) {
            g.SetActive(false);
            g.transform.localScale = Vector3.one;
        }
    }

    public static void SetText(Transform targetTransform, float damage, Color color) {
        if (_instance == null || targetTransform == null) return;
        SetText(targetTransform.position, damage, color);
    }

    public static void SetText(Vector3 position, float damage, Color color) {
        SetTextWithPrefix(position, damage, color, "", 1.45f);
    }

    public static void SetTextWithPrefix(Vector3 position, float damage, Color color, string prefix, float scale = 1.45f) {
        if (_instance == null) return;
        var textAnim = _instance.GetObjectFromPool();
        textAnim.SetActive(true);

        Vector3 scatter = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.1f, 0.25f), 0f);
        textAnim.transform.position = position + (Vector3.up / 3.3f) + scatter;

        var txt = textAnim.GetComponent<Text>();
        if (txt != null) {
            int displayDmg = Mathf.Max(1, Mathf.RoundToInt(damage));
            string numStr = displayDmg >= 10000 ? CurrencyManager.GetSuffix(displayDmg) : displayDmg.ToString();
            txt.text = prefix + numStr;
            txt.color = color;
        }

        _instance.StartCoroutine(_instance.PunchScale(textAnim.transform, scale));
    }

    IEnumerator PunchScale(Transform tr, float peakMultiplier = 1.45f) {
        float elapsed = 0f;
        float duration = 0.22f;
        Vector3 peakScale = Vector3.one * peakMultiplier;

        while (elapsed < duration) {
            if (tr == null) yield break;
            elapsed += Time.deltaTime;
            tr.localScale = Vector3.Lerp(peakScale, Vector3.one, elapsed / duration);
            yield return null;
        }

        if (tr != null)
            tr.localScale = Vector3.one;
    }
}
