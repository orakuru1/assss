using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackPattern/Cross")]
public class CrossAttackPattern : AttackPatternBase
{
    public int range = 1;

    public override List<Vector2Int> GetPattern(Vector2Int center)
    {
        List<Vector2Int> result = new();

        for (int i = 1; i <= range; i++)
        {
            result.Add(center + Vector2Int.up * i);
            result.Add(center + Vector2Int.down * i);
            result.Add(center + Vector2Int.left * i);
            result.Add(center + Vector2Int.right * i);
        }

        return result;
    }
}