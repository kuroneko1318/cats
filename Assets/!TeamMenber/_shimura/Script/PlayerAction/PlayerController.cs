
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PlayerBase {

    

    void Start() {
        input = GetComponent<PlayerInput>();
        if (input == null) {
            Debug.LogError("PlayerInput が取得できませんでした");
            return;
        }

        moveAction = input.actions["Move"];
        avoidanceAction = input.actions["Avoidance"];
        GatherAction = input.actions["Gather"];

        cam = GameObject.Find("Main Camera");

        initialPosition = transform.position;
    }

    void Update() {
        if (attackFlag == false) {
            PlayerMove();
            RotMove();
            Avoidance();
            Stop();
        }

        
    }

    

  

   

    

    
}
