using UnityEngine;
using System;


[Flags]
public enum TileColor
{
    None = 0,
    Stage1_Flower = 1 << 0, // 1
    Stage1_Moon = 1 << 1, // 2
    Blue = 1 << 2, // 4

    Yellow = 1 << 3, // 8 
}
