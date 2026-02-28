using UnityEngine;
using Srinex.OdinInspector;
using System.ComponentModel;
using System.Reflection;

public class BoardManager : SerializedMonoBehaviour
{


    [Title ("현재 보드 관련")]
    public Board currentBoard;
    public Board boardInstance;



    [Tooltip("현재 색")]
    public TileColor currentColor;
    [Tooltip("현재 드래그 중인 타일들의 위치")]
    [ShowInInspector]
    private List<Verctor2Int> currentDragPositions = new List<Vector2Int>();

    public void SetNewBoard(Board board)
    {
        currentBoard = board;
    }

    public void SetBoardInstance(Board board)
    {
        boardInstance = board.Copy();
    }

    public void ResetBoard()
    {
        if (board != null)
        {
            boardInstance = board.Copy();
        }
    }


    public void SetTileColor(Vector2Int position, TileColor color)
    {
        if (boardInstance != null)
        {
            boardInstance.SetTileColor(position, color);
        }
    }

    public void CancelDrag()
    {
        if (currentDragPositions.Count > 0)
        {
            foreach (var pos in currentDragPositions)
            {
                boardInstance.SetTileColor(pos, TileColor.None);
            }
            currentDragPositions.Clear();
        }
    }

    public void CompleteDrag()
    {
        if (currentDragPositions.Count > 0)
        {
            foreach (var pos in currentDragPositions)
            {
                boardInstance.SetTileColor(pos, currentColor);
            }
            currentDragPositions.Clear();
        }
    }

}
