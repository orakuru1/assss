using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class kyaraAttack : MonoBehaviour
{
    private Animator anim;
    private bool isAnimating = false;
    
    public Slider targetSlider; //防御時に減らす対象のスライダー
    public Slider targetSlider1;//攻撃時に減らすスライダー
    public float decreaseAmout = 1f; //ボタンを押すごとに減らす量
    public float decreaseAmout1 = 0.5f;
    public float animationDuration = 0.5f; //アニメーションの時間（秒）

    public Canvas canvas; //変数


    // Start is called before the first frame update

    void Start()
    {
        
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnpushAttack()
    {
        anim.SetTrigger("Attack");
        canvas.enabled = false;
        if(targetSlider1 != null && !isAnimating)  //targetSlider1のHPが減る
        {
            float targetValue = Mathf.Max(targetSlider1.minValue, targetSlider1.value - decreaseAmout);
            StartCoroutine(AnimateSliderDecrease1(targetSlider1.value, targetValue));//コルーチンを使い滑らかにスライダーを減らす
        }
    }

    public void OnpushHit()
    {
        anim.SetTrigger("Block");
       /* if(targetSlider != null && !isAnimating)
        {
            float targetValue = Mathf.Max(targetSlider.minValue, targetSlider.value - decreaseAmout1);
            StartCoroutine(AnimateSliderDecrease(targetSlider.value, targetValue));
        }*/
    }

    private IEnumerator AnimateSliderDecrease(float startValue, float endValue)
    {
        isAnimating = true;

        float elapsed = 0f;

        while(elapsed < animationDuration)
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
        isAnimating = false;
    }
    
}
