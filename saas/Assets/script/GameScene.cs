using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    private bool firstPush = false; //スタート
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressStart()
    {
        if(!firstPush)
        {
            SceneManager.LoadScene("統合用シーン");
            firstPush = true;
        }
    }
}
