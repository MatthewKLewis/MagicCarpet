using System;
using UnityEngine;

public static class Deformations
{
    //Typical Colors
    public static Color G = Color.gray;

    //First Row
    public static Vector2 BLANK = new Vector2(0, 0);
    public static Vector2 C_FORWARD = new Vector2(1, 0);
    public static Vector2 C_SIDE = new Vector2(2, 0);
    public static Vector2 C_CORNER = new Vector2(3, 0);
    //public static Vector2 aa = new Vector2(4, 0);
    //public static Vector2 aaa = new Vector2(5, 0);
    //public static Vector2 aaaa = new Vector2(6, 0);
    //public static Vector2 aaaaa = new Vector2(7, 0);

    //Second Row
    public static Vector2 RUBBLE = new Vector2(0, 1);
    public static Vector2 WATER = new Vector2(1, 1);

    /*
    * 
    * Neutral Buildings
    * 
    */
    public static BuildingDeformation Lodge()
    {
        BuildingDeformation lodge = new BuildingDeformation();

        //INFORMATION

        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        lodge.heightOffsets = new float[5, 5] {
            { 0, 0, 0, 0, 0 },
            { 0, 4, 4, 4, 0 },
            { 0, 4, 4, 4, 0 },
            { 0, 4, 4, 4, 0 },
            { 0, 0, 0, 0, 0 },
        };
        lodge.colorChanges = new Color[5, 5] {
            { G, G, G, G, G },
            { G, G, G, G, G },
            { G, G, G, G, G },
            { G, G, G, G, G },
            { G, G, G, G, G },
        };

        //FENCESPANS - ALWAYS EVEN
        lodge.uvBasisRemaps = new Vector2[4, 4]
        {
            {C_CORNER, C_FORWARD, C_FORWARD, C_CORNER},
            {C_SIDE, BLANK, BLANK, C_SIDE},
            {C_SIDE, BLANK, BLANK, C_SIDE},
            {C_CORNER, C_FORWARD, C_FORWARD, C_CORNER},
        };
        lodge.triangleFlips = new bool[4, 4]
        {
            {true, false, false, false},
            {false, false, false, false},
            {false, false, false, false},
            {false, false, false, true},
        };
        return lodge;
    }

    /*
     * 
     * Player Castles
     * 
     */
    public static BuildingDeformation CastleOrigin()
    {
        BuildingDeformation castle = new BuildingDeformation();

        //INFORMATION

        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        castle.heightOffsets = new float[5, 5] {
            {0,0,0,0,0,},
            {0,4,4,4,0,},
            {0,4,4,4,0,},
            {0,4,4,4,0,},
            {0,0,0,0,0,},
        };
        castle.colorChanges = new Color[5, 5] {
            { G, G, G, G, G },
            { G, G, G, G, G },
            { G, G, G, G, G },
            { G, G, G, G, G },
            { G, G, G, G, G },
        };

        //FENCESPANS - ALWAYS EVEN
        castle.uvBasisRemaps = new Vector2[4, 4]
        {
            {C_CORNER, C_FORWARD, C_FORWARD, C_CORNER},
            {C_SIDE, BLANK, BLANK, C_SIDE},
            {C_SIDE, BLANK, BLANK, C_SIDE},
            {C_CORNER, C_FORWARD, C_FORWARD, C_CORNER},
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

    public static BuildingDeformation CastleUpgrade_2()
    {
        //How to do a hole?

        BuildingDeformation castle = new BuildingDeformation();

        //INFORMATION


        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        //CAN THIS ALLOW NULLS RATHER THAN FLOATS? TO SKIP?
        castle.heightOffsets = new float[13, 13] {
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,0,0,0,0,4,4,4,0,0,0,0,0,},
            { 0,0,0,0,0,4,4,4,0,0,0,0,0,},
            { 0,0,0,0,0,4,4,4,0,0,0,0,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
        };
        castle.colorChanges = new Color[13, 13] {
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
            {G,G,G,G,G,G,G,G,G,G,G,G,G,},
        };

        //FENCESPANS - ALWAYS EVEN
        castle.uvBasisRemaps = new Vector2[12, 12]
        {
            {C_CORNER, C_FORWARD, C_FORWARD, C_CORNER, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {C_CORNER, BLANK, BLANK, C_CORNER, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},
            {BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK, BLANK},

        };
        castle.triangleFlips = new bool[12, 12]
        {
            {true, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, true, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false},

        };
        return castle;
    }


    /*
     * 
     * Destruction
     * 
     */
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
            { G, G},
            { G, G},
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
