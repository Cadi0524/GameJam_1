using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
public class BoardDrawView : SerializedMonoBehaviour
{

    [Title("렌더러")]
    [SerializeField] private LineRenderer lineRenderer;


    [Title("팔레트")]
    public Dictionary<TileColor, Color> colorPalette = new();


    // 내부 관리용 변수들
    [HideInInspector]
    private Dictionary<TileColor, LineRenderer> lineRenderers = new Dictionary<TileColor, LineRenderer>();


    private RectTransform currentBoardRect;
    private int boardWidth;
    private int boardHeight;
    private float cellWidth;
    private float cellHeight;




    public void OnEnable()
    {
        GameEvents.OnBoardInitialized += InitBoard;
        GameEvents.OnPathUpdate += DrawLine;
        GameEvents.OnPathCanceled += DrawLine;

        GameEvents.OnGameClear += () => SetLineRendererOrder(false);
    }

    public void OnDisable()
    {
        GameEvents.OnBoardInitialized -= InitBoard;
        GameEvents.OnPathUpdate -= DrawLine;
        GameEvents.OnPathCanceled -= DrawLine;

        GameEvents.OnGameClear -= () => SetLineRendererOrder(false);
    }

    private void InitBoard(TileMap tileMap, RectTransform boardRect)
    {
        ClearAllLines();

        if (tileMap == null || boardRect == null) return;

        currentBoardRect = boardRect;

        boardWidth = tileMap.width;
        boardHeight = tileMap.height;

        cellWidth = currentBoardRect.rect.width / boardWidth;
        cellHeight = currentBoardRect.rect.height / boardHeight;

    }

    private void ClearAllLines()
    {
        foreach (var line in lineRenderers.Values)
        {
            if (line != null) Destroy(line.gameObject);
        }
        lineRenderers.Clear();
    }
    private void DrawLine(List<Vector2Int> path, TileColor color)
    {
        Debug.Log($"<color=magenta>[LineDrawer]</color> 🖌️ DrawLine 업데이트 요청됨! / 색상: {color}, 현재 경로 길이: {(path != null ? path.Count : 0)}");

        if (currentBoardRect == null)
        {
            Debug.LogError("<color=red>[LineDrawer]</color> ❌ currentBoardRect가 Null입니다! 보드 UI가 할당되지 않아 선을 그릴 수 없습니다.");
            return;
        }

        if (!lineRenderers.ContainsKey(color))
        {
            Debug.Log($"<color=yellow>[LineDrawer]</color> 🆕 [{color}] 색상의 LineRenderer가 존재하지 않아 새로 생성합니다.");
            LineRenderer newLine = Instantiate(lineRenderer, currentBoardRect);
            newLine.useWorldSpace = false;

            Color col = colorPalette.ContainsKey(color) ? colorPalette[color] : Color.white;
            col.a = 1f;
            newLine.positionCount = 0;

            Gradient solidGradient = new Gradient();
            solidGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(col, 0.0f), new GradientColorKey(col, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            newLine.colorGradient = solidGradient;

            lineRenderers[color] = newLine;
        }

        LineRenderer currentLine = lineRenderers[color];

        if (path == null || path.Count == 0)
        {
            Debug.Log($"<color=grey>[LineDrawer]</color> 🗑️ 경로가 비어있습니다. [{color}] 색상의 선을 화면에서 지웁니다 (positionCount = 0).");
            currentLine.positionCount = 0;
            return;
        }

        currentLine.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 localPos = GetCellCenterLocalPosition(path[i]);
            currentLine.SetPosition(i, localPos);
        }

        Debug.Log($"<color=green>[LineDrawer]</color> ✅ [{color}] 선 그리기 완료! 총 {path.Count}개의 점이 연결되었습니다. (마지막 점의 그리드 좌표: {path[path.Count - 1]})");
    }

    public void SetLineRendererOrder(bool isInFront)
    {
        int sortingOrder = isInFront ? 300 : 0;
        foreach (var line in lineRenderers.Values)
        {
            if (line != null)
            {
                line.sortingOrder = sortingOrder;
            }
        }
    }

    private Vector3 GetCellCenterLocalPosition(Vector2Int gridPos)
    {
        float startX = -currentBoardRect.rect.width * currentBoardRect.pivot.x;
        float startY = -currentBoardRect.rect.height * currentBoardRect.pivot.y;

        float centerX = startX + (gridPos.x * cellWidth) + (cellWidth / 2f);


        int visualY = (boardHeight - 1) - gridPos.y;
        float centerY = startY + (visualY * cellHeight) + (cellHeight / 2f);


        return new Vector3(centerX, centerY, -1f);
    }

}
