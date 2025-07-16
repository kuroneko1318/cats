//using UnityEngine;

//public class Dummy : EnemyBase {
//    [SerializeField] private GameObject damagePopupPrefab;

//    private void Update() {
//        if (Input.GetKeyUp(KeyCode.J)) TakeDamage(); 
//    }

//    public override void TakeDamage() {
//        int damageAmount = 10; // 仮のダメージ値
//        ShowDamagePopup(damageAmount);
//        Debug.Log($"Dummy took {damageAmount} damage.");
//    }

//    private void ShowDamagePopup(int damage) {
//        if (damagePopupPrefab != null) {
//            GameObject popup = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
//            popup.GetComponent<DamagePopup>().Setup(damage);
//        }
//    }

//    public override void Attack() { }
//    public override void Dead() { }
//    public override void HealHp() { }
//    public override void Move() { }
//}

using UnityEngine;

public class Dummy : EnemyBase {
    [SerializeField] private GameObject digitPopupPrefab;

    private void Update() {
        if (Input.GetKeyUp(KeyCode.J)) TakeDamage();
    }
    public override void TakeDamage() {
        int damageAmount = 123; // 仮のダメージ値
        ShowDamagePopup(damageAmount);
    }

    private void ShowDamagePopup(int damage) {
        string damageStr = damage.ToString();
        float spacing = 0.5f;
        Vector3 startPos = transform.position + Vector3.up * 2f;

        for (int i = 0; i < damageStr.Length; i++) {
            Vector3 digitPos = startPos + Vector3.right * (i * spacing);
            GameObject digitObj = Instantiate(digitPopupPrefab, digitPos, Quaternion.identity);
            digitObj.GetComponent<DamagePopupDigit>().Setup(damageStr[i], i * 0.1f);
        }
    }

    public override void Attack() { }
    public override void Dead() { }
    public override void HealHp() { }
    public override void Move() { }
}
