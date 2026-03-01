using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{

    public static Action<TileMap, RectTransform> OnBoardInitialized;


    // 포인터 이벤트들
    public static Action<Vector2Int> OnTilePointerDown;
    public static Action<Vector2Int> OnTilePointerDrag;
    public static Action OnTilePointerUp;





    // 길 업데이트
    public static Action<List<Vector2Int>, TileColor> OnPathUpdate;
    // 길 취소
    public static Action<List<Vector2Int>, TileColor> OnPathCanceled;
    public static Action<Vector2Int, TileColor> OnTileColorChanged;


    // 게임 끝 ( 리셋)
    public static Action OnGameReset;

    // 게임 클리어
    public static Action OnGameClear;

    public static Action OnMailBoxClicked;

}
