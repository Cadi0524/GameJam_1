using UnityEngine;

public class TileData
{

    public TileType type = TileType.None;
    public TileColor color = TileColor.None;

    public int x;
    public int y;

    public TileData(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    //비어있는지 체크

    public bool IsEmpty()
    {
        if (type != TileType.None)
        {
            return false;
        }
        else
        {
            if (color != TileColor.None)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public TileData Clone()
    {
        TileData clone = new TileData(this.x, this.y);
        clone.type = this.type;
        clone.color = this.color;
        return clone;
    }
}
