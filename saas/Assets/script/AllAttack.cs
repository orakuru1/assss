using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllAttack : MonoBehaviour
{
    private Animator anim;
    public AllStatus allyStatus;  //味方のステータス（インスペクターで指定）
    public EnemyStatus targetEnemy; //攻撃対処の敵

    public Canvas canva;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAttackButton()
    {
        if(allyStatus != null && targetEnemy != null)
        {
            anim.SetTrigger("Attack");
            canva.enabled = false;
            float attackPower = allyStatus.attack; //味方の攻撃力取得
            targetEnemy.TakeDamage(attackPower); //敵にダメージを与える
        }
    }
}
