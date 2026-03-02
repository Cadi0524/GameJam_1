using UnityEngine;
using System;


[Flags]
public enum TileColor
{
    None = 0,
    Stage1_Flower = 1 << 0, // 1
    Stage1_Moon = 1 << 1, // 2

    // ==================================
    Stage2_Flower = 1 << 2, // 4
    Stage2_Vase = 1 << 3, // 8
    Stage2_Cat = 1 << 4, // 16
    Stage2_SOFA = 1 << 5, // 32,


    //================================

    Stage3_Sun_Red = 1 << 6, // 64
    Stage3_Sun_Yellow = 1 << 7, // 128
    Stage3_Sun = Stage3_Sun_Red | Stage3_Sun_Yellow, // 빨강+노랑 = 주황 (192)
    Stage3_Ship = 1 << 8, // 256
    Stage3_Sail = 1 << 9, // 512
    Stage3_Human = 1 << 10, // 1024


    // ===========================
    Stage4_AirBalloonPurple_Red = 1 << 11, // 2048
    Stage4_AirBalloonPurple_Blue = 1 << 12, //
    Stage4_AirBalloonPurple = Stage4_AirBalloonPurple_Red | Stage4_AirBalloonPurple_Blue, // 보라색 풍선 (4096 + 2048 = 6144)
    Stage4_AirBallonYellow = 1 << 13, // 8192
    Stage4_AirBalloonGreen = 1 << 14, // 16384
    Stage4_AirBallonwPink = 1 << 15, // 32768
    Stage4_AirBallonMagenta = 1 << 16, // 65536
    Stage4_AirBallonEmerald = 1 << 17, // 131072


}
