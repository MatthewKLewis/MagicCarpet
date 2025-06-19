using System;
using UnityEngine;



public static class Deformations
{
    //Black and White, for brevity.
    public static Color B = Color.black;
    public static Color W = Color.white;

    //First Row
    public static int NIL = 0;
    public static int CCO = 1;
    public static int CFO = 2;
    public static int CSI = 3;
    public static int RUF = 4;
    public static int RUB = 5;
    //Second Row

    public static Deformation Lodge = new Deformation(
        true,
        true,
        DEFORMATION_TYPE.BUILDING, 
        OWNER_ID.CITIZENS, 
        new float[6, 6] {
            { 0, 0, 0, 0, 0, 0 },
            { 0, 4, 4, 4, 4, 0 },
            { 0, 4, 4, 4, 4, 0 },
            { 0, 4, 4, 4, 4, 0 },
            { 0, 4, 4, 4, 4, 0 },
            { 0, 0, 0, 0, 0, 0 },
        }, new int[5, 5]
        {
            {CFO, CFO, CFO, CFO, CFO},
            {CFO, CFO, CFO, CFO, CFO},
            {CFO, CFO, CFO, CFO, CFO},
            {CFO, CFO, CFO, CFO, CFO},
            {CFO, CFO, CFO, CFO, CFO},

        }, new bool[5, 5]
        {
            {true, false, false, false, false},
            {false, true, false, false, false},
            {false, false, true, false, false},
            {false, false, false, true, false},
            {false, false, false, false, true},

        }
    );

    public static Deformation HugeSpike()
    {
        Deformation spike = new Deformation();

        //INFORMATION
        spike.noAnimation = false;
        spike.deformationType = DEFORMATION_TYPE.BUILDING;
        spike.ownerID = OWNER_ID.CITIZENS;

        //FENCEPOSTS - ALWAYS ODD BECAUSE WE WANT A LOVELY PEAKY ROOF
        spike.heightOffsets = new float[3, 3] {
            { 0, 0, 0,},
            { 0, 8, 0,},
            { 0, 0, 0,},
        };

        //FENCESPANS - ALWAYS EVEN
        spike.uvBasisRemaps = new int[2, 2]
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

        //FENCESPANS - ALWAYS EVEN
        castle.uvBasisRemaps = new int[4, 4]
        {
            {CCO, CFO, CFO, CCO},
            {CFO, NIL, NIL, CFO},
            {CFO, NIL, NIL, CFO},
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
            { 0,0,0,0,0,4,4,4,0,0,0,0,0,},
            { 0,0,0,0,0,4,5,4,0,0,0,0,0,},
            { 0,0,0,0,0,4,4,4,0,0,0,0,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,4,5,4,0,0,0,0,0,4,5,4,0,},
            { 0,4,4,4,0,0,0,0,0,4,4,4,0,},
            { 0,0,0,0,0,0,0,0,0,0,0,0,0,},
        };

        //FENCESPANS - ALWAYS EVEN
        castle.uvBasisRemaps = new int[12, 12]
        {
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},
            {CFO, RUF, RUF, CFO, NIL, NIL, NIL, NIL, CFO, RUF, RUF, CFO},
            {CFO, RUF, RUF, CFO, NIL, NIL, NIL, NIL, CFO, RUF, RUF, CFO},
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},
            {NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL},
            {NIL, NIL, NIL, NIL, CFO, RUF, RUF, CFO, NIL, NIL, NIL, NIL},
            {NIL, NIL, NIL, NIL, CFO, RUF, RUF, CFO, NIL, NIL, NIL, NIL},
            {NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL},
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},
            {CFO, RUF, RUF, CFO, NIL, NIL, NIL, NIL, CFO, RUF, RUF, CFO},
            {CFO, RUF, RUF, CFO, NIL, NIL, NIL, NIL, CFO, RUF, RUF, CFO},
            {CCO, CFO, CFO, CCO, NIL, NIL, NIL, NIL, CCO, CFO, CFO, CCO},

        };
        castle.triangleFlips = new bool[12, 12]
        {
            {true, false, false, false, false, false, false, false, true, false, false, false},
            {false, true, false, false, false, false, false, false, false, true, false, false},
            {false, false, true, false, false, false, false, false, false, false, true, false},
            {false, false, false, true, false, false, false, false, false, false, false, true},
            {false, false, false, false, true, false, false, false, false, false, false, false},
            {false, false, false, false, false, true, false, false, false, false, false, false},
            {false, false, false, false, false, false, true, false, false, false, false, false},
            {false, false, false, false, false, false, false, true, false, false, false, false},
            {true, false, false, false, false, false, false, false, true, false, false, false},
            {false, true, false, false, false, false, false, false, false, true, false, false},
            {false, false, true, false, false, false, false, false, false, false, true, false},
            {false, false, false, true, false, false, false, false, false, false, false, true},

        };
        return castle;
    }

    public static Deformation PockMark()
    {
        Deformation pock = new Deformation();

        //INFORMATION
        pock.noAnimation = true;
        pock.deformationType = DEFORMATION_TYPE.DESTRUCTION;
        pock.ownerID = OWNER_ID.UNOWNED;

        //FENCEPOSTS
        pock.heightOffsets = new float[2, 2] {
            { -0.5f, -0.5f,},
            { -0.5f, -0.5f,},
        };
        //pock.colorChanges = new Color[2, 2] {
        //    { B, B },
        //    { B, B },
        //};

        //FENCESPANS
        pock.uvBasisRemaps = new int[0, 0] {};
        pock.triangleFlips = new bool[0,0] {};
        return pock;
    }
}
