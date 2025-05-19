using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class typewriter : MonoBehaviour
{
    public Text dialogueText;
    public float letterInterval = 0.05f;

    public void ShowText(string fulltext)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(fulltext));
    }

    IEnumerator TypeText(string fulltext)
    {
        dialogueText.text = "";
        foreach(char c in fulltext)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(letterInterval);
        }
    }
}
