using UnityEngine;
using System.Collections.Generic;

public class SystemManager : MonoBehaviour {
    [Header("SystemObject プレハブ一覧")]
    [SerializeField] private List<GameObject> systemObjectPrefabs;


    void Awake() {
        DontDestroyOnLoad(gameObject);

        foreach (var prefab in systemObjectPrefabs) {
            if (prefab == null) continue;

            GameObject instance = Instantiate(prefab);
            DontDestroyOnLoad(instance);

            // すべての MonoBehaviour を取得して確認
            var monoBehaviours = instance.GetComponents<MonoBehaviour>();
            foreach (var mono in monoBehaviours) {
                var type = mono.GetType();
                var baseType = type.BaseType;

                // SystemObject<> を継承しているか確認
                while (baseType != null) {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(SystemObject<>)) {
                        var method = type.GetMethod("Initialize");
                        method?.Invoke(mono, null);
                        break;
                    }
                    baseType = baseType.BaseType;
                }
            }
        }
    }

}
