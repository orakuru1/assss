using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCanva : MonoBehaviour
{
    public Canvas dialogueCanvas;
    // Start is called before the first frame update
    void Start()
    {
        if(dialogueCanvas != null)
        {
            dialogueCanvas.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(dialogueCanvas != null)
            {
                dialogueCanvas.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(dialogueCanvas != null)
            {
                dialogueCanvas.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
