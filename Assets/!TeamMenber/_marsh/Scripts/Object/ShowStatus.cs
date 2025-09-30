using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowStatus : MonoBehaviour {
    [SerializeField] private TextMeshPro statusText3D; // Inspectorで3Dテキストをアサイン
    private PlayerBase player;

    void Start() {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.GetComponent<PlayerBase>();
        }
    }

    void Update() {
        if (player == null || statusText3D == null) return;

        // ステータスを改行でまとめて表示
        statusText3D.text =
            $"HP: {player.hp}/{player.maxHp}\n" +
            $"基礎攻撃力: {player.baseAttack}\n" +
            $"攻撃力: {player.attack}\n" +
            $"基礎防御力: {player.baseDefence}\n" +
            $"防御力: {player.defence}\n" +
            $"会心率: {player.criticalChance * 100}%\n" +
            $"会心倍率: {player.criticalMultiplier}倍";
    }
}
