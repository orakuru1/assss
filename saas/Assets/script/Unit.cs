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
        public int exp = 0;
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
        //レベルアップ時に上がるステータスの倍率をキャラごとに変えれる
        public List<float> levelUpMultipliers = new List<float> { 1f, 1.1f, 1.2f, 1.3f };
        public AttackPatternBase attackPattern;
    
            // 攻撃範囲取得
            public List<Vector2Int> GetAttackRange(Vector2Int currentPos)
            {
                return attackPattern.GetPattern(currentPos);
            }
        
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
    private Vector3 originalPosition; // 移動前の位置

    #region  //マス効果の適用
    private int baseMaxHP, baseCurrentHP,
                baseAttack, baseDefense,
                baseMoveRange, baseAttackRange,
                baseSpeed, baseLuck;
    private float baseMaxStepHeight;

    //マス効果を受ける前のステータス
    private void Awake()
    {
        baseMaxHP = status.maxHP;
        baseCurrentHP = status.currentHP;
        baseAttack = status.attack;
        baseDefense = status.defense;
        baseMoveRange = status.moveRange;
        baseAttackRange = status.attackRange;
        baseMaxStepHeight = status.maxStepHeight;
        baseSpeed = status.speed;
        baseLuck = status.luck;
        gridManager = FindObjectOfType<GridManager>();
    }

    
    void Update()
    {
        UpdateBlockEffect();
    }
    public void ResetToBase()
    {
        status.maxHP = baseMaxHP;
        status.attack = baseAttack;
        status.defense = baseDefense;
        status.moveRange = baseMoveRange;
        status.attackRange = baseAttackRange;
        status.maxStepHeight = baseMaxStepHeight;
        status.speed = baseSpeed;
        status.luck = baseLuck;
    }

    void UpdateBlockEffect()
    {
        Vector2Int currentPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );

        GridBlock currentBlock = gridManager.GetBlock(currentPos);

        // リセット
        ResetToBase();

        // 効果を再適用
        if (currentBlock != null)
        {
            switch (currentBlock.blockKinds)
            {
                case GridBlock.BlockKinds.sand:
                    status.attack += 10;
                    break;
                case GridBlock.BlockKinds.glass:
                    status.moveRange -= 1;
                    break;
            }
        }
    }
    #endregion

    public void MoveTo(Vector3 targetPosition)
    {
        originalPosition = transform.position;
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

    public IEnumerator MoveToPath(List<GridBlock> path)
    {
        yield return StartCoroutine(MoveAlongPath(path));
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
        if(finalBlock.occupantUnit == true)
        {
            
            TurnManager.Instance.OnPlayerMoveComplete(); //ここで通知
        }

    }

    public void Attack(Unit target)
    {
        int damage = Mathf.Max(status.attack - target.status.defense, 1);
        target.TakeDamage(damage);
        status.exp += 30;
        if(status.exp >= 100)
        {
            status.exp = 0;
            LevelUp();
        }
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

    public void LevelUp()
    {
        status.level++;

        float multiplier = status.levelUpMultipliers[Mathf.Clamp(status.level, 0, status.levelUpMultipliers.Count - 1)];

        status.maxHP = Mathf.RoundToInt(baseMaxHP * multiplier);
        status.attack = Mathf.RoundToInt(baseAttack * multiplier);

        Awake(); // 新しいbaseから再適用
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

    public void CancelMove()
    {
        InputHandler.Instance.ClearHighlights();
        InputHandler.Instance.ClearAllHighlights();

        transform.position = originalPosition;

        ResetToBase();
        // occupantUnit の再登録処理も必要（重要）
        Vector2Int gridPos = gridManager.GetGridPosition(transform.position);
        GridBlock block = gridManager.GetBlock(gridPos);
        if (block != null)
        {
            block.occupantUnit = this;
            UpdateBlockEffect();
        }
        Debug.Log("移動をキャンセルしました");
        InputHandler.Instance.ClearHighlights();
        InputHandler.Instance.ShowMoveRange(this);
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
