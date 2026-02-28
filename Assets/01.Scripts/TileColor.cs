using UnityEngine;
using System;


[Flags]
public enum TileColor 
{
    None = 0,
    Red = 1 << 0, // 1
    Green = 1 << 1, // 2
    Blue = 1 << 2, // 4
}
