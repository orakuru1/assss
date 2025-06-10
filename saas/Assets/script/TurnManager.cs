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

    public void InitializeTurnOrder()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        var sortedUnits = allUnits.OrderByDescending(u => u.status.speed).ToList();

        turnQueue = new Queue<Unit>(sortedUnits);
    }

    public void StartNextTurn()
    {
        inputHandler.ClearAllHighlights();

        if (turnQueue.Count == 0)
        {
            InitializeTurnOrder();
        }
        
        CurrentUnit = turnQueue.Dequeue();
        OnTurnStart?.Invoke(CurrentUnit);;
        Debug.Log($"現在の行動ユニット: {CurrentUnit.name}（{CurrentUnit.team}）");
        // ここでUI更新やAI起動などしてもよい
        // ここで敵か味方かを判定
        if (CurrentUnit.team == Unit.Team.Enemy)
        {
            GridManager gridManager = FindObjectOfType<GridManager>();
            //enemyAI.HighlightEnemyMoveRange(CurrentUnit, gridManager);
            
            //StartCoroutine(enemyAI.ExecuteEnemyMove(CurrentUnit, FindObjectOfType<GridManager>()));
        }
        else
        {
            // 味方のときは入力受付など
            //inputHandler.ShowMoveRange(CurrentUnit);

            HighlightCurrentUnitMoveRange(); // 必要であれば
        }
    }

    public void EndUnitTurn()
    {
        turnQueue.Enqueue(CurrentUnit); // 後ろに戻す
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
            inputHandler.ShowMoveRange(CurrentUnit);
        }
    }

    public void OnPlayerMoveComplete()
    {
        // ハイライト更新など必要であればここで
        HighlightCurrentUnitMoveRange();

        // 敵の移動処理開始
        //StartCoroutine(enemyAI.ExecuteEnemyMove(CurrentUnit, gridManager));
    }

}
