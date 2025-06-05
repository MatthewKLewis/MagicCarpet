using UnityEngine;

#region TERRAIN CHUNK STRUCTS
public struct Square
{
    public Vector2 uvBasis;
    public bool triangleFlipped;
    public OWNER_ID ownerID;

    public override string ToString()
    {
        return $"Square:\n" +
               $"  Triangle Flipped: {triangleFlipped}";
    }
}

public struct Vertex
{
    public float height;
    public Color color;
}
#endregion


#region CASTLE STRUCTS

public enum OWNER_ID
{
    NONE = 0,
    PLAYER = 1,
    //
    NPC_1 = 2,
    NPC_2 = 3,
    NPC_3 = 4,
    NPC_4 = 5,
    NPC_5 = 6,
    NPC_6 = 7,
    NPC_7 = 8,
    UNOWNED = 9,
}

public struct Castle
{
    public Castle(int x, float y, int z, int l, OWNER_ID owner)
    {
        xOrigin = x;
        yOrigin = y;
        zOrigin = z;
        level = l;
        ownerID = owner;
    }

    public OWNER_ID ownerID;
    public int xOrigin;
    public float yOrigin;
    public int zOrigin;
    public int level;

    public override string ToString()
    {
        return $"Castle {{ OwnerID = {ownerID}, X = {xOrigin}, Y = {yOrigin}, Z = {zOrigin}, Level = {level} }}";
    }
}

#endregion


#region DEFORMATION STRUCTS
public enum DEFORMATION_TYPE
{
    CASTLE = 0,
    DESTRUCTION = 1,
    BUILDING = 2,
    CASTLE_UPGRADE = 3,
}

public struct Deformation
{
    //Informatation
    public bool noAnimation;
    public bool runtime;

    public DEFORMATION_TYPE deformationType;
    public OWNER_ID ownerID;

    //Vertex based
    public float[,] heightOffsets;
    public Color[,] colorChanges;

    //Square based
    public Vector2[,] uvBasisRemaps;
    public bool[,] triangleFlips;

    public override string ToString()
    {

        return $"Deformation(\n" +
               $"  heightOffsets: {heightOffsets[0, 0]},\n" +
               $"  colorChanges: {colorChanges[0, 0]},\n" +
               $"  uvBasisRemaps: {uvBasisRemaps[0, 0]},\n" +
               $"  triangleFlips: {triangleFlips[0, 0]}\n" +
               $")";
    }
}

#endregion