using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
public class PlayerController : CharacterBase {

    //各アクションの宣言
    PlayerInput input;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction avoidanceAction;
    public Rigidbody rb;

    public Vector3 latestPos;
    public Vector3 direction;
    public Vector3 diff;
    // Start is called before the first frame update
    void Start() {

        input = GetComponent<PlayerInput>();
        if (input == null) {
            Debug.LogError("PlayerInput が取得できませんでした");
            return;
        }

        moveAction = input.actions["Move"];
        if (moveAction == null) {
            Debug.LogError("Move アクションが見つかりません");
        }


        avoidanceAction = input.actions["Avoidance"];
        if (avoidanceAction == null) {
            Debug.LogError("Avoidance アクションが見つかりません");
        }



    }

    // Update is called once per frame
    void Update() {
        PlayerMove();
        RotMove();
        Avoidance();
        Stop();
    }

    public void RotMove() {

        diff = transform.position - latestPos;   //前回からどこに進んだかをベクトルで取得
        latestPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);  //前回のPositionの更新
        
        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        if (diff.magnitude > 0.01f) {
            
            transform.rotation = Quaternion.LookRotation(diff); //向きを変更する
            //transform.rotation = Quaternion.Euler(0,diff.magnitude,0);
        }


    }

    public void PlayerMove() {
        if (moveAction.IsInProgress()) {
            Vector3 dir = moveAction.ReadValue<Vector3>() * moveSpeed;
            rb.velocity = new Vector3(dir.x,0,dir.z);
            direction = dir;
        }
       

    }

    public void Avoidance() {
        if (avoidanceAction.IsPressed()) {
            rb.velocity =transform.forward*moveSpeed*2;
        }
    }

    public void Stop() {
        if (!moveAction.IsInProgress()&&!avoidanceAction.IsInProgress())
        rb.velocity = Vector3.zero;
    }
    public override void TakeDamage() {

    }

    public override void HealHp() {

    }

    public override void Dead() {

    }
}
