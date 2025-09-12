using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public GridManager gridManager;
    public Slider hpSlider;
    public Text hptext;

    [System.Serializable]
    public class UnitStatus
    {
        public string unitName = "���j�b�g��";
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
        //���x���A�b�v���ɏオ��X�e�[�^�X�̔{�����L�������Ƃɕς����
        public List<float> levelUpMultipliers = new List<float> { 1f, 1.1f, 1.2f, 1.3f };
        public AttackPatternBase attackPattern;
    
            // �U���͈͎擾
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
    private Vector3 originalPosition; // �ړ��O�̈ʒu

    #region  //�}�X���ʂ̓K�p
    private int baseMaxHP, baseCurrentHP,
                baseAttack, baseDefense,
                baseMoveRange, baseAttackRange,
                baseSpeed, baseLuck;
    private float baseMaxStepHeight;

    //�}�X���ʂ��󂯂�O�̃X�e�[�^�X
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
        UpdateHPBar(status.currentHP);
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

        // ���Z�b�g
        ResetToBase();

        // ���ʂ��ēK�p
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
            currentBlock.UpdateOccupant(null);
        }

        transform.position = targetPosition;

        Vector2Int newGridPos = gridManager.GetGridPosition(targetPosition);
        GridBlock newBlock = gridManager.GetBlock(newGridPos);
        if (newBlock != null)
        {
            newBlock.UpdateOccupant(this);
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

            // �e�}�X���Ƃɏ����҂i�����Ă銴�j
            yield return new WaitForSeconds(0.05f);
        }

        // �Ō�� occupantUnit ���X�V
        Vector2Int finalPos = gridManager.GetGridPosition(transform.position);
        GridBlock finalBlock = gridManager.GetBlock(finalPos);
        if (finalBlock != null)
        {
            finalBlock.occupantUnit = this;
        }
        TurnManager.Instance.HighlightCurrentUnitMoveRange();
        if(finalBlock.occupantUnit == true)
        {
            
            TurnManager.Instance.OnPlayerMoveComplete(); //�����Œʒm
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

    public void AreaAttack(List<GridBlock> targetBlocks)
    {
        foreach (var block in targetBlocks)
        {
            if (block.occupantUnit != null && block.occupantUnit.team != this.team)
            {
                int damage = Mathf.Max(status.attack - block.occupantUnit.status.defense, 1);
                block.occupantUnit.TakeDamage(damage);
            }
        }

        // �o���l�t�^�Ƃ��͈͍U����p�̉��o������΂����ɒǉ�
        status.exp += 30;
        if (status.exp >= 100)
        {
            status.exp = 0;
            LevelUp();
        }
    }
    public void PerformAttack(GridManager gridManager)
    {
        Vector2Int pos = gridManager.GetGridPosition(transform.position);
        var attackBlocks = gridManager.GetAttackableBlockss(pos, status.attackPattern);

        if (status.attackPattern.isAreaAttack)
        {
            // �͈͍U��
            AreaAttack(attackBlocks);
        }
        else
        {
            // �P�̍U���i��: �ŏ��Ɍ��������G���U���j
            foreach (var block in attackBlocks)
            {
                if (block.occupantUnit != null && block.occupantUnit.team != this.team)
                {
                    Attack(block.occupantUnit);
                    break;
                }
            }
        }
    }


    public void TakeDamage(int damage)
    {
        int actualDamage = damage;
        status.currentHP -= actualDamage;
        UpdateHPBar(status.currentHP);
        Debug.Log($"{status.unitName} �� {actualDamage} �_���[�W���󂯂��I�i�cHP: {status.currentHP}�j");

        if (status.currentHP <= 0)
        {
            Die();
        }
        
    }

    public void Heal(int amount)
    {
        status.currentHP = Mathf.Clamp(status.currentHP + amount, 0, status.maxHP);
        Debug.Log($"{status.unitName} �� {amount} �񕜂����I�i����HP: {status.currentHP}�j");
    }

    public void LevelUp()
    {
        status.level++;

        float multiplier = status.levelUpMultipliers[Mathf.Clamp(status.level, 0, status.levelUpMultipliers.Count - 1)];

        status.maxHP = Mathf.RoundToInt(baseMaxHP * multiplier);
        status.attack = Mathf.RoundToInt(baseAttack * multiplier);

        Awake(); // �V����base����ēK�p
        
    }

    void UpdateHPBar(float currentHP)
    {
        if(hpSlider != null)
        {
            hpSlider.value = (float)status.currentHP / (float)status.maxHP;
        }

        if(hptext != null)
        {
            hptext.text = Mathf.CeilToInt(status.currentHP) + "/" + Mathf.CeilToInt(status.maxHP);
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

        Debug.Log($"{status.unitName} �͌��j����܂����I");
        TurnManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
    }

    public void ApplyMoveRangeBonus(int amount)
    {
        baseMoveRange += amount;
    }

    public void CancelMove()
    {
        InputHandler.Instance.ClearHighlights();
        InputHandler.Instance.ClearAllHighlights();

        Vector2Int gridPos = gridManager.GetGridPosition(transform.position);
        GridBlock block = gridManager.GetBlock(gridPos);
        block.occupantUnit = null;

        transform.position = originalPosition;

        ResetToBase();
        // occupantUnit �̍ēo�^�������K�v�i�d�v�j
        gridPos = gridManager.GetGridPosition(transform.position);
        block = gridManager.GetBlock(gridPos);
        if (block != null)
        {
            block.occupantUnit = this;
            UpdateBlockEffect();
        }
        Debug.Log("�ړ����L�����Z�����܂���");
        InputHandler.Instance.ClearHighlights();
    }

    void Start()
    {
        Vector2Int gridPos = gridManager.GetGridPosition(transform.position);
        GridBlock block = gridManager.GetBlock(gridPos);
        if (block != null)
        {
            block.occupantUnit = this;
        }
        UpdateHPBar(status.currentHP);
    }
}
