using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Unit : MonoBehaviour
{
    public GridManager gridManager;

    [System.Serializable]
    public class UnitStatus
    {
        public string unitName = "ユニット名";
        public int level = 1;
        public int maxHP = 20;
        public int currentHP = 20;
        public int attack = 5;
        public int defense = 3;
        public int moveRange = 3;
        public int attackRange = 1;
        public float maxStepHeight = 0.5f;
        public int speed = 4;
        public int luck = 1;
    }

    public UnitStatus status = new UnitStatus();

    public enum Team
    {
        Player,
        Enemy
    }

    public Team team;
    public float moveSpeed = 2.0f;
    private bool isMoving = false;
    public List<GridBlock> movableBlocks = new List<GridBlock>();


    public void MoveTo(Vector3 targetPosition)
    {
        Vector2Int currentGridPos = gridManager.GetGridPosition(transform.position);
        GridBlock currentBlock = gridManager.GetBlock(currentGridPos);
        if (currentBlock != null && currentBlock.occupantUnit == this)
        {
            currentBlock.occupantUnit = null;
        }

        transform.position = targetPosition;

        Vector2Int newGridPos = gridManager.GetGridPosition(targetPosition);
        GridBlock newBlock = gridManager.GetBlock(newGridPos);
        if (newBlock != null)
        {
            newBlock.occupantUnit = this;
        }
    }

    public void MoveToPath(List<GridBlock> path)
    {
        StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<GridBlock> path)
    {
        foreach (var block in path)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = block.transform.position;
            float elapsed = 0f;
            float duration = 0.2f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            // 各マスごとに少し待つ（歩いてる感）
            yield return new WaitForSeconds(0.05f);
        }

        // 最後に occupantUnit を更新
        Vector2Int finalPos = gridManager.GetGridPosition(transform.position);
        GridBlock finalBlock = gridManager.GetBlock(finalPos);
        if (finalBlock != null)
        {
            finalBlock.occupantUnit = this;
        }
        TurnManager.Instance.HighlightCurrentUnitMoveRange();
    }





    public void Attack(Unit target)
    {
        int damage = Mathf.Max(status.attack - target.status.defense, 1);
        target.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = damage;
        status.currentHP -= actualDamage;

        Debug.Log($"{status.unitName} は {actualDamage} ダメージを受けた！（残HP: {status.currentHP}）");

        if (status.currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Vector2Int pos = gridManager.GetGridPosition(transform.position);
        GridBlock block = gridManager.GetBlock(pos);
        if (block != null && block.occupantUnit == this)
        {
            block.occupantUnit = null;
        }

        Debug.Log($"{status.unitName} は撃破されました！");
        gameObject.SetActive(false);
    }

    void Start()
    {
        Vector2Int gridPos = gridManager.GetGridPosition(transform.position);
        GridBlock block = gridManager.GetBlock(gridPos);
        if (block != null)
        {
            block.occupantUnit = this;
        }
    }
}
