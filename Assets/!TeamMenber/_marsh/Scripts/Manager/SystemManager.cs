using UnityEngine;

public class SystemManager : MonoBehaviour {
    void Awake() {
        // 各マネージャーのインスタンス化（アクセスするだけで生成される）
        var gameManager = GameManager.Instance;
        var playerManager = PlayerManager.Instance;
        var resourceManager = ResourceManager.Instance;
        var monsterManager = MonsterManager.Instance;
        var inventoryManager = InventoryManager.Instance;
        var craftingManager = CraftingManager.Instance;
        var uiManager = UIManager.Instance;
        var audioManager = AudioManager.Instance;
        var environmentManager = EnvironmentManager.Instance;
        var saveLoadManager = SaveLoadManager.Instance;

        // 必要に応じて初期化処理を呼び出す(Initialize)

    }
}
