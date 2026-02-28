using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Numerics;


public class Board : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler
{

    public TileMap tileMap;
    private RectTransform rectTransform;

    int width;
    int height;


    // 외부에서 발동시킬 액션들

    public event System.Action<Vector2Int> OnDragStart;
    public event System.Action<Vector2Int> OnDragMove;
    public event System.Action<Vector2Int> OnDragEnd;
    public void Awake()
    {
        width = tileMap.width;
        height = tileMap.height;
    }

    private Vector2Int GetGridPosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            boardRect, screenPosition, null, out Vector2 localPoint);


        // 계산 편하게 하기 위해서 다음과 같은 작업을 해줌
        localPoint.x += boardRect.rect.width / 2f;
        localPoint.y += boardRect.rect.height / 2f;

        // 1칸의 크기
        float cellWidth = boardRect.rect.width / mapData.width;
        float cellHeight = boardRect.rect.height / mapData.height;

        int x = Mathf.FloorToInt(localPoint.x / cellWidth);
        int y = Mathf.FloorToInt(localPoint.y / cellHeight);

        x = Mathf.Clamp(x, 0, mapData.width - 1);
        y = Mathf.Clamp(y, 0, mapData.height - 1);

        return new Vector2Int(x, y);
    }
    public void OnPointerDown(PointEventData eventData)
    {
        Vector2 gridPos = GetGridPosition(eventData.position);
        OnDragStart?.Invoke(gridPos);
    }


    public void OnDrag(PointEventData eventData)
    {
        Vector2 gridPos = GetGridPosition(eventData.position);
        OnDragMove?.Invoke(gridPos);
    }

    public void OnPointerUp(PointEventData eventData)
    {
        Vector2 gridPos = GetGridPosition(eventData.position);
        OnDragEnd?.Invoke(gridPos);
    }

    public Board Copy()
    {
        Board newBoard = new Board();
        newBoard.tileMap = this.tileMap; // 깊은 복사가 필요한 경우, tileMap도 복사해야 할 수 있습니다.
        newBoard.width = this.width;
        newBoard.height = this.height;
        return newBoard;
    }



}

