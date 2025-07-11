using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllAttack : MonoBehaviour
{
    public List<EnemyStatus> enemies = new List<EnemyStatus>();
    private Animator anim;
    public AllStatus allyStatus;  //味方のステータス（インスペクターで指定）
    public EnemyStatus currentTarget; //攻撃対処の敵

    //public TurnSystem turnsystem;

    public Canvas canva;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        //SelectNextTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAttackButton()
    {
        if(allyStatus != null && currentTarget != null)
        {
            FaceTarget(currentTarget.transform); //攻撃する際敵の方向を向く
            anim.SetTrigger("Attack");
            canva.enabled = false;
            float attackPower = allyStatus.attack; //味方の攻撃力取得
            currentTarget.TakeDamage(attackPower); //敵にダメージを与える
            
            if(!currentTarget.IsAlive())
            {
                enemies.Remove(currentTarget);
               
                //SelectNextTarget();
            }
        }
    }

    public void TwoAttackButton()
    {
        if(allyStatus != null && currentTarget != null)
        {
            FaceTarget(currentTarget.transform); //攻撃する際敵の方向を向く
            anim.SetTrigger("kenAttack");
            canva.enabled = false;
            float attackPower = allyStatus.attack; //味方の攻撃力取得
            currentTarget.TakeDamage(attackPower * 2); //敵にダメージを与える
            
            if(!currentTarget.IsAlive())
            {
                enemies.Remove(currentTarget);
               
                //SelectNextTarget();
            }
        }
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; //上下は固定

        if(direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }
    }

    public void SetTarget(EnemyStatus target)
    {
        currentTarget = target;
        Debug.Log("ターゲット変更");
    }

    /*void SelectNextTarget()
    {
        currentTarget = enemies.Find(e => e != null && e.IsAlive());

        if(currentTarget == null)
        {
            Debug.Log("すべての敵を倒しました");
            canva.enabled = false; //もう攻撃できない
        }
    }*/
}
