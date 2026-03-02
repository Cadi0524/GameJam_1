using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "Level_", menuName = "Board/Level Data")]
public class TileMap : SerializedScriptableObject
{
    public int width;
    public int height;


    [ShowInInspector]
    [TableMatrix(SquareCells = true, DrawElementMethod = "DrawTileCell")]
    public TileData[,] tiles;



#if UNITY_EDITOR
    private static TileData DrawTileCell(Rect rect, TileData value)
    {
        // 1. 데이터가 비어있다면 기본값으로 초기화 (에러 방지용)
        if (value == null) return new TileData(0, 0);


        Color bgColor = GetColorFromTileColor(value.color);
        EditorGUI.DrawRect(rect.Padding(1), bgColor);

        Rect typeRect = new Rect(rect.x, rect.y, rect.width, rect.height / 2f);
        Rect colorRect = new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height / 2f);

        // 4. 위쪽 절반: TileType 선택 드롭다운
        value.type = (TileType)EditorGUI.EnumPopup(typeRect, value.type);

        // 5. 아래쪽 절반: TileColor 선택 드롭다운
        value.color = (TileColor)EditorGUI.EnumPopup(colorRect, value.color);

        return value;
    }


    // 색상을 시각적으로 보여주기 위한 헬퍼 함수
    private static Color GetColorFromTileColor(TileColor color)
    {
        switch (color)
        {
            case TileColor.None: return new Color(0.2f, 0.2f, 0.2f); // 빈칸은 어두운 회색
            case TileColor.Stage1_Flower: return Color.red;
            case TileColor.Stage1_Moon: return Color.green;
            case TileColor.Stage2_Flower: return Color.red;
            case TileColor.Stage2_Vase: return Color.cyan;
            case TileColor.Stage2_Cat: return Color.yellow;
            case TileColor.Stage2_SOFA: return Color.brown;
            case TileColor.Stage3_Sun_Red: return Color.red;
            case TileColor.Stage3_Sun_Yellow: return Color.yellow;
            case TileColor.Stage3_Sun: return new Color(1f, 0.5f, 0f); // 빨강+노랑 = 주황
            case TileColor.Stage3_Ship: return Color.brown;
            case TileColor.Stage3_Sail: return Color.white;
            case TileColor.Stage3_Human: return Color.purple;
            case TileColor.Stage4_AirBalloonPurple_Red: return new Color(0.5f, 0f, 0.5f); // 보라색 풍선 빨강
            case TileColor.Stage4_AirBalloonPurple_Blue: return new Color(0.5f, 0f, 0.5f); // 보라색 풍선 파랑
            case TileColor.Stage4_AirBalloonPurple: return new Color(0.5f, 0f, 0.5f); // 보라색 풍선 (빨강+파랑)
            case TileColor.Stage4_AirBallonYellow: return Color.yellow;
            case TileColor.Stage4_AirBalloonGreen: return Color.green;
            case TileColor.Stage4_AirBallonwPink: return Color.hotPink;
            case TileColor.Stage4_AirBallonMagenta: return Color.magenta;
            case TileColor.Stage4_AirBallonEmerald: return new Color(0f, 0.8f, 0.8f); // 에메랄드 색상


            default: return new Color(0.2f, 0.2f, 0.2f);
        }
    }
#endif
    [Button("맵 격자 생성 및 초기화", ButtonSizes.Medium)]
    private void InitializeMap()
    {
        tiles = new TileData[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new TileData(x, y);
            }
        }
    }
    public TileData GetTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return tiles[x, y];
    }
    // 맵 전체의 런타임 복사본을 생성하는 함수
    public TileMap Clone()
    {
        TileMap cloneMap = ScriptableObject.CreateInstance<TileMap>();

        cloneMap.width = this.width;
        cloneMap.height = this.height;

        cloneMap.tiles = new TileData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (this.tiles[x, y] != null)
                {
                    cloneMap.tiles[x, y] = this.tiles[x, y].Clone();
                }
            }
        }
        return cloneMap;
    }
}
