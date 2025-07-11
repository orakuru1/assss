using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    [Header("ステータス")]
    public float maxHP = 10f;
    public float currentHP;
    public float defense = 0f; //防御力
    public float attack = 3f; //攻撃力

    [Header("UI")]
    public Slider hpSlider;
    public Text hptext;
    public GameObject statusImage;

    private Animator anim;
    private Coroutine hpAnimCoroutine;
    public AllStatus player;
    public BattleCamera battalecamera;
    //public EnemySpawer spawner; //スポナーからセットされる

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        UpdateHPBar(currentHP);
    }

    ///<summary>
    /// 味方からダメージを受けた時に呼ぶ
    /// </summary>
    /// <param name = "amount">味方の攻撃力</param>
    public void TakeDamage(float amount)
    {
       

        //防御力を無視して実際のダメージを計算
        float actualDamage = Mathf.Max(0, amount - defense);

        //HPを減らす
        currentHP -= actualDamage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if(hpAnimCoroutine != null)
            StopCoroutine(hpAnimCoroutine);
            hpAnimCoroutine = StartCoroutine(AnimateHPBar());
        
        anim.SetTrigger("Hit");

        //HPが0になったら死亡処理
        if(currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator AnimateHPBar() //HPバーを滑らかに減らす
    {
        float duration = 0.5f;
        float startValue = hpSlider.value;
        float endValue = currentHP / maxHP;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            hpSlider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }

        hpSlider.value = endValue;
        if(hptext != null) UpdateHPBar(currentHP);
    }

    void Die()
    {
        anim.SetTrigger("DieHit");
        Destroy(gameObject, 3.0f);
        Destroy(statusImage, 3.0f);
        player.GainExp(10);
        battalecamera.ExitBattleView();

       /* if(spawner != null)
        {
            spawner.SpawnNextEnemy();
        }*/
    }

    // Update is called once per frame
    private void UpdateHPBar(float currentHP)
    {
        if(hpSlider != null)
        {
            hpSlider.maxValue = 1f;
            hpSlider.value = currentHP / maxHP;
        }

        if(hptext != null)
        {
            hptext.text = Mathf.CeilToInt(currentHP) + "/" + Mathf.CeilToInt(maxHP);
        }
    }

    public bool IsAlive()
    {
        return currentHP > 0;
    }
}
