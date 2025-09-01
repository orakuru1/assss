using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AnimatationPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private bool isWalking = false;

    public GameObject levelImage;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
    }

    void Update()
    {
        if(levelImage.activeSelf) return;
        if(Input.GetMouseButtonDown(0) && !IsPosinterOverUIObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
                anim.SetTrigger("Run");
                isWalking = true;
            }
        }

        //目的地についたら待機モーション
        if(isWalking && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                anim.SetTrigger("Idle");
                isWalking = false;
            }
        } 
    }
    public void OnAttackAnimationEnd()
    {
        // 攻撃後の処理（移動解除とか、次の行動許可とか）
        Debug.Log("攻撃アニメが終わったよ");
    }

    //UIの上をタップしているか判定
    private bool IsPosinterOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}