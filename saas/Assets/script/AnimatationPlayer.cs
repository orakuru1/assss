using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatationPlayer : MonoBehaviour
{
    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();

        anim["Attack"].wrapMode = WrapMode.Once; //一回しか再生されない
    }


    public void OnAttackButtonPressed()
    {
        if(!anim.isPlaying /*連打対策*/)
        {
            anim.Play("Attack");
        }
    }
}