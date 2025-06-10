using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCanva : MonoBehaviour
{
    public Canvas dialogueCanvas; //会話Canvas
    public GameObject statusImage;   //ステータスCanvas

    private bool alreadyClicked = false;

    // Start is called before the first frame update
    void Start()
    {
       //最初は非表示
       if(dialogueCanvas != null) dialogueCanvas.enabled = false;
       if(statusImage != null) statusImage.SetActive(false);
    }

    void OnMouseDown()
    {
        if(alreadyClicked) return; //一度だけ表示したい場合

        if(dialogueCanvas != null) dialogueCanvas.enabled = true;
        if(statusImage != null) statusImage.SetActive(true);

        alreadyClicked = true; //2回目以降表示しない
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
