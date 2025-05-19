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

                // �����삵�Ă郆�j�b�g�������擾
                Unit currentUnit = TurnManager.Instance.CurrentUnit;
                if (currentUnit == null) return;

                // ���̃��j�b�g�̈ړ��͈͂ɂ��邩�m�F
                if (!currentUnit.movableBlocks.Contains(clickedBlock)) return;

                // �o�H�T�� �� �A�j���[�V�����t���ړ�
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
        // �K�v�Ȃ炱����A*�Ȃǂ̌o�H�T�����Ă�
        // ���͂Ƃ肠����1�}�X�������ڕԂ��_�~�[����
        Vector2Int startPos = gridManager.GetGridPosition(startWorld);
        Vector2Int endPos = gridManager.GetGridPosition(endWorld);

        List<GridBlock> dummyPath = new List<GridBlock>();

        // �_�~�[: �א�1�}�X����
        if (Vector2Int.Distance(startPos, endPos) <= 1f)
        {
            dummyPath.Add(gridManager.GetBlock(endPos));
        }

        return dummyPath;
    }
}
