using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public IEnumerator ExecuteEnemyMove(Unit enemy, GridManager gridManager)
    {
        if(enemy.team == Unit.Team.Enemy)
        {
Vector2Int startPos = gridManager.GetGridPosition(enemy.transform.position);

        // 1. 移動可能なブロックを取得
        gridManager.GetMovableBlocks(startPos, enemy.status.moveRange, out List<GridBlock> movableBlocks, out _);

        // 2. 最も遠いブロックを探す（マンハッタン距離）
        GridBlock furthestBlock = null;
        int maxDistance = -1;

        foreach (var block in movableBlocks)
        {
            int dist = Mathf.Abs(block.gridPos.x - startPos.x) + Mathf.Abs(block.gridPos.y - startPos.y);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                furthestBlock = block;
            }
        }

        // 3. 経路探索して移動（移動先が今いる場所と違う場合）
        if (furthestBlock != null && furthestBlock.gridPos != startPos)
        {
            List<GridBlock> path = gridManager.FindPath(startPos, furthestBlock.gridPos, enemy);
            if (path != null && path.Count > 0)
            {
                yield return enemy.MoveToPath(path);
            }
        }

        // 4. 移動後、ターン終了（または次の処理へ）
        
        }
        TurnManager.Instance.EndUnitTurn();
    }

    public void HighlightEnemyMoveRange(Unit enemy, GridManager gridManager)
    {
        Vector2Int gridPos = gridManager.GetGridPosition(enemy.transform.position);

        gridManager.GetMovableBlocks(
            gridPos,
            enemy.status.moveRange,
            out List<GridBlock> walkable,
            out _
        );

        // ここは InputHandler と同じようなハイライト処理を使う
        foreach (var block in walkable)
        {
            block.Highlight(false); // 赤とかで表示（仮）
        }
    }

}
