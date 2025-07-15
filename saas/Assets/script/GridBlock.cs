using UnityEngine;
using System.Collections.Generic;

public class GridBlock : MonoBehaviour
{
    public Vector2Int gridPos; // ブロックのグリッド座標
    public bool isWalkable = true;
    private Unit previousUnit;
    public Unit occupantUnit = null;
    public bool isRamp;
    public float Height => transform.position.y;

    private Renderer blockRenderer;
    private Color highlightColor;//移動可能なマスの色
    private Color blockedColor;//移動不可なマスの色
    private Color originalColor;//元のマスの色

    public TileEffectData tileEffect;
    private TileEffectData previousEffect;

    public void UpdateOccupant(Unit newUnit)
    {
        if (occupantUnit != null && previousEffect != null)
        {
            previousEffect.RemoveEffect(occupantUnit);
        }

        occupantUnit = newUnit;

        if (occupantUnit != null && tileEffect != null)
        {
            tileEffect.ApplyEffect(occupantUnit);
            previousEffect = tileEffect;
        }
        else
        {
            previousEffect = null;
        }
    }

    private void Update()
    {
        if (occupantUnit != null && tileEffect != null && tileEffect.applyEachTurn)
        {
            tileEffect.ApplyEffect(occupantUnit);
        }
    }
    public enum BlockKinds//ブロックの種類
    {
        sand,
        glass
    }
    public BlockKinds blockKinds;//インスペクターから設定
    private void Start()
    {
        highlightColor = new Color(0f, 0f, 1f, 0f);//Unityのエディタで Opaque → Transparent にしないと半透明にならない
        blockedColor = new Color(1f, 0f, 0f, 0f);
    }
    public void Highlight(bool on)
    {
        if (blockRenderer != null)
        {
            
            if (on)
            {
                blockRenderer.material.color = isWalkable ? highlightColor : blockedColor;
            }
            else
            {
                blockRenderer.material.color = originalColor;
            }
        }
    }
    public void ClearHighlights()
    {
        blockRenderer.material.color = originalColor;
    }

    private void Awake()
    {
        blockRenderer = GetComponent<Renderer>();
        if (blockRenderer != null)
        {
            originalColor = blockRenderer.material.color; //元の色を保存
        }
    }

    public void SetColor(Color color)
    {
        if (blockRenderer != null)
            blockRenderer.material.color = color;
    }

}
