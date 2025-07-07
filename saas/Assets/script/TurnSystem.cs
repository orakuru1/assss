using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class TurnSystem : MonoBehaviour
{
    public TurnState currentTurn = TurnState.PlayerTurn;

    public AllStatus enemy; //敵AIスクリプト
    public GameObject playerUI; //攻撃ボタンなど

    public void EndPlayerTurn()
    {
        currentTurn = TurnState.EnemyTurn;
        playerUI.SetActive(false); //プレイヤ操作無効
        StartCoroutine(EnemyAction());
    }

    public IEnumerator EnemyAction()
    {
        yield return new WaitForSeconds(1f); //少し間を開ける

        enemy.TaKeDamage(1f); //敵の自動攻撃
        
        yield return new WaitForSeconds(1f);

        currentTurn = TurnState.PlayerTurn;
        playerUI.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
