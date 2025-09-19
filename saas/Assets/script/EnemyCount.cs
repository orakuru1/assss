using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EnemyCount : MonoBehaviour
{
    // Start is called before the first frame update
    private int enemyCount;
    public int StageNum = 0;

    private Animator anim;
    [SerializeField] private Animator textanim;

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] SecondEnemies; // 第二ステージの敵オブジェクトへの参照
    [SerializeField] private GameObject player; // プレイヤーオブジェクトへの参照
    [SerializeField] private GameObject lowding; // ローディング画面への参照
    [SerializeField] private GameObject MoveButtonCanvas; // 行動ボタンのUI
    [SerializeField] private GameObject PlayerStatusUI; // プレイヤーステータスUI

    [SerializeField] private Camera mainCamera; // メインカメラへの参照

    [SerializeField] private VideoPlayer videoPlayer;// ロード画面を動かすのに参照

    //ロード画面のプレイを自分で決める
    //数秒たったら、ロード画面を非表示にして動画を停止。→また最初から流れるのか？

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

        Debug.Log("Enemyの数は: " + enemyCount); ;
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
                videoPlayer.Play(); // 動画を再生する
                videoPlayer.loopPointReached += VideoEnd; // 動画終了時のイベントを登録
                MoveButtonCanvas.SetActive(false); // 行動ボタンのUIを非表示にする
                PlayerStatusUI.SetActive(false); // プレイヤーステータスUIを非表示にする

                Debug.Log("右クリックされました");
                anim.SetTrigger("NextStage");
                textanim.SetTrigger("end");
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

    //ロード動画が終わったらの処理
    void VideoEnd(VideoPlayer vp)
    {
        lowding.SetActive(false); // ローディング画面を非表示にする
        //videoPlayer.Stop(); // 動画を停止する
        MoveButtonCanvas.SetActive(true); // 行動ボタンのUIを再表示する
        PlayerStatusUI.SetActive(true); // プレイヤーステータスUIを再表示する
    }

    public void texttenmetu()
    {
        textanim.SetTrigger("start");
    }
}
