using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatationPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private bool isWalking = false;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
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
}