using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using TMPro;
public class BoardManager : SerializedMonoBehaviour
{

    public TileMap currentBoard;
    private bool isDragging = false;
    private TileColor currentColor = TileColor.None;

    [ShowInInspector]
    private Dictionary<TileColor, List<Vector2Int>> paths = new Dictionary<TileColor, List<Vector2Int>>();

    private Dictionary<Vector2Int, List<TileColor>> mixerInputs = new Dictionary<Vector2Int, List<TileColor>>();


    private bool isCleared = false;

    public void Init(TileMap tileMap)
    {
        currentBoard = tileMap;
        paths.Clear();
        mixerInputs.Clear();
        isCleared = false;
    }

    private void OnEnable()
    {
        GameEvents.OnTilePointerDown += HandleTilePointerDown;
        GameEvents.OnTilePointerDrag += HandleTileDrag;
        GameEvents.OnTilePointerUp += HandleTilePointerUp;
        GameEvents.OnBoardInitialized += (tileMap, boardRect) =>
        {
            Init(tileMap);
        };
        isCleared = false;
    }

    private void OnDisable()
    {
        GameEvents.OnTilePointerDown -= HandleTilePointerDown;
        GameEvents.OnTilePointerDrag -= HandleTileDrag;
        GameEvents.OnTilePointerUp -= HandleTilePointerUp;
        GameEvents.OnBoardInitialized -= (tileMap, boardRect) =>
        {
            Init(tileMap);
        };
    }

    private void HandleTilePointerUp()
    {


        isDragging = false;
        currentColor = TileColor.None;

        //게임 오버 검사
        // CheckGameClear();

        SoundManager.Instance.PlaySFX(SoundType.PenUp);
    }
    private void HandleTileDrag(Vector2Int pos)
    {
        if (!isDragging || currentBoard == null) return;

        // 현재 색 길 정보
        List<Vector2Int> path = paths[currentColor];
        if (path.Count == 0) return;

        Vector2Int lastPos = path[path.Count - 1];

        // 1. 상하좌우 1칸 인접 검사 (드래그 이벤트는 시도때도 없이 들어오므로, 칸이 안 바뀌었으면 조용히 리턴)
        if (Mathf.Abs(pos.x - lastPos.x) + Mathf.Abs(pos.y - lastPos.y) != 1) return;

        TileData targetTile = currentBoard.GetTile(pos.x, pos.y);

        // 맵 범위를 벗어났거나, 벽(Block)이면 전진 불가
        if (targetTile == null || targetTile.type == TileType.Block)
        {
            Debug.Log($"<color=orange>[BoardManager]</color> 🚫 이동 불가! 범위를 벗어났거나 Block 타일입니다. (위치: {pos})");
            return;
        }

        // 2. 내 선을 되돌아가는 경우 (꼬리 자르기)
        if (path.Contains(pos))
        {
            int crossIndex = path.IndexOf(pos);
            Debug.Log($"<color=yellow>[BoardManager]</color> ↩️ 내 선으로 되돌아감! 인덱스 {crossIndex + 1}부터 꼬리 자르기 실행.");
            CutPath(currentColor, crossIndex + 1);

            // 잘려나간 상태로 화면 갱신
            GameEvents.OnPathUpdate?.Invoke(path, currentColor);
            return;
        }

        // 기본적 금지 ( Block, Start)
        bool canMove = false;

        if (targetTile.type == TileType.None)
        {
            canMove = true;

            // 다른색상 만나면
            if (targetTile.color != TileColor.None && targetTile.color != currentColor)
            {
                Debug.Log($"<color=red>[BoardManager]</color> 💥 다른 색상({targetTile.color})의 선과 교차! 상대방 선 자르기 진입.");
                // 만난 색의 선을 자름
                foreach (var otherPath in paths)
                {
                    if (otherPath.Key != currentColor && otherPath.Value.Contains(pos))
                    {
                        int otherCrossIndex = otherPath.Value.IndexOf(pos);
                        Debug.Log($"<color=red>[BoardManager]</color> ✂️ 상대방({otherPath.Key}) 선을 인덱스 {otherCrossIndex}에서 자름!");
                        // 교차점 포함해서 자름
                        CutPath(otherPath.Key, otherCrossIndex);
                        break;
                    }
                }
            }
        }
        else if (targetTile.type == TileType.Mixer)
        {
            Debug.Log($"<color=cyan>[BoardManager]</color> 🌀 믹서(Mixer) 노드 진입!");
            canMove = true; // 믹서 노드는 무조건 진입 허용
        }
        else if (targetTile.type == TileType.End)
        {
            Debug.Log($"<color=cyan>[BoardManager]</color> 🏁 도착점(End) 타일 진입 확인!");
            canMove = true; // 도착점이면 허용
        }

        // 최종 처리
        if (canMove)
        {
            if (targetTile.type == TileType.None)
            {
                targetTile.color = currentColor; // 빈칸 덮어쓰기
            }
            else if (targetTile.type == TileType.Mixer)
            {
                // 일단 mixerInput에 있는지 확인
                if (!mixerInputs.ContainsKey(pos))
                {
                    mixerInputs[pos] = new List<TileColor>();
                }

                // 인풋에 어떤 색이 들어와 있는지 보는 리스트
                List<TileColor> inputColors = mixerInputs[pos];

                if (!inputColors.Contains(currentColor)) inputColors.Add(currentColor);

                // 두 개 이상 들어오면 먼저 들어온거 자르기 
                if (inputColors.Count > 2)
                {
                    TileColor oldColor = inputColors[0];
                    inputColors.RemoveAt(0);

                    int oldestIndex = paths[oldColor].IndexOf(pos);
                    if (oldestIndex != -1) CutPath(oldColor, oldestIndex);
                }

                // 색 섞기
                TileColor mixedColor = TileColor.None;
                foreach (TileColor c in inputColors)
                {
                    mixedColor |= c; // 비트 연산으로 색상 섞기
                }
                targetTile.color = mixedColor;

                isDragging = false; // 믹서에 닿으면 드래그 종료
                Debug.Log($"<color=cyan>[Mixer]</color> 현재 믹서 색상: {targetTile.color}");
            }

            path.Add(pos);
            GameEvents.OnPathUpdate?.Invoke(path, currentColor);

            if (targetTile.type == TileType.End)
            {
                Debug.Log($"<color=green>[BoardManager]</color> 🎉 [도착점 연결 성공] 색상: {currentColor}. 드래그 자동 종료.");
                isDragging = false;

                CheckGameClear();
                //targetTile.color = currentColor; // 도착점도 내 색으로 칠하기
            }
        }
    }

    private void HandleTilePointerDown(Vector2Int pos)
    {
        Debug.Log($"<color=yellow>[BoardManager]</color> 👆 PointerDown 이벤트 수신 완료! / 클릭 위치: {pos}");

        SoundManager.Instance.PlaySFX(SoundType.PenDown);
        if (currentBoard == null)
        {
            Debug.LogError("<color=red>[BoardManager]</color> ❌ currentBoard가 Null이야! GameEvents.OnBoardInitialized가 안 불렸거나 Init이 안 됐어.");
            return;
        }

        TileData clickedTile = currentBoard.GetTile(pos.x, pos.y);

        if (clickedTile == null)
        {
            Debug.Log($"<color=orange>[BoardManager]</color> ⚠️ {pos} 위치의 타일 데이터가 Null이야. 범위를 벗어났을 수 있어.");
            return;
        }

        Debug.Log($"<color=cyan>[BoardManager]</color> 🔎 클릭한 타일 정보 -> 타입: {clickedTile.type}, 색상: {clickedTile.color}");


        bool canStartDragging = clickedTile.color != TileColor.None &&
                                clickedTile.type != TileType.Block &&
                                clickedTile.type != TileType.End;

        if (canStartDragging)
        {
            if (clickedTile.type == TileType.Mixer)
            {
                if (!mixerInputs.ContainsKey(pos) || mixerInputs[pos].Count < 2)
                {
                    Debug.Log("<color=grey>[Mixer]</color> 아직 두 가지 색상이 다 모이지 않아서 출발할 수 없습니다.");
                    return;
                }
            }

            isDragging = true;
            currentColor = clickedTile.color;
            Debug.Log($"<color=green>[BoardManager]</color> ✅ 드래그 시작! 색상: {currentColor}");

            // 2. 경로 처리
            if (paths.ContainsKey(currentColor))
            {
                int index = paths[currentColor].IndexOf(pos);

                if (index != -1)
                {
                    Debug.Log($"<color=yellow>[BoardManager]</color> ✂️ 기존 경로 위를 클릭함. 인덱스 {index + 1}부터 꼬리 자르기 실행!");
                    CutPath(currentColor, index + 1);
                }
                else if (clickedTile.type == TileType.Start || clickedTile.type == TileType.Mixer)
                {
                    Debug.Log($"<color=yellow>[BoardManager]</color> 🔄 시작점/믹서를 다시 누름! 기존 경로 초기화 후 새로 시작!");
                    paths[currentColor].Clear();
                    paths[currentColor].Add(pos);
                }
                else
                {
                    Debug.Log($"<color=grey>[BoardManager]</color> ℹ️ 이미 있는 색상이지만, 경로나 시작점이 아닌 곳을 눌러서 드래그만 활성화함.");
                }
            }
            else
            {
                Debug.Log($"<color=green>[BoardManager]</color> 🆕 처음 누르는 색상! 새로운 경로 리스트 생성 완료.");
                paths[currentColor] = new List<Vector2Int> { pos };
            }
        }
        else
        {
            string reason = clickedTile.type == TileType.End ? "도착점(End)" : "색상이 없는 타일";
            Debug.Log($"<color=orange>[BoardManager]</color> ⚪ {reason}이므로 드래그를 시작하지 않습니다.");
        }
    }
    public void CutPath(TileColor color, int startIndex)
    {
        if (paths.ContainsKey(color))
        {
            List<Vector2Int> path = paths[color];

            // 자를 게 없으면 안전하게 리턴
            if (startIndex >= path.Count) return;

            int removeCount = path.Count - startIndex;
            List<Vector2Int> cutPath = path.GetRange(startIndex, removeCount);

            foreach (Vector2Int p in cutPath)
            {
                TileData tile = currentBoard.GetTile(p.x, p.y);

                if (tile != null)
                {
                    // 빈칸이었으면 색상 지우기
                    if (tile.type == TileType.None)
                    {
                        tile.color = TileColor.None;
                    }
                    // 잘려나간 곳에 믹서가 있다면 ? 
                    else if (tile.type == TileType.Mixer)
                    {
                        // 믹서에서 해당 색 제거
                        if (mixerInputs.ContainsKey(p) && mixerInputs[p].Contains(color))
                        {
                            mixerInputs[p].Remove(color);

                            // 타일 색 다시 계산
                            TileColor mixedColor = TileColor.None;
                            foreach (TileColor c in mixerInputs[p])
                            {
                                mixedColor |= c; // 남은 색상들로 다시 비트 덧셈
                            }
                            tile.color = mixedColor;

                            Debug.Log($"<color=cyan>[Mixer]</color> 선이 끊어져서 {color} 색상이 믹서에서 제거됨. 남은 색상: {tile.color}");
                        }
                    }
                }
            }

            path.RemoveRange(startIndex, removeCount);
            GameEvents.OnPathCanceled?.Invoke(path, color);
        }
    }

    public void CheckGameClear()
    {
        // 이미 클리어된 상태이거나 보드가 없으면 무시
        if (currentBoard == null || isCleared) return;

        bool isBoardFull = true;
        HashSet<TileColor> requireColors = new HashSet<TileColor>();

        // 1. 보드 전체 스캔 (빈 칸 여부 확인 및 요구 색상 수집)
        for (int x = 0; x < currentBoard.width; x++)
        {
            for (int y = 0; y < currentBoard.height; y++)
            {
                TileData tile = currentBoard.GetTile(x, y);

                // 빈 칸 체크
                if (tile.type == TileType.None && tile.color == TileColor.None)
                {
                    isBoardFull = false;
                }

                // End 타일의 '색상'만 수집 (개발자님의 원래 로직 복구!)
                if (tile.type == TileType.End)
                {
                    requireColors.Add(tile.color);
                }
            }
        }

        int wrongCount = 0;

        // 2. 각 "요구 색상별"로 선이 End 타일에 도착했는지 검사
        foreach (TileColor targetColor in requireColors)
        {
            // 2-1. 아직 해당 색상의 선을 긋지도 않았거나, 끊겨있다면 (진행 중)
            if (!paths.ContainsKey(targetColor) || paths[targetColor].Count == 0)
            {
                return; // 검사 중단, 더 플레이해야 함
            }

            List<Vector2Int> path = paths[targetColor];
            Vector2Int endPos = path[path.Count - 1];
            TileData endTile = currentBoard.GetTile(endPos.x, endPos.y);

            // 2-2. 선의 끝이 End 타일이 아닌 허공(None 등)에 멈춰있다면 (진행 중)
            if (endTile == null || endTile.type != TileType.End)
            {
                return; // 검사 중단, 더 플레이해야 함
            }

            // 2-3. End 타일에 닿긴 닿았는데, 타일이 요구하는 색상과 선의 색상이 다를 경우
            if (endTile.color != targetColor)
            {
                wrongCount++; // 오답 카운트 증가! (return하지 않고 끝까지 검사함)
            }
        }

        // 3. 여기까지 코드가 내려왔다는 것은 "모든 요구 색상의 선이 어떤 End 타일에든 연결은 되었다"는 뜻입니다.

        if (isBoardFull && wrongCount == 0)
        {
            // [조건 3] 빈 칸 없음 + 오답 없음 = 완벽한 클리어!
            Debug.Log("<color=green>-----게임 클리어 완료! 모든 색상이 End 지점에 알맞게 연결되었습니다!------</color>");
            isCleared = true;
            GameEvents.OnGameClear?.Invoke();
        }
        else
        {
            // [조건 1] 빈칸이 있음 (isBoardFull == false)
            // [조건 2] 색이 틀림 (wrongCount > 0)
            Debug.Log($"<color=orange>[Helper 호출]</color> 보드 꽉참 여부: {isBoardFull} / 틀린 색 개수: {wrongCount}");
            SoundManager.Instance.PlaySFX(SoundType.Fail);
            GameEvents.OnClearHelper?.Invoke(isBoardFull, wrongCount);
        }
    }
}
