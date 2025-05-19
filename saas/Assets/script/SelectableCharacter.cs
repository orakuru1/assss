using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableCharacter : MonoBehaviour
{
    [SerializeField]private GameObject player;
    
    private Animator anim;

    public bool isSelected = false;
    private static SelectableCharacter currentSelected = null;//現在のプレイヤーを取る必要がある。
    // Start is called before the first frame update

    private void OnMouseDown()//プライベートでスタティックはどんな効果？　isSelectedはまだ使っていない？
    {
        if (currentSelected == this)
        {
            //２回目のクリックで攻撃
            Debug.Log("攻撃します");
            Attack();
            currentSelected = null;
            isSelected = false;
        }
        else
        {
            //1回目のクリックで選択
            if (currentSelected != null)
            {
                currentSelected.isSelected = false;
            }

            currentSelected = this;
            isSelected = true;
            Debug.Log("選択しました");
        }
    }

    private void Attack()
    {
        // 攻撃処理をここに追加
        Vector3 attackPosition = player.transform.position; //攻撃するキャラの位置
        Vector3 attackDirection = this.transform.position; //攻撃されるキャラの位置
        Vector3 attackVector = attackDirection - attackPosition; //攻撃するための向き
        attackVector.y = 0; //y軸の移動は無視

        if (attackVector != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(attackVector);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, 1f);
        }


        
        Debug.Log($"{gameObject.name}に攻撃！");
        anim.SetTrigger("Attack");
    }
    void Start()
    {

        anim = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
