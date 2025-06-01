using UnityEngine;

public static class Deformations
{
    //First Row
    public static Vector2 BLANK = new Vector2(0, 0);
    public static Vector2 CASTLE_FORWARD = new Vector2(1, 0);
    public static Vector2 CASTLE_SIDE = new Vector2(2, 0);
    public static Vector2 CASTLE_CORNER = new Vector2(3, 0);
    //public static Vector2 aa = new Vector2(4, 0);
    //public static Vector2 aaa = new Vector2(5, 0);
    //public static Vector2 aaaa = new Vector2(6, 0);
    //public static Vector2 aaaaa = new Vector2(7, 0);

    //Second Row
    public static Vector2 RUBBLE = new Vector2(0, 1);
    public static Vector2 WATER = new Vector2(1, 1);

    public static BuildingDeformation CastleOrigin()
    {
        BuildingDeformation castle = new BuildingDeformation();

        //INFORMATION

        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        castle.heightOffsets = new float[5, 5] {
            { 0, 0, 0, 0, 0 },
            { 0, 5, 5, 5, 0 },
            { 0, 5, 5.5f, 5, 0 },
            { 0, 5, 5, 5, 0 },
            { 0, 0, 0, 0, 0 },
        };
        castle.colorChanges = new Color[5, 5] {
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.orange, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
        };

        //FENCESPANS - ALWAYS EVEN
        castle.uvBasisRemaps = new Vector2[4, 4]
        {
            {CASTLE_CORNER, CASTLE_FORWARD, CASTLE_FORWARD, CASTLE_CORNER},
            {CASTLE_SIDE, BLANK, BLANK, CASTLE_SIDE},
            {CASTLE_SIDE, BLANK, BLANK, CASTLE_SIDE},
            {CASTLE_CORNER, CASTLE_FORWARD, CASTLE_FORWARD, CASTLE_CORNER},
        };
        castle.triangleFlips = new bool[4, 4]
        {
            {true, false, false, false},
            {false, true, false, false},
            {false, false, true, false},
            {false, false, false, true},
        };
        return castle;
    }

    public static BuildingDeformation CastleUpgrade_1()
    {
        BuildingDeformation castle = new BuildingDeformation();
        return castle;
    }

    public static DestructionDeformation PockMark()
    {
        DestructionDeformation pock = new DestructionDeformation();

        pock.noAnimation = true;

        //FENCEPOSTS
        pock.heightOffsets = new float[2, 2] {
            { -0.5f, -0.5f,},
            { -0.5f, -0.5f,},
        };
        pock.colorChanges = new Color[2, 2] {
            { Color.gray3, Color.gray3},
            { Color.gray3, Color.gray3},
        };

        //FENCESPANS
        pock.uvBasisRemaps = new Vector2[1, 1]
        {
            {RUBBLE}
        };
        pock.triangleFlips = new bool[1, 1]
        {
            {true}
        };
        return pock;
    }
}
