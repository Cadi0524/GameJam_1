using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BoardInputView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public TileMap tileMap;
    private RectTransform boardRect;

    int width;
    int height;



    private Vector2Int lastDragPos = new Vector2Int(-1, -1);

    [SerializeField] private List<GameObject> dotObjects = new List<GameObject>();

    public void Awake()
    {
        boardRect = GetComponent<RectTransform>();

    }

    public void Initialize(int width, int height)
    {
        this.width = width;
        this.height = height;
       SetDotActive(true);
    }

    private Vector2Int GetGridPosition(Vector2 screenPosition)
    {
        Camera uiCamera = Camera.main;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            boardRect, screenPosition, uiCamera, out Vector2 localPoint);

        localPoint.x += boardRect.rect.width / 2f;
        localPoint.y += boardRect.rect.height / 2f;

        // 1칸의 크기
        float cellWidth = boardRect.rect.width / tileMap.width;
        float cellHeight = boardRect.rect.height / tileMap.height;

        int x = Mathf.FloorToInt(localPoint.x / cellWidth);
        int y = Mathf.FloorToInt(localPoint.y / cellHeight);

        // 이전 답변에서 맞췄던 Y축 시각적 반전 적용
        y = (tileMap.height - 1) - y;

        x = Mathf.Clamp(x, 0, tileMap.width - 1);
        y = Mathf.Clamp(y, 0, tileMap.height - 1);

        return new Vector2Int(x, y);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("클릭" + eventData.position);
        Vector2Int gridPos = GetGridPosition(eventData.position);
        GameEvents.OnTilePointerDown?.Invoke(gridPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2Int gridPos = GetGridPosition(eventData.position);
        if (gridPos != lastDragPos)
        {
            //  Debug.Log("드래그" + eventData.position);
            lastDragPos = gridPos;
            GameEvents.OnTilePointerDrag?.Invoke(gridPos);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("포인터 업");
        GameEvents.OnTilePointerUp?.Invoke();
    }


    public void SetDotActive(bool active)
    {
        foreach (var dot in dotObjects)
        {
            dot.SetActive(active);
        }
    }
}

