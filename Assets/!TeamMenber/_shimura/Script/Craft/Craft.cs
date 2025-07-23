using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Craft : MonoBehaviour
{
    private string inputItemString1;
    private string inputItemString2;
    [SerializeField]
    CraftingManager crManager;
   
    //Inventory inventory=null;
    [SerializeField]
    private TMP_InputField inputField1;
    [SerializeField]
    private TMP_InputField inputField2;

    public void Start() {
        crManager.ItemInitialize();
    }

    public void Update() {

        if (Input.GetKeyDown(KeyCode.Q)) {
            Crafting();
            

        }

    }

    public void Crafting() {
        
        ScanString();

        if (inputItemString1 == null || inputItemString2 == null) {
            Debug.Log("‰½‚©“ü—Í‚µ‚ë");
            return;
        }

        crManager.CraftItem(inputItemString1,inputItemString2);
    }

    public void ScanString() {
        

        inputItemString1 = inputField1.text;
        inputItemString2 = inputField2.text;
    }
}
