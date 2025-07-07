using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllStatus : MonoBehaviour
{
    public int level = 1; //キャラのレベル
    public float maxHP = 10f; //最大HP
    public float currentHP;
    public float attack = 2f; //攻撃力
    public float defense = 1f; //防御力
    public int currentExp = 0; //初期経験値
    public int expToNextLevel = 10; //次のレベルが上がるまでの数

    public Slider hpSlider;
    public Text hptext;
    public Slider expSlider;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        UpdateHPBar(currentHP);
    }

    //ダメージ処理
    public void TaKeDamage(float damage)
    {
        float actualDamage = Mathf.Max(0, damage - defense);
        currentHP -= actualDamage;
        currentHP = Mathf.Max(0, currentHP);
        UpdateHPBar(currentHP);
    }

    public void GainExp(int amount)
    {
        currentExp += amount;


        while(currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        UpdateExpSlider();
    }

    void LevelUp()
    {
        level++;
        maxHP += 2f;
        attack += 1f;
        defense += 0.5f;
        currentHP = maxHP; //レベルアップ時に全回復
        expToNextLevel += 5; //次のレベルに必要な経験値を増やす

        Debug.Log("レベルアップ! 現在のレベル" + level);
        UpdateHPBar(currentHP);
    }

    // Update is called once per frame
    void UpdateHPBar(float currentHP)
    {
        if(hpSlider != null)
        {
            hpSlider.value = currentHP / maxHP;
        }

        if(hptext != null)
        {
            hptext.text = Mathf.CeilToInt(currentHP) + "/" + Mathf.CeilToInt(maxHP);
        }
    }

    void UpdateExpSlider()
    {
        if(expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }
    }
}
