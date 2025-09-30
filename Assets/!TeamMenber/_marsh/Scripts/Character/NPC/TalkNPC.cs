using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkNPC : MonoBehaviour {

    public GameObject Text;

    private Animator animator;

    public static TalkNPC Instance;

    private bool clearText = false;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Text.SetActive(false);
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        RandomTalkText();
        Text.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        Text.SetActive(false);
    }

    public void ChangeAnimation() {
        animator.SetBool("Clap",true);
        clearText = animator.GetBool("Clap");
    }

    private void RandomTalkText() {
        int rand = Random.Range(0, 7);

        switch (rand) {
            case 0:
                Text.GetComponent<TextMeshPro>().text = ("–‚Î‚ğg‚¤‚Æ\n‘•”õ‚ğ‹­‰»‚Å‚«‚é‚ç‚µ‚¢");
                break; 
            case 1:
                Text.GetComponent<TextMeshPro>().text = ("•R‚ğg‚¦‚Î–ò‚à•‚àì‚ê‚é‚¼");
                break; 
            case 2:
                Text.GetComponent<TextMeshPro>().text = ("–Ø‚ÌÀH‚×‚é‚Æ\n‚È‚ñ‚©—Í‚ª—N‚¢‚Ä‚­‚é‚È");
                break; 
            case 3:
                Text.GetComponent<TextMeshPro>().text = ("•—jÎ‚ğ¬‚º‚Ä\nˆù‚Ş‚È‚ñ‚Ä³‹C‚©H");
                break;
            case 4:
                Text.GetComponent<TextMeshPro>().text = ("•Ï‚í‚Á‚½Î‚ğ‘g‚İ‡‚í‚¹‚é‚Æ\n‹­‚»‚¤‚ÈÎ‚ªo—ˆ‚»‚¤‚¾‚È");
                break;
            case 5:
                if(!clearText) Text.GetComponent<TextMeshPro>().text = ("—nŠâ‚ÍƒCƒJ‚ê‚½‹­‚³‚µ‚Ä‚¢‚é\n“z‚ç‚ª‘òR—N‚¢‚Ä‚â‚ª‚é");
                else Text.GetComponent<TextMeshPro>().text = ("‚ ‚Ì‰»‚¯•¨‚ğˆê‘|‚µ‚½‚ª\n‚Ü‚½‹­‚­‚È‚Á‚Ä—N‚¢‚Ä‚«‚â‚ª‚é");
                break;
            case 6:
                if(!clearText) Text.GetComponent<TextMeshPro>().text = ("ƒCƒJ‚ê‚½“z‚ç‚ğ“|‚µ‚Ä‚«‚Ä‚­‚ê");
                else Text.GetComponent<TextMeshPro>().text = ("‚æ‚­‚â‚Á‚½I\nŒN‚Í‰p—Y‚¾I");
                break;
        }
    }

}
