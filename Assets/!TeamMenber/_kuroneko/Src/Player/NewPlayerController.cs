using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private Animator anim;
    private CharacterController controller;

    private NewPlayerMove move;      // 移動処理クラス
    private NewPlayerAttack attack;  // 攻撃処理クラス

    private Vector2 moveInput;       // 入力キャッシュ

    void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // インスタンス生成
        move = new NewPlayerMove(controller, anim);
        attack = new NewPlayerAttack(anim);
    }

    void Update() {
        // 入力取得（例: Unity Input Systemがある場合はそこに差し替え）
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 移動処理
        move.Move(moveInput);

        // 回避入力
        if (Input.GetKeyDown(KeyCode.Space)) {
            move.Dash(moveInput);
        }

        // 攻撃入力
        if (Input.GetMouseButtonDown(0)) {
            attack.OnAttackInput();
        }

        // 毎フレーム更新（回避状態解除など）
        move.Update();
    }

    // 攻撃アニメーション終了時に呼ばれる
    public void OnAttackAnimationEnd() {
        attack.OnAttackAnimationEnd();
    }
}
