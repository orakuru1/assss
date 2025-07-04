using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    private Dictionary<Vector2Int, GridBlock> gridMap = new Dictionary<Vector2Int, GridBlock>();

    private void Awake()
    {
        // 全GridBlockを見つけて登録する
        GridBlock[] blocks = FindObjectsOfType<GridBlock>();
        foreach (var block in blocks)
        {
            block.gridPos = new Vector2Int(
               Mathf.RoundToInt(block.transform.position.x),
               Mathf.RoundToInt(block.transform.position.z)
           );
            Debug.Log($"登録したマス: {block.gridPos}");
            gridMap[block.gridPos] = block;
        }
    }

    // 指定グリッド座標のブロックを取得
    public GridBlock GetBlock(Vector2Int gridPos)
    {
        gridMap.TryGetValue(gridPos, out GridBlock block);
        return block;
    }

    // ワールド座標からグリッド座標へ変換
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x),
            Mathf.RoundToInt(worldPosition.z)
        );
    }

    public List<GridBlock> GetBlocksInRange(Vector2Int center, int range)
    {
        List<GridBlock> blocksInRange = new List<GridBlock>();

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2Int checkPos = new Vector2Int(center.x + x, center.y + y);
                int distance = Mathf.Abs(x) + Mathf.Abs(y); // マンハッタン距離

                if (distance <= range && gridMap.ContainsKey(checkPos))
                {
                    blocksInRange.Add(gridMap[checkPos]);
                }
            }
        }

        return blocksInRange;
    }


    public List<GridBlock> GetAttackableBlocks(Vector2Int center, int range)
    {
        List<GridBlock> result = new List<GridBlock>();

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector2Int pos = new Vector2Int(center.x + x, center.y + y);
                if (Mathf.Abs(x) + Mathf.Abs(y) <= range) // マンハッタン距離
                {
                    var block = GetBlock(pos);
                    if (block != null)
                    {
                        result.Add(block);
                    }
                }
            }
        }

        return result;
    }

    public List<GridBlock> GetAttackableBlockss(Vector2Int center, AttackPatternBase pattern)
    {
        List<GridBlock> result = new List<GridBlock>();

        foreach (var offset in pattern.relativePositions)
        {
            Vector2Int pos = center + offset;
            if (gridMap.TryGetValue(pos, out GridBlock block))
            {
                if (block != null)
                {
                    result.Add(block);
                }
            }
        }

        return result;
    }



    public void GetMovableBlocks(Vector2Int startPos, int moveRange, out List<GridBlock> walkable, out List<GridBlock> unwalkable)
    {
        walkable = new List<GridBlock>();
        unwalkable = new List<GridBlock>();

        Unit unit = TurnManager.Instance.CurrentUnit;
        if (unit == null) return;

        GridBlock startBlock = GetBlock(startPos);
        if (startBlock == null) return;

        Queue<(GridBlock block, int cost)> queue = new();
        HashSet<GridBlock> visited = new();

        queue.Enqueue((startBlock, 0));
        visited.Add(startBlock);

        while (queue.Count > 0)
        {
            var (current, cost) = queue.Dequeue();
            walkable.Add(current);

            if (cost >= moveRange) continue;

            Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

            foreach (var dir in directions)
            {
                Vector2Int nextPos = current.gridPos + dir;
                GridBlock nextBlock = GetBlock(nextPos);

                if (nextBlock == null || visited.Contains(nextBlock)) continue;
                if (nextBlock.occupantUnit != null && nextBlock.occupantUnit != unit)
                {
                    unwalkable.Add(nextBlock);
                    continue;
                }
                float heightDiff = Mathf.Abs(nextBlock.transform.position.y - current.transform.position.y);

                // ★ ここが重要！: 階段 or 通常段差チェック
                bool isReachable = heightDiff <= unit.status.maxStepHeight;

                if (isReachable && nextBlock.isWalkable)
                {
                    queue.Enqueue((nextBlock, cost + 1));
                    visited.Add(nextBlock);
                }
                else
                {
                    // ここで追加しておく
                    unwalkable.Add(nextBlock);
                    
                }
            }
        }

        // 移動範囲外の isWalkable なマスだけ unwalkable に入れる（見た目の参考用）
        foreach (var block in GetAllBlocks())
        {
            if (block.isWalkable && !walkable.Contains(block))
            {
                unwalkable.Add(block);
            }
        }
    }

    public List<GridBlock> GetAllBlocks()
    {
        return gridMap.Values.ToList(); // Dictionary<Vector2Int, GridBlock> blocks;
    }


    // GridManager.cs に追加するA*パス探索メソッド
    public List<GridBlock> FindPath(Vector2Int startPos, Vector2Int goalPos, Unit unit)
    {
        GridBlock startBlock = GetBlock(startPos);
        GridBlock goalBlock = GetBlock(goalPos);
        if (startBlock == null || goalBlock == null || !goalBlock.isWalkable) return null;

        var openSet = new PriorityQueue<GridBlock>(); // 優先度付きキュー（要実装 or 代用）
        var cameFrom = new Dictionary<GridBlock, GridBlock>();
        var gScore = new Dictionary<GridBlock, int>();

        foreach (var block in GetAllBlocks())
        {
            gScore[block] = int.MaxValue;
        }
        gScore[startBlock] = 0;

        openSet.Enqueue(startBlock, 0);

        while (openSet.Count > 0)
        {
            GridBlock current = openSet.Dequeue();
            if (current == goalBlock)
            {
                return ReconstructPath(cameFrom, current);
            }

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            foreach (var dir in directions)
            {
                GridBlock neighbor = GetBlock(current.gridPos + dir);
                if (neighbor == null || !CanMoveBetween(current, neighbor, unit)) continue;

                int tentativeG = gScore[current] + 1; // 各マスの移動コストは1
                if (tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    int fScore = tentativeG + GetManhattanDistance(neighbor.gridPos, goalPos);
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }
        return null; // 経路なし
    }

    private List<GridBlock> ReconstructPath(Dictionary<GridBlock, GridBlock> cameFrom, GridBlock current)
    {
        List<GridBlock> path = new List<GridBlock>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }

    private int GetManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // --- 簡易 PriorityQueue ---
    public class PriorityQueue<T>
    {
        private List<(T item, int priority)> elements = new List<(T, int)>();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIndex].priority)
                {
                    bestIndex = i;
                }
            }
            T bestItem = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }





    public bool CanMoveBetween(GridBlock from, GridBlock to, Unit unit)
    {
        if (to == null || !to.isWalkable) return false;

        float heightDelta = to.transform.position.y - from.transform.position.y;

        // 上り下りどっちも maxStepHeight 以内かチェック
        return Mathf.Abs(heightDelta) <= unit.status.maxStepHeight;
    }





    public bool IsWithinAttackRange(Vector2Int from, Vector2Int to, int range)
    {
        int distance = Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        return distance <= range;
    }

    

}
