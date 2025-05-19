using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public GridManager gridManager;
    [SerializeField] private Unit unit;
    [SerializeField] private InputHandler inputHandler;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GridBlock clickedBlock = hit.collider.GetComponent<GridBlock>();
                if (clickedBlock == null) return;

                // 今操作してるユニットだけを取得
                Unit currentUnit = TurnManager.Instance.CurrentUnit;
                if (currentUnit == null) return;

                // そのユニットの移動範囲にあるか確認
                if (!currentUnit.movableBlocks.Contains(clickedBlock)) return;

                // 経路探索 → アニメーション付き移動
                var path = currentUnit.gridManager.FindPath(
                currentUnit.gridManager.GetGridPosition(currentUnit.transform.position),
                clickedBlock.gridPos,
                currentUnit
                );


                if (path != null && path.Count > 0)
                {
                    currentUnit.MoveToPath(path);
                }
            }
        }
    }



    List<GridBlock> FindPath(Vector3 startWorld, Vector3 endWorld)
    {
        // 必要ならここでA*などの経路探索を呼ぶ
        // 今はとりあえず1マスだけ直接返すダミー実装
        Vector2Int startPos = gridManager.GetGridPosition(startWorld);
        Vector2Int endPos = gridManager.GetGridPosition(endWorld);

        List<GridBlock> dummyPath = new List<GridBlock>();

        // ダミー: 隣接1マスだけ
        if (Vector2Int.Distance(startPos, endPos) <= 1f)
        {
            dummyPath.Add(gridManager.GetBlock(endPos));
        }

        return dummyPath;
    }
}
