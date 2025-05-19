using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kariMove : MonoBehaviour
{
    public float moveSpeed = 2f; // 移動速度（ユニット/秒）
    private bool isMoving = false;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isMoving)
        {
            Vector3 direction = transform.forward;
            Vector3 targetPosition = transform.position + direction * 3f;
            targetPosition.y = transform.position.y;

            StartCoroutine(MoveToPosition(targetPosition));
        }

    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        anim.SetTrigger("Run");
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null; // 次のフレームまで待つ
        }

        transform.position = target; // 誤差調整
        isMoving = false;
        anim.SetTrigger("Idle");
    }
}
