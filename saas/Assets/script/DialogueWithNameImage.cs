using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueWithNameImage : MonoBehaviour
{
    [System.Serializable]
    public class Line
    {
        public string nameKey;   // "shado", "kashin" など
        public string text;      // セリフ本文
    }

    [System.Serializable]
    public class NameImageEntry
    {
        public string key;       // 名前のキー
        public Sprite sprite;    // 対応する画像
    }

    public Image nameImage;         // 名前画像を表示するImage
    public Text dialogueText;       // セリフ表示用Text

    public Line[] lines;
    public NameImageEntry[] nameImages;

    private int currentIndex = 0;

    void Start()
    {
        ShowCurrentLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // クリックで進む
        {
            currentIndex++;
            if (currentIndex < lines.Length)
            {
                ShowCurrentLine();
            }
            else
            {
                Debug.Log("会話終了");
            }
        }
    }

    void ShowCurrentLine()
    {
        Line current = lines[currentIndex];
        dialogueText.text = current.text;

        foreach (var entry in nameImages)
        {
            if (entry.key == current.nameKey)
            {
                nameImage.sprite = entry.sprite;
                return;
            }
        }

        Debug.LogWarning($"キー '{current.nameKey}' に対応する名前画像がありません。");
    }

    //UIの上をタップしているか判定
    private bool IsPosinterOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
