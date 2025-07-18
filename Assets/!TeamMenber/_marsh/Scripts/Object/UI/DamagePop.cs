using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour {
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float disappearTime = 1f;

    public void Setup(int damageAmount) {
        textMesh.text = damageAmount.ToString();
        Destroy(gameObject, disappearTime);
    }

    private void Update() {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
