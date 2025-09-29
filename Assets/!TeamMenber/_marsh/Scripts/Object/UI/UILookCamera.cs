using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookCamera : MonoBehaviour {
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        // カメラの方向に向ける（Y軸は固定）
        if (mainCamera != null) {
            Vector3 lookDirection = transform.position - mainCamera.transform.position;
            lookDirection.y = 0f; // Y軸の回転を固定（必要に応じて調整）
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
