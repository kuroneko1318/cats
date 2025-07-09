using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//全キャラ共通のステータス

public abstract class CharacterBase : MonoBehaviour{

    public int hp;
    public int maxHp;
    public int attack;
    public float stamina;
    public float moveSpeed;
    public float coolTime;
    public bool isInvincible = false;

    public abstract void TakeDamage();
    public abstract void HealHp();
    public abstract void Dead();
    public abstract void Attack();
    public abstract void Move();


}
