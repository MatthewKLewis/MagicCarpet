using UnityEngine;

public enum OWNER_ID
{
    UNOWNED,
    PLAYER,
    NPC_1,
    NPC_2,
    NPC_3,
    NPC_4,
    NPC_5,
    NPC_6,
    NPC_7,
    CITIZENS,
}

public enum DEFORMATION_TYPE
{
    CASTLE = 0,
    DESTRUCTION = 1,
    BUILDING = 2,
    CASTLE_UPGRADE = 3,
}

public struct Square
{
    public bool triangleFlipped;
    public int uvBasis;

    public Square(int uvB, bool isTriFlipped, OWNER_ID owner)
    {
        uvBasis = uvB;
        triangleFlipped = isTriFlipped;
    }

    public override string ToString()
    {
        return $"  UV Basis: ({uvBasis}) " +
               $"  Triangle Flipped: {triangleFlipped} ";
    }
}

public struct Vertex
{
    public float height;
    public Color color;
    public Vector3 normal;
    public OWNER_ID ownerID;

    public Vertex(float h, Color col, OWNER_ID ownId, Vector3 norm)
    {
        height = h;
        color = col;
        normal = norm;
        ownerID = ownId;
    }
}

public struct Castle
{
    public Castle(int x, float y, int z, int l, OWNER_ID owner, int h, int maxH)
    {
        xOrigin = x;
        yOrigin = y;
        zOrigin = z;
        level = l;
        ownerID = owner;
        health = h;
        maxHealth = maxH;
    }

    public OWNER_ID ownerID;
    public int xOrigin;
    public float yOrigin;
    public int zOrigin;
    public int level;
    public int health;
    public int maxHealth;

    public override string ToString()
    {
        return $"Castle {{ OwnerID = {ownerID}, X = {xOrigin}, Y = {yOrigin}, Z = {zOrigin}, Level = {level} }}";
    }
}

public struct Deformation
{
    //Informatation
    public bool noAnimation;
    public bool flattenFirst;

    public DEFORMATION_TYPE deformationType;
    public OWNER_ID ownerID;

    //Vertex based
    public float[,] heightOffsets;

    //Square based
    public int[,] uvBasisRemaps;
    public bool[,] triangleFlips;

    public Deformation(bool noA, bool flatFirst, DEFORMATION_TYPE defType, OWNER_ID ownID, float[,] heights, int[,]uvs, bool[,] triFlips)
    {
        noAnimation = noA;
        flattenFirst = flatFirst;
        deformationType = defType;
        ownerID = ownID;
        heightOffsets = heights;
        uvBasisRemaps = uvs;
        triangleFlips = triFlips;
    }

    public override string ToString()
    {
        return $"Deformation(\n" +
               $"  heightOffsets: {heightOffsets[0, 0]},\n" +
               $"  uvBasisRemaps: {uvBasisRemaps[0, 0]},\n" +
               $"  triangleFlips: {triangleFlips[0, 0]}\n" +
               $")";
    }
}

