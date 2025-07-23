using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


//全キャラ共通のステータス

public abstract class CharacterBase : MonoBehaviour {

    public int hp;
    public int maxHp;
    public int defence;
    public int attack;
    public float stamina;
    public float moveSpeed;
    public bool isInvincible = false;

    [NonSerialized]public int damage = 0;

    //ダメージを受ける処理はそれぞれのBaseにある
    public abstract void HealHp();
    public abstract void Dead();
    public abstract void Attack();
    public abstract void Move();


}