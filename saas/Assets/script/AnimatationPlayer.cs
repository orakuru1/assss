using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatationPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
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
            }
        }

        float speed = agent.velocity.magnitude;
        if (speed < 0.1f)
            anim.SetFloat("Run", 0f);  // Idle
        else
            anim.SetFloat("Run", 1f);  // Run
    }
}