using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossTest : MonoBehaviour
{
    Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1ÉLÅ[
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            anim.SetTrigger("Attack2");
        }

    }

    public void BossAttack2AnimationEvent() {
        Debug.Log("BossAttack");
        EffectManager.Instance.SpawnEffect("BossAttack2Effect", transform.position, Quaternion.identity);
    }

}
