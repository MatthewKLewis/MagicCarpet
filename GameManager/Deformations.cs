using System;
using UnityEngine;



public static class Deformations
{
    //Typical Colors
    public static Color G = Color.gray;
    public static Color G1 = Color.gray1;

    //First Row
    public static Vector2 NIL = new Vector2(0, 0);
    public static Vector2 CFO = new Vector2(1, 0);
    public static Vector2 CSI = new Vector2(2, 0);
    public static Vector2 CCO = new Vector2(3, 0);
    public static Vector2 RUF = new Vector2(4, 0);

    //public static Vector2 aaa = new Vector2(5, 0);
    //public static Vector2 aaaa = new Vector2(6, 0);
    //public static Vector2 aaaaa = new Vector2(7, 0);

    //Second Row
    public static Vector2 RUBBLE = new Vector2(0, 1);
    public static Vector2 WATER = new Vector2(1, 1);


    public static Deformation Lodge()
    {
        Deformation lodge = new Deformation();

        //INFORMATION
        lodge.noAnimation = true;
        lodge.runtime = false;
        lodge.deformationType = DEFORMATION_TYPE.BUILDING;
        lodge.ownerID = OWNER_ID.CITIZENS;


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
            {CCO, CFO, CFO, CCO},
            {CSI, RUF, RUF, CSI},
            {CSI, RUF, RUF, CSI},
            {CCO, CFO, CFO, CCO},
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

    public static Deformation HugeSpike()
    {
        Deformation spike = new Deformation();

        //INFORMATION
        spike.noAnimation = false;
        spike.runtime = true;
        spike.deformationType = DEFORMATION_TYPE.BUILDING;
        spike.ownerID = OWNER_ID.CITIZENS;

        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        spike.heightOffsets = new float[3, 3] {
            { 0, 0, 0,},
            { 0, 8, 0,},
            { 0, 0, 0,},
        };
        spike.colorChanges = new Color[3, 3] {
            { G, G, G,},
            { G, G, G,},
            { G, G, G,},
        };

        //FENCESPANS - ALWAYS EVEN
        spike.uvBasisRemaps = new Vector2[2, 2]
        {
            {NIL, NIL,},
            {NIL, NIL,},

        };
        spike.triangleFlips = new bool[2, 2]
        {
            {true, false,},
            {false, true,},

        };
        return spike;
    }

    public static Deformation CastleOrigin(OWNER_ID owner)
    {
        Deformation castle = new Deformation();

        //INFORMATION
        castle.noAnimation = false;
        castle.runtime = true;
        castle.deformationType = DEFORMATION_TYPE.CASTLE;
        castle.ownerID = owner;
        
        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        castle.heightOffsets = new float[5, 5] {
            {0,0,0,0,0,},
            {0,4,4,4,0,},
            {0,4,5,4,0,},
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
            {CCO, CFO, CFO, CCO},
            {CSI, NIL, NIL, CSI},
            {CSI, NIL, NIL, CSI},
            {CCO, CFO, CFO, CCO},
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

    public static Deformation CastleUpgrade_2(OWNER_ID owner)
    {
        Deformation castle = new Deformation();

        //INFORMATION
        castle.noAnimation = false;
        castle.runtime = true;
        castle.ownerID = owner;
        castle.deformationType = DEFORMATION_TYPE.CASTLE_UPGRADE;

        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        //CAN THIS ALLOW NULLS RATHER THAN FLOATS? TO SKIP?
        castle.heightOffsets = new float[13, 13] {
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,5,4,0,0,0,0,0,4,5,4,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,5,4,0,0,0,0,0,4,5,4,0,},
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
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},
            {CSI, RUF, RUF, CSI, NIL, NIL, NIL, NIL, CSI, RUF, RUF, CSI},
            {CSI, RUF, RUF, CSI, NIL, NIL, NIL, NIL, CSI, RUF, RUF, CSI},
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},
            {NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL},
            {NIL, NIL, NIL, NIL, CSI, RUF, RUF, CSI, NIL, NIL, NIL, NIL},
            {NIL, NIL, NIL, NIL, CSI, RUF, RUF, CSI, NIL, NIL, NIL, NIL},
            {NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL},
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},
            {CSI, RUF, RUF, CSI, NIL, NIL, NIL, NIL, CSI, RUF, RUF, CSI},
            {CSI, RUF, RUF, CSI, NIL, NIL, NIL, NIL, CSI, RUF, RUF, CSI},
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},

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

    public static Deformation PockMark()
    {
        Deformation pock = new Deformation();

        //INFORMATION
        pock.noAnimation = true;
        pock.runtime = true;
        pock.deformationType = DEFORMATION_TYPE.DESTRUCTION;
        pock.ownerID = OWNER_ID.UNOWNED;

        //FENCEPOSTS
        pock.heightOffsets = new float[2, 2] {
            { -0.5f, -0.5f,},
            { -0.5f, -0.5f,},
        };
        pock.colorChanges = new Color[2, 2] {
            { G1, G1},
            { G1, G1},
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
