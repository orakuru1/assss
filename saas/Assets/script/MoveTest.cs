using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public Unit unit;
    public GridManager gridManager;

    void Start()
    {
        List<GridBlock> path = new List<GridBlock>();
        path.Add(gridManager.GetBlock(new Vector2Int(4, -8)));
        path.Add(gridManager.GetBlock(new Vector2Int(5, -8)));
        path.Add(gridManager.GetBlock(new Vector2Int(6, -8)));

        unit.MoveToPath(path);
    }
}
