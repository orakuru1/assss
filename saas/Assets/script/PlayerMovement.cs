using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask clickableLayer; //Inspectorで設定する用
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //レイヤーでヒット判定を制限 Enemy除外
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayer))
            {
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, targetPosition) < 0.1f)
            isMoving = false;
        }
    }
}
