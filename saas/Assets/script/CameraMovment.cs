using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    public float mouseSensitivity = 0.1f; //マウスによる移動の感度

    private bool isRightMouseDown = false;
    private Vector3 lastMousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //右クリックの押下状態を確認
        if(Input.GetMouseButtonDown(1))
        {
            isRightMouseDown = true;
            lastMousePosition = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            isRightMouseDown =false;
        }

        if(isRightMouseDown)
        {
            //マウスの移動量を計算
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // カメラの右方向（X軸）と前方向（Z軸）を取得（Y=0で水平に補正）
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            right.y = 0;
            forward.y = 0;
            right.Normalize();
            forward.Normalize();

            // 水平移動ベクトルを作成
            Vector3 move = (-mouseDelta.x * right + -mouseDelta.y * forward) * mouseSensitivity;

            // Y軸を変えずに移動
            transform.position += move;
        }
    }
}
