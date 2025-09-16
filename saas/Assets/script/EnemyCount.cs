using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCount : MonoBehaviour
{
    // Start is called before the first frame update
    private int enemyCount;
    public int StageNum = 0;

    private Animator anim;
    [SerializeField] private GameObject[] enemies;

    [SerializeField] private GameObject player; // プレイヤーオブジェクトへの参照
    [SerializeField] private GameObject[] SecondEnemies; // 第二ステージの敵オブジェクトへの参照

    [SerializeField] private Camera mainCamera; // メインカメラへの参照

    [SerializeField] private GameObject lowding; // ローディング画面への参照


    void Start()
    {
        anim = GetComponent<Animator>();
        EnemySecrch();
        lowding.SetActive(false); // ローディング画面を非表示にする
    }

    public void EnemySecrch()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // ← 初期化
                // 配列の長さがそのまま数になる
        enemyCount = enemies.Length;

        Debug.Log("Enemyの数は: " + enemyCount);;
    }


    public void CountEnemies()//enemytagを持つオブジェクトを持ってきて、それがenemyだったら、数を減らすようにしたほうが良いと思う
    {
        // 配列の長さがそのまま数になる
        enemyCount -= 1;

        Debug.Log("Enemyの数は: " + enemyCount);
        if (enemyCount <= 0)
        {
            Debug.Log("全ての敵を倒しました。");
            anim.SetTrigger("Clear");
        }
    }

    // Update is called once per frame
    void Update()//動画の時間が途中で止まるようにする。動画が勝手に再生されちゃってるから、自分で再生を操作するようにする。
    {
        if (enemyCount <= 0)
        {
            if (Input.GetMouseButtonDown(1))
            {
                lowding.SetActive(true); // ローディング画面を表示する
                Debug.Log("左クリックされました");
                anim.SetTrigger("NextStage");
                //主人公と仲間たちを次のステージに送るVector3(-105,4.5,-100.033997)
                player.transform.position = new Vector3(-105.0f, 4.5f, -100.0f);
                mainCamera.transform.position = new Vector3(-105.0f, 12.0f, -97.0f);

                StageNum += 1;
                
            }

            if (StageNum == 1)
            {
                foreach (GameObject enemy in SecondEnemies)
                {
                    enemy.SetActive(true); // 敵オブジェクトをアクティブにする
                }

                //ここまでくる時点で、敵は全滅してるから、新しくする。
                EnemySecrch();
            }
        
        }

    }
}
