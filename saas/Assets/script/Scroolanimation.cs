using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroolanimation : MonoBehaviour
{
     public RectTransform scrollImage;   // 掛け軸のRectTransform
    public Text turnText;               // ターン表示テキスト
    public GameObject canvasObject;     // 掛け軸を含むCanvas or パネル
    public float openDuration = 1.5f;   // 開く時間
    public float textFadeDuration = 0.5f; // テキストフェード時間
    public float targetWidth = 800f;    // 掛け軸の最終横幅
    public float displayTime = 2.0f;    // 表示しておく時間

    private float originalHeight;

    void Start()
    {
        // 高さを保存
        originalHeight = scrollImage.sizeDelta.y;

        // 初期状態（幅ゼロ）
        scrollImage.sizeDelta = new Vector2(0, originalHeight);

        // テキスト透明
        Color c = turnText.color;
        c.a = 0;
        turnText.color = c;

        // 再生開始
        StartCoroutine(PlayScrollAnimation("自分のターン"));
    }

    IEnumerator PlayScrollAnimation(string message)
    {
        // 横幅を伸ばす
        float t = 0;
        while (t < openDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / openDuration);
            float width = Mathf.Lerp(0, targetWidth, progress);
            scrollImage.sizeDelta = new Vector2(width, originalHeight);
            yield return null;
        }

        // テキストセット
        turnText.text = message;

        // フェードイン
        t = 0;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / textFadeDuration);
            Color c = turnText.color;
            c.a = alpha;
            turnText.color = c;
            yield return null;
        }

        // 表示時間待機
        yield return new WaitForSeconds(displayTime);

        // Canvasを非表示にする
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
        else
        {
            // このスクリプトが付いているオブジェクトを消す場合
            gameObject.SetActive(false);
        }
    }
}
