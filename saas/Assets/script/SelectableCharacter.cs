using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectableCharacter : MonoBehaviour
{
    [SerializeField]private GameObject player;
    [SerializeField] private GameObject StatusCanvas;
    [SerializeField] private GameObject ClearCanvas;

    private Animator anim;
    public Animator enemyAnim;
    public Animator ClearAnim;

    public bool isSelected = false;
    private static SelectableCharacter currentSelected = null;//現在のプレイヤーを取る必要がある。

    private bool isAnimating = false;
    
    public Slider targetSlider; //減らす対象のスライダー
    public Slider targetSlider1;
    public float decreaseAmout = 1; //ボタンを押すごとに減らす量
    public float animationDuration = 0.5f; //アニメーションの時間（秒）
    // Start is called before the first frame update

/*
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
*/

    private void Attack()
    {
        // 攻撃処理をここに追加
        Vector3 attackPosition = player.transform.position; //攻撃するキャラの位置
        Vector3 attackDirection = this.transform.position; //攻撃されるキャラの位置
        Vector3 attackVector = attackDirection - attackPosition; //攻撃するための向き
        attackVector.y = 0; //y軸の移動は無視

        if (attackVector != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(attackVector);//攻撃するキャラの向きがわかる
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, 1f);//プレイヤーが敵の方向を動いて向く
        }

        Debug.Log($"{gameObject.name}に攻撃！");
        anim.SetTrigger("Attack");//攻撃アニメーションをトリガー

        //anim.SetTrigger("Attack");
    }

    public void OnAttackAnimationEnd()
    {
        Debug.Log("攻撃アニメーションが終了！敵をヒットさせます");

        if (enemyAnim != null)
        {
            enemyAnim.SetTrigger("Hit"); // 敵の「Hit」アニメーションを再生
        }
        
        if (targetSlider1 != null && !isAnimating)//HPが減る処理は停止
        {
            //float targetValue = Mathf.Max(targetSlider1.minValue, targetSlider1.value - decreaseAmout);
            //StartCoroutine(AnimateSliderDecrease1(targetSlider1.value, targetValue));
        }
    }
    
/*
        public void OnpushHit()
        {
            anim.SetTrigger("Hit");
            if (targetSlider != null && !isAnimating)
            {
                float targetValue = Mathf.Max(targetSlider.minValue, targetSlider.value - decreaseAmout);
                StartCoroutine(AnimateSliderDecrease(targetSlider.value, targetValue));
            }
        }
    */

    private IEnumerator AnimateSliderDecrease(float startValue, float endValue)
    {
        isAnimating = true;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            targetSlider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        targetSlider.value = endValue;
        isAnimating = false;
    }

    private IEnumerator AnimateSliderDecrease1(float startValue, float endValue)
    {
        isAnimating = true;

        float elapsed = 0f;

        while(elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            targetSlider1.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        targetSlider1.value = endValue;
        if (targetSlider1.value <= 0)
        {
            ClearCanvas.SetActive(true);
            ClearAnim.SetTrigger("Clear");
        }
        isAnimating = false;
    }

    void Start()
    {
        anim = player.GetComponent<Animator>();
        if (StatusCanvas != null)
        {
            //StatusCanvas.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
