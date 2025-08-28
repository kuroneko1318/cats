using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartOnClic() {
        SceneManager.LoadScene("maingame");
    }

    public void EndOnClic() {
        Application.Quit();
    }
}
