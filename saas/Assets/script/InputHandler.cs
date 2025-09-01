using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] public Unit unit;
    
    [SerializeField] private GameObject cursorFramePrefab;
    private GameObject currentFrame;

    private List<GridBlock> currentHighlightedBlocks = new List<GridBlock>();
    public List<GridBlock> movableBlocks = new List<GridBlock>();
    List<GridBlock> attackableBlocks = new List<GridBlock>();
    private List<Vector2Int> currentMovablePositions = new List<Vector2Int>();
    private List<GridBlock> currentAttackableBlocks = new();

    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject cancelMoveButton;
    public GameObject BackButton;
    public GameObject TurnEndButton;
    public static InputHandler Instance { get; internal set; }

    private void Start()
    {
        unit = TurnManager.Instance.CurrentUnit;

        if (unit != null)
        {
            //ShowMoveRange(unit);
        }
        else
        {
            Debug.LogWarning("Start時にユニットが見つかりませんでした");
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void ShowActionButtons()
    {
        moveButton.SetActive(true);
        attackButton.SetActive(true);
        cancelMoveButton.SetActive(false);
        BackButton.SetActive(false);
        TurnEndButton.SetActive(true);
    }
    public void HideActionButtons()
    {
        moveButton.SetActive(false);
        attackButton.SetActive(false);
        cancelMoveButton.SetActive(false);
        BackButton.SetActive(false);
        TurnEndButton.SetActive(false);
    }
    public void OnMoveButtonPressed()
    {
        if (TurnManager.Instance.CurrentUnit != null)
        {
            ClearHighlights();
            ClearAllHighlights();
            if (TurnManager.Instance.CurrentUnit != null)
            {

                ShowMoveRange(unit);
            }
        }
        moveButton.SetActive(false);
        BackButton.SetActive(true);
    }

    public void OnAttackButtonPressed()
    {
        if (TurnManager.Instance.CurrentUnit != null)
        {
            ClearHighlights();
            ClearAllHighlights();
            ShowAttackRange(unit);
            return;
        }
        attackButton.SetActive(false);
        BackButton.SetActive(true);
    }

    public void OnCancelButtonPressed()
    {
        ClearHighlights();
        ClearAllHighlights();
        if (TurnManager.Instance.CurrentUnit != null)
        {
            TurnManager.Instance.CurrentUnit.CancelMove();
        }
        moveButton.SetActive(true);
        cancelMoveButton.SetActive(false);
    }

    public void OnBackButtonPressed()
    {
        ClearHighlights();
        ClearAllHighlights();
        BackButton.SetActive(false);
    }
    
    public void OnTurnEndButtonPressed()
    {
        ClearHighlights();
        ClearAllHighlights();
        if (TurnManager.Instance.CurrentUnit != null)
        {
            TurnManager.Instance.EndUnitTurn();
        }
    }

    void Update()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit2))
        {
            GridBlock hoveredBlock = hit2.collider.GetComponent<GridBlock>();
            if (hoveredBlock != null)
            {Vector2Int gridPos = hoveredBlock.gridPos;
                if (currentFrame == null)
                {
                    currentFrame = Instantiate(cursorFramePrefab);
                }

                Vector3 blockWorldPos = gridManager.GetBlock(gridPos).transform.position;
                currentFrame.transform.position = blockWorldPos + Vector3.up * 0.05f;
            }
        }

            if (Input.GetMouseButtonDown(0)) // 左クリックしたら
        {
            unit = TurnManager.Instance.CurrentUnit;

            if (unit == null)
            {
                Debug.LogWarning("現在のユニットが取得できません");
                return;
            }

            if (!TurnManager.Instance.IsCurrentUnit(unit)) return; // 自分のターンでなければ無視

            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                
                Vector3 clickPos = hit.point;
                Vector3 adjustedHitPoint = hit.point + new Vector3(0.15f, 0, -0.15f);

                Vector2Int gridPos = gridManager.GetGridPosition(adjustedHitPoint);
                GridBlock clickedBlock = gridManager.GetBlock(gridPos);

                if (clickedBlock != null)
                {
                    Unit currentUnit = TurnManager.Instance.CurrentUnit;

                    // 移動可能な範囲かチェック
                    if (movableBlocks.Contains(clickedBlock))
                    {
                        Vector2Int startPos = gridManager.GetGridPosition(currentUnit.transform.position);
                        Vector2Int goalPos = clickedBlock.gridPos;
                        var path = gridManager.FindPath(startPos,goalPos, currentUnit);
                        StartCoroutine(currentUnit.MoveToPath(path)); // ★ここで呼ぶ
                    }

                    Debug.Log("クリックしたマス: " + gridPos);
                    // ここに移動とかハイライト処理とかを書く
                    bool canMove = movableBlocks.Exists(b => b.gridPos == clickedBlock.gridPos);
                    

                    if (clickedBlock != null && clickedBlock.occupantUnit != null)
                    {

                        Debug.Log("そのマスにいるユニット: " + clickedBlock.occupantUnit.name);
                        Debug.Log("そのユニットのチーム: " + clickedBlock.occupantUnit.team);
                        Unit attacker = TurnManager.Instance.CurrentUnit;
                        Unit target = clickedBlock.occupantUnit;
                        Vector2Int unitPos = gridManager.GetGridPosition(unit.transform.position);
                        List<Vector2Int> attackRange = unit.status.attackPattern.GetPattern(unitPos);
                        Vector2Int targetPos = clickedBlock.gridPos;

                        if (attackableBlocks.Any(b => b.gridPos == clickedBlock.gridPos))
                        {
                            if (attacker.team != target.team)
                            {
                                
                                attacker.Attack(target);
                                ClearHighlights();
                                ClearAllHighlights();
                                currentAttackableBlocks.Clear();
                                //TurnManager.Instance.EndUnitTurn();
                                return;
                            }
                            
                        }
                        else
                        {
                            Debug.Log("範囲外");
                            return;
                        }
                    }

                    if (canMove)
                    {
                        
                        ClearHighlights();
                        unit.MoveTo(clickedBlock.transform.position);
                        ClearAllHighlights();
                        //TurnManager.Instance.EndUnitTurn();
                        cancelMoveButton.SetActive(true);
                    }
                    
                    

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

        if (Input.GetKeyDown(KeyCode.A))//攻撃
        {
            ClearHighlights();
            ClearAllHighlights();
            ShowAttackRange(unit);
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))//移動
        {
            ClearHighlights();
            ClearAllHighlights();
            if (TurnManager.Instance.CurrentUnit != null)
            {
                
                ShowMoveRange(unit);
            }
        }
        if (Input.GetKeyDown(KeyCode.C))//移動キャンセル
        {
            ClearHighlights();
            ClearAllHighlights();
            if (TurnManager.Instance.CurrentUnit != null)
            {
                TurnManager.Instance.CurrentUnit.CancelMove();
                ShowMoveRange(unit);
            }
        }
        if (Input.GetKeyDown(KeyCode.B))//1つ前の操作に戻る
        {
            ClearHighlights();
            ClearAllHighlights();
            
        }
        if (Input.GetKeyDown(KeyCode.E))//ターン終了
        {
            ClearHighlights();
            ClearAllHighlights();
            if (TurnManager.Instance.CurrentUnit != null)
            {

                TurnManager.Instance.EndUnitTurn();
            }
        }



    }

    //移動範囲の表示
    public void ShowMoveRange(Unit unit)
    {
        ClearHighlights();
        ClearAllHighlights();
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
            //ShowAttackRange(unit);
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
         // 攻撃範囲表示
        currentBlock.isWalkable = originalWalkable;
    }



    //攻撃範囲の表示
    public void ShowAttackRange(Unit unit)
    {
        attackableBlocks.Clear();
        Vector2Int unitPos = gridManager.GetGridPosition(unit.transform.position);
        List<GridBlock> allAttackableBlocks = gridManager.GetBlocksInRange(unitPos, unit.status.attackRange);
        List<Vector2Int> attackPositions = unit.status.attackPattern.GetPattern(unitPos);
        var blocks = gridManager.GetAttackableBlockss(unitPos, unit.status.attackPattern);
        attackableBlocks = gridManager.GetAttackableBlockss(unitPos, unit.status.attackPattern);
        foreach (var block in blocks)
        {
            block.SetColor(new Color(1, 0, 0, 0.5f)); // 赤色
            attackableBlocks.Add(block);
            currentAttackableBlocks.Add(block);
        }

        foreach (var pos in attackPositions)
        {
            GridBlock block = gridManager.GetBlock(pos);
            if (block != null)
            {
                block.SetColor(new Color(1, 0, 0, 0.5f)); // 例：赤色
                currentHighlightedBlocks.Add(block);     // 消すとき用に登録
            }
            else
            {
                block.SetColor(new Color(1f, 0.5f, 0.5f, 0.2f)); // 敵はいないけど攻撃範囲
            }
            currentHighlightedBlocks.Add(block);
        }

    }

    //移動・攻撃範囲の表示を消す
    public void ClearHighlights()
    {
        foreach (var block in currentHighlightedBlocks)
        {
            block.Highlight(true);
        }
        currentAttackableBlocks.Clear();
        attackableBlocks.Clear();
        currentHighlightedBlocks.Clear();
        movableBlocks.Clear();
    }

    public void ClearAllHighlights()
    {
        foreach (var block in gridManager.GetAllBlocks())
        {
            block.ClearHighlights();
        }
        currentAttackableBlocks.Clear();
        attackableBlocks.Clear();
    }

    private void ShowUnwalkableBlocks(List<GridBlock> unwalkable)
    {
        foreach (var block in unwalkable)
        {
            block.SetColor(new Color(1f, 0f, 0f, 0.5f)); // 赤色
            currentHighlightedBlocks.Add(block);
        }
    }

    

    private void OnDisable()
    {
        TurnManager.OnTurnStart -= ShowMoveRange;
    }

}
