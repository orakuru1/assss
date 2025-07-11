using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public Transform normalView; //通常カメラ位置
    public Transform battleView; //戦闘カメラ位置
    public float transitionTime = 1f; //カメラ移動にかかる時間

    private bool isInBattale = false;
    private Coroutine currentTransition;

    // Start is called before the first frame update
    void Start()
    {
        //初期位置にカメラを移動
        if(normalView != null)
        {
            transform.position = normalView.position;
            transform.rotation = normalView.rotation;
        }
    }

    public void EnterBattleView()
    {
        if(!isInBattale)
        {
            isInBattale = true;
            StartTransition(battleView);
        }
    }

    public void ExitBattleView()
    {
        if(isInBattale)
        {
            isInBattale = false;
            StartTransition(normalView);
        }
    }

    private void StartTransition(Transform targetView)
    {
        if(currentTransition != null) StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(MoveCamera(targetView));
    }

    private IEnumerator MoveCamera(Transform targetView)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = targetView.position;
        Quaternion endRot = targetView.rotation;

        float elapsed = 0f;

        while(elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionTime;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
