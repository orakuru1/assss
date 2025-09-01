using UnityEngine;

[CreateAssetMenu(fileName = "NewTileEffect", menuName = "TileEffect/Create New Effect")]
public class TileEffectData : ScriptableObject
{
    public enum EffectType
    {
        None,
        FireDamage,
        Heal,
        BuffAttack,
        BuffDefense,
        BuffMoveRange,
        BuffAttackRange
    }

    public EffectType effectType;

    [Tooltip("数値：ダメージや回復の量など")]
    public int value;

    [Tooltip("ターン開始時に毎回効果を発動するか")]
    public bool applyEachTurn = true;

    public void ApplyEffect(Unit unit)
    {
        switch (effectType)
        {
            case EffectType.FireDamage:
                unit.TakeDamage(value);
                break;
            case EffectType.Heal:
                unit.Heal(value);
                break;
            case EffectType.BuffAttack:
                unit.status.attack += value;
                break;
            case EffectType.BuffDefense:
                unit.status.defense += value;
                break; 
            case EffectType.BuffMoveRange:
                unit.ApplyMoveRangeBonus(value);
                break;
            case EffectType.BuffAttackRange:
                unit.status.attackRange += value;
                break;case EffectType.None:
                unit.ResetToBase();
                break;
        }
    }

    public void RemoveEffect(Unit unit)
    {
        switch (effectType)
        {
            case EffectType.BuffAttack:
                unit.status.attack -= value;
                break;
            case EffectType.BuffDefense:
                unit.status.defense -= value;
                break;
            case EffectType.BuffMoveRange:
                unit.ResetToBase();
                break;
            case EffectType.BuffAttackRange:
                unit.status.attackRange += value;
                break;
        }
    }
}
