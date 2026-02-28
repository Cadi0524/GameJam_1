using UnityEngine;
using Sirenix.OdinInspector;

public class TileMap
{
    public int width;
    public int height;


    [ShowInInspector]
    [TableMatrix(SquareCells = true)]
    public TileData[,] tiles;

    public TileMap(int width, int height)
    {
        this.width = width;
        this.height = height;
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

}
