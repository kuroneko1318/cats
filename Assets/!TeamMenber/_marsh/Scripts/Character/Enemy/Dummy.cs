using UnityEngine;

public class Dummy : EnemyBase {
    [SerializeField] private GameObject damagePopupPrefab;
    private DamagePopupController currentPopup;

    bool isCritical = false;

    private void Update() {
        //デバッグでJキーでダメージ
        if (Input.GetKeyDown(KeyCode.J)) TakeDamage();
    }

    //ダメージを受ける処理、必要であれば引数を設定
    public override void TakeDamage() {
        int damageAmount = Random.Range(10, 30); // 仮のダメージ値
        isCritical = Random.value < 0.2f; // 20%でクリティカル

        // 既存のポップアップが存在し、まだフェードアウトしていない場合は加算
        if (currentPopup != null && !currentPopup.IsFadingOut) {
            currentPopup.AddDamage(damageAmount, isCritical);
        }
        else {
            // 新しいポップアップを生成
            Vector3 popupPos = transform.position + Vector3.up * 2f;
            GameObject popupObj = Instantiate(damagePopupPrefab, popupPos, Quaternion.identity);
            currentPopup = popupObj.GetComponent<DamagePopupController>();

            currentPopup.AddDamage(damageAmount, isCritical);

        }
    }

    public override void Attack() { }
    public override void Dead() { }
    public override void HealHp() { }
    public override void Move() { }
}
