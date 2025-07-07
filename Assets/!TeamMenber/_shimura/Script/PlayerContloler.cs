using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContloler : MonoBehaviour {
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayerController() {

        Vector3 moveDirection = Vector3.zero;

        // Wキー（前進）
        if (Input.GetKey(KeyCode.W)) {
            moveDirection += transform.forward;
        }

        // Sキー（後退）
        if (Input.GetKey(KeyCode.S)) {
            moveDirection -= transform.forward;
        }

        // Dキー（右移動）
        if (Input.GetKey(KeyCode.D)) {
            moveDirection += transform.right;
        }

        // Aキー（左移動）
        if (Input.GetKey(KeyCode.A)) {
            moveDirection -= transform.right;
        }

        // 移動方向を正規化して速度を一定に
        moveDirection = moveDirection.normalized;

        // 実際に移動
       // transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }


}
