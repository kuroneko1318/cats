using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenu : MonoBehaviour
{
    private const int _WEAPON_ID=1001;

    private string inputItemString;
    [SerializeField]
    private TMP_InputField EquipmentinputField;
    public ItemManager itemManager;
    public ItemManager weaponManager;
    WeaponBase weapon;
    Inventory inventory;
    [SerializeField]RawImage swordImage;
    
    private bool equiping;
    // Start is called before the first frame update
    void Start()
    {
        if (inventory == null) {
            inventory = GameObject.FindGameObjectWithTag("Bag").GetComponent<Inventory>();
            
        }
        itemManager.Initialize();
        inventory.AddItem(ItemManager.Instance.GetWeaponByID(_WEAPON_ID));
        equiping = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.E)) {
            ScanString();
            if (equiping) {
                Equipment(inputItemString);
                swordImage.gameObject.SetActive(true);
            }
            else {
                NotEquipment();
                swordImage.gameObject.SetActive(false);
            }
        }
    }


    private void Equipment(string EquipmentName) {
        if (inventory == null) {
            inventory = GameObject.FindGameObjectWithTag("Bag").GetComponent<Inventory>();
            
        }

         weapon = weaponManager.GetWeaponByName(EquipmentName);
       if (weapon == null) return;
        inventory.RemoveItem(weapon);
        equiping = false;
    }

    private void NotEquipment() {
        if (inventory == null) {
            inventory = GameObject.FindGameObjectWithTag("Bag").GetComponent<Inventory>();
        }
        

        inventory.AddItem(ItemManager.Instance.GetItemByID(_WEAPON_ID));
        equiping = true;
    }



    public void ScanString() {
        inputItemString=EquipmentinputField.text;
    }
}
