using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public IEnumerator ExecuteEnemyTurn(Unit enemy, GridManager gridManager)
    {
        if (enemy == null)
        {
            Debug.LogError("EnemyUnit が null です");
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        Vector2Int enemyPos = gridManager.GetGridPosition(enemy.transform.position);

        Unit[] players = FindObjectsOfType<Unit>().Where(u => u.team == Unit.Team.Player).ToArray();

        // === ① 現在位置から攻撃可能かチェック ===
        var directAttackable = gridManager.GetAttackableBlockss(enemyPos, enemy.status.attackPattern);
        foreach (var block in directAttackable)
        {
            if (block.occupantUnit != null && block.occupantUnit.team == Unit.Team.Player)
            {
                InputHandler.Instance.ShowAttackRange(enemy);
                yield return new WaitForSeconds(0.5f);
                enemy.Attack(block.occupantUnit);
                TurnManager.Instance.EndUnitTurn();
                yield break;
            }
        }

        // === ② 移動後に攻撃できるマスを探す ===
        gridManager.GetMovableBlocks(enemyPos, enemy.status.moveRange, out var walkable, out _);

        foreach (var moveBlock in walkable)
        {
            var testPos = moveBlock.gridPos;
            var testAttackable = gridManager.GetAttackableBlockss(testPos, enemy.status.attackPattern);

            foreach (var block in testAttackable)
            {
                if (block.occupantUnit != null && block.occupantUnit.team == Unit.Team.Player)
                {
                    // 攻撃できる位置を見つけたので移動＆攻撃
                    var path = gridManager.FindPath(enemyPos, testPos, enemy);
                    if (path != null && path.Count > 0)
                    {
                        InputHandler.Instance.ShowMoveRange(enemy);
                        yield return enemy.MoveToPath(path);
                        yield return new WaitForSeconds(0.5f);
                        InputHandler.Instance.ClearHighlights();
                        InputHandler.Instance.ClearAllHighlights();
                        yield return new WaitForSeconds(0.5f);
                        InputHandler.Instance.ShowAttackRange(enemy);
                        yield return new WaitForSeconds(0.5f);
                        enemy.Attack(block.occupantUnit);
                        TurnManager.Instance.EndUnitTurn();
                        yield break;
                    }
                }
            }
        }

        // === ③ 攻撃できなかったので最も近いプレイヤーに近づく ===
        GridBlock bestTargetBlock = null;
        float minDistance = float.MaxValue;

        foreach (var player in players)
        {
            Vector2Int playerPos = gridManager.GetGridPosition(player.transform.position);

            foreach (var candidate in walkable)
            {
                float dist = Vector2Int.Distance(candidate.gridPos, playerPos);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestTargetBlock = candidate;
                }
            }
        }

        if (bestTargetBlock != null)
        {
            var path = gridManager.FindPath(enemyPos, bestTargetBlock.gridPos, enemy);
            if (path != null && path.Count > 0)
            {
                InputHandler.Instance.ShowMoveRange(enemy);
                yield return enemy.MoveToPath(path);
            }
        }

        yield return new WaitForSeconds(0.5f);
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
