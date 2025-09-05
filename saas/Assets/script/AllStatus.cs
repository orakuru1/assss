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
    public float speed = 2f;  //素早さ
    public int currentExp = 0; //初期経験値
    public int expToNextLevel = 10; //次のレベルが上がるまでの数

    public Slider hpSlider;
    public Slider expSlider;
    public GameObject levelImage;  //レベルが上がった時ステータスが上がるのを分かりやすくするimage
    public Text hptext;
    public Text hpText;
    public Text atktext;
    public Text deftext;
    public Text spdtext;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        if(levelImage != null) levelImage.SetActive(false);
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
        speed += 1f;
        currentHP = maxHP; //レベルアップ時に全回復
        expToNextLevel += 5; //次のレベルに必要な経験値を増やす
        if(levelImage != null) levelImage.SetActive(true); //レベルアップした場合ステータスの上昇テキスト表示

        Debug.Log("レベルアップ! 現在のレベル" + level);
        UpdateHPBar(currentHP);
        UpdateStatusTextWithBonus(maxHP, attack, defense, speed);
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

    void Update()
    {
        if(Input.GetMouseButton(0))

        {
            if(levelImage != null) levelImage.SetActive(false);  //クリックでテキスト閉じる
        }
    }

    void UpdateStatusTextWithBonus(float maxHP, float attack, float defense, float speed)
    {
        if(hpText) hpText.text = $"{maxHP}";
        if(atktext) atktext.text = $"{attack}";
        if(deftext) deftext.text = $"{defense}";
        if(spdtext) spdtext.text = $"{speed}";

        StartCoroutine(RemoveBonusText());
    }

    IEnumerator RemoveBonusText()
    {
        yield return new WaitForSeconds(3f);

        if(hpText) hpText.text = $"{maxHP}";
        if(atktext) atktext.text = $"{attack}";
        if(deftext) deftext.text = $"{defense}";
        if(spdtext) spdtext.text = $"{speed}";
    }

}
