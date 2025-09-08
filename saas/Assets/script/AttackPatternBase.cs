using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AttackPatternBase : ScriptableObject
{
    public string patternName = "New Pattern";

    public abstract List<Vector2Int> GetPattern(Vector2Int center);
    public List<Vector2Int> relativePositions;
    public bool isAreaAttack;      // ”ÍˆÍUŒ‚ƒtƒ‰ƒO
}