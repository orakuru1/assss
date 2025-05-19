using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Unit unit;

    private List<GridBlock> currentHighlightedBlocks = new List<GridBlock>();
    public List<GridBlock> movableBlocks = new List<GridBlock>();
    private List<Vector2Int> currentMovablePositions = new List<Vector2Int>();
    private void Start()
    {
        unit = TurnManager.Instance.CurrentUnit;

        if (unit != null)
        {
            ShowMoveRange(unit);
        }
        else
        {
            Debug.LogWarning("Start時にユニットが見つかりませんでした");
        }
    }

    void Update()
    {


        if (Input.GetMouseButtonDown(0)) // 左クリックしたら
        {
            unit = TurnManager.Instance.CurrentUnit;

            if (unit == null)
            {
                Debug.LogWarning("現在のユニットが取得できません");
                return;
            }

            if (!TurnManager.Instance.IsCurrentUnit(unit)) return; // 自分のターンでなければ無視

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickPos = hit.point;
                Vector2Int gridPos = gridManager.GetGridPosition(clickPos);
                GridBlock clickedBlock = gridManager.GetBlock(gridPos);

                if (clickedBlock != null)
                {
                    Debug.Log("クリックしたマス: " + gridPos);
                    // ここに移動とかハイライト処理とかを書く
                    bool canMove = movableBlocks.Exists(b => b.gridPos == clickedBlock.gridPos);

                    if (clickedBlock != null && clickedBlock.occupantUnit != null)
                    {

                        Debug.Log("そのマスにいるユニット: " + clickedBlock.occupantUnit.name);
                        Debug.Log("そのユニットのチーム: " + clickedBlock.occupantUnit.team);
                        Unit target = clickedBlock.occupantUnit;

                        Vector2Int unitPos = gridManager.GetGridPosition(unit.transform.position);
                        Vector2Int targetPos = clickedBlock.gridPos;

                        if (gridManager.IsWithinAttackRange(unitPos, targetPos, unit.status.attackRange))
                        {
                            unit.Attack(target);
                            TurnManager.Instance.EndUnitTurn();
                            return;
                        }
                        else
                        {
                            Debug.Log("範囲外");
                            return;
                        }
                    }

                    if (canMove)
                    {
                        

                        unit.MoveTo(clickedBlock.transform.position);
                        TurnManager.Instance.EndUnitTurn();
                        ClearHighlights();
                    }
                    ShowMoveRange(unit);


                }
                /*
                else if (clickedBlock.isWalkable)
                {
                    unit.MoveTo(clickedBlock.transform.position);
                    //Debug.Log("何もないマス！");
                }
                */
            }
        }

    }

    //移動範囲の表示
    public void ShowMoveRange(Unit unit)
    {
        ClearHighlights();

        Vector2Int unitPos = gridManager.GetGridPosition(unit.transform.position);
        GridBlock currentBlock = gridManager.GetBlock(unitPos);

        // ① 今いるマスの isWalkable を一時的に false にする
        bool originalWalkable = currentBlock.isWalkable;
        currentBlock.isWalkable = false;

        // 実際に移動可能なマス（段差など考慮）
        List<GridBlock> walkable, unwalkable;
        gridManager.GetMovableBlocks(unitPos, unit.status.moveRange, out walkable, out unwalkable);
        unit.movableBlocks = walkable;

        // 青ハイライト：移動可能なマス
        foreach (var block in walkable)
        {
            block.Highlight(true);
            currentHighlightedBlocks.Add(block);
            movableBlocks.Add(block);
        }

        // 範囲内にあるけど移動できない（青くなってない）ブロックに対して
        var rangeBlocks = gridManager.GetBlocksInRange(unitPos, unit.status.moveRange);
        foreach (var block in rangeBlocks)
        {
            if (!walkable.Contains(block))
            {
                block.SetColor(new Color(1, 0, 0, 0.5f)); // 赤くする
                currentHighlightedBlocks.Add(block);
            }
        }
        ShowAttackRange(); // 攻撃範囲表示
        currentBlock.isWalkable = originalWalkable;
    }



    //攻撃範囲の表示
    private void ShowAttackRange()
    {
        Vector2Int unitPos = gridManager.GetGridPosition(unit.transform.position);
        List<GridBlock> allAttackableBlocks = gridManager.GetBlocksInRange(unitPos, unit.status.attackRange);

        foreach (var block in allAttackableBlocks)
        {
            if (block.occupantUnit != null && block.occupantUnit.team != unit.team)
            {
                block.SetColor(new Color(1f, 0f, 0f, 0.5f));
            }
            else
            {
                block.SetColor(new Color(1f, 0.5f, 0.5f, 0.2f)); // 敵はいないけど攻撃範囲
            }
            currentHighlightedBlocks.Add(block);
        }

    }

    //移動・攻撃範囲の表示を消す
    private void ClearHighlights()
    {
        foreach (var block in currentHighlightedBlocks)
        {
            block.Highlight(false);
        }
        currentHighlightedBlocks.Clear();
        movableBlocks.Clear();
    }

    private void ShowUnwalkableBlocks(List<GridBlock> unwalkable)
    {
        foreach (var block in unwalkable)
        {
            block.SetColor(new Color(1f, 0f, 0f, 0.5f)); // 赤色
            currentHighlightedBlocks.Add(block);
        }
    }

    private void OnEnable()
    {
        TurnManager.OnTurnStart += ShowMoveRange;
    }

    private void OnDisable()
    {
        TurnManager.OnTurnStart -= ShowMoveRange;
    }

}
