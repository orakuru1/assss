using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
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
        if (turnQueue.Count == 0)
        {
            InitializeTurnOrder();
        }

        CurrentUnit = turnQueue.Dequeue();
        OnTurnStart?.Invoke(CurrentUnit);
        Debug.Log($"現在の行動ユニット: {CurrentUnit.name}（{CurrentUnit.team}）");

        // ここでUI更新やAI起動などしてもよい
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

}
