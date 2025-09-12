using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    public EnemyAI enemyAI;
    public GridManager gridManager;
    public static TurnManager Instance { get; private set; }

    private Queue<Unit> turnQueue = new Queue<Unit>();
    public Unit CurrentUnit { get; private set; }

    public delegate void OnTurnStartDelegate(Unit unit);
    public static event OnTurnStartDelegate OnTurnStart;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeTurnOrder();
        StartNextTurn();
    }

    private List<Unit> allUnits = new List<Unit>(); // �ǉ�

    public void InitializeTurnOrder()
    {
        allUnits = FindObjectsOfType<Unit>().ToList();
        var sortedUnits = allUnits.OrderByDescending(u => u.status.speed).ToList();
        turnQueue = new Queue<Unit>(sortedUnits);
    }

    public void RemoveUnit(Unit unit)
    {
        allUnits.Remove(unit);

        // �L���[�̍č\�z�i�폜�Ώۂ��܂܂Ȃ��悤�Ɂj
        turnQueue = new Queue<Unit>(turnQueue.Where(u => u != unit));

        // ���݂̃��j�b�g�������瑦�I�����Ď���
        if (CurrentUnit == unit)
        {
            StartNextTurn();
        }
    }


    public void StartNextTurn()
    {
        inputHandler.ClearAllHighlights();
        

        if (turnQueue.Count == 0)
        {
            InitializeTurnOrder();
        }
        CurrentUnit = turnQueue.Dequeue();
        Vector2Int pos = CurrentUnit.gridManager.GetGridPosition(CurrentUnit.transform.position);
        GridBlock block = CurrentUnit.gridManager.GetBlock(pos);
        if (block != null)
        {
            block.UpdateOccupant(CurrentUnit);
        }
        OnTurnStart?.Invoke(CurrentUnit);;
        InputHandler.Instance.unit = CurrentUnit;
        Debug.Log($"���݂̍s�����j�b�g: {CurrentUnit.name}�i{CurrentUnit.team}�j");
        // ������UI�X�V��AI�N���Ȃǂ��Ă��悢
        // �����œG���������𔻒�
        if (CurrentUnit.team == Unit.Team.Enemy)
        {
            GridManager gridManager = FindObjectOfType<GridManager>();
            //enemyAI.HighlightEnemyMoveRange(CurrentUnit, gridManager);
            
            StartCoroutine(enemyAI.ExecuteEnemyTurn(CurrentUnit, FindObjectOfType<GridManager>()));
        }
        else
        {
            // �����̂Ƃ��͓��͎�t�Ȃ�
            //inputHandler.ShowMoveRange(CurrentUnit);

            //HighlightCurrentUnitMoveRange(); // �K�v�ł����
            InputHandler.Instance.ShowActionButtons();
        }
    }

    public void EndUnitTurn()
    {
        turnQueue.Enqueue(CurrentUnit); // ���ɖ߂�
        InputHandler.Instance.HideActionButtons();
        StartNextTurn();
    }

    public bool IsCurrentUnit(Unit unit)
    {
        return CurrentUnit == unit;
    }

    public void HighlightCurrentUnitMoveRange()
    {
        
        if (CurrentUnit != null)
        {
            //inputHandler.ShowMoveRange(CurrentUnit);
        }
    }

    public void OnPlayerMoveComplete()
    {
        // �n�C���C�g�X�V�ȂǕK�v�ł���΂�����
        //HighlightCurrentUnitMoveRange();

        // �G�̈ړ������J�n
        //StartCoroutine(enemyAI.ExecuteEnemyMove(CurrentUnit, gridManager));
    }



}
