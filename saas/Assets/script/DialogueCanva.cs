using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCanva : MonoBehaviour
{
    public Canvas dialogueCanvas; //会話Canvas
    public GameObject statusImage;   //ステータスimage
    public GameObject AttackImage; //攻撃image
    public EnemyStatus myStatus;


    private bool alreadyClicked = false;
    public static DialogueCanva currentOpenDialogue;

    // Start is called before the first frame update
    void Start()
    {
       //最初は非表示
       if(dialogueCanvas != null) dialogueCanvas.enabled = false;
       if(statusImage != null) statusImage.SetActive(false);
       if(AttackImage != null) AttackImage.SetActive(false);
    }

    private void OnMouseDown()
    {
        if(!alreadyClicked) //一度だけ表示したい場合
        {
            if(dialogueCanvas != null) dialogueCanvas.enabled = true;
            if(AttackImage != null) AttackImage.SetActive(true);
            alreadyClicked = true;
        }

        
        if(currentOpenDialogue != null && currentOpenDialogue != this)
        {
            currentOpenDialogue.HideStatusImage();
        }

        //自分のUIを表示
        ShowStatusImage();

        //現在開いてるUIを更新
        currentOpenDialogue = this;
        

        //alreadyClicked = true; //2回目以降表示しない

        AllAttack attacker = FindObjectOfType<AllAttack>();
        if(attacker != null && myStatus != null)
        {
            attacker.SetTarget(myStatus);
        }

    }

    public void ShowStatusImage()
    {
        if(statusImage != null)
        {
            statusImage.SetActive(true);
        }
    }

    public void HideStatusImage()
    {
        if(statusImage != null)
        {
            statusImage.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
