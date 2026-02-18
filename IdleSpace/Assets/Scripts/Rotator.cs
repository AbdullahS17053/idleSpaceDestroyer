using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public bool fixedRotation = true;
    public float rotationSpeed;

    LevelManager levelManager;

    private void Start() {
        levelManager = LevelManager._instance;
    }


    // Update is called once per frame
    void Update() {
        if (fixedRotation)
            rotationSpeed = 1 + Mathf.Max(-levelManager.currentLevel / 6f, -4);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
