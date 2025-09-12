using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCount : MonoBehaviour
{
    // Start is called before the first frame update
    
    private Animator anim;
    private GameObject[] enemies;
    void Start()
    {
        anim = GetComponent<Animator>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // ← 初期化
                // 配列の長さがそのまま数になる
        int enemyCount = enemies.Length;

        Debug.Log("Enemyの数は: " + enemyCount);;
    }

    public void CountEnemies()
    {
                // "Enemy" というタグを持つオブジェクトを探す
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 配列の長さがそのまま数になる
        int enemyCount = enemies.Length;

        Debug.Log("Enemyの数は: " + enemyCount);;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Length == 1)
        {
            anim.SetTrigger("Clear");
        }

    }
}
