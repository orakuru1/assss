using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCanvas : MonoBehaviour
{
    public Canvas canvas; //変数
    // Start is called before the first frame update
    void Start()
    {
        canvas.enabled = false; //スタート時非表示
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Openattack()
    {
        canvas.enabled = !canvas.enabled; //非表示のCanvasを表示
    }

    public void kyaraClick()
    {
        canvas.enabled = !canvas.enabled; //非表示のCanvasを表示
    }

    public void onRetrun()
    {
        canvas.enabled = false;
    }
}
