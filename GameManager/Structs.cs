using UnityEngine;

#region TERRAIN CHUNK STRUCTS

public struct Square
{
    public Vector2 uvBasis;
    public bool triangleFlipped;
    public OWNER_ID ownerID;

    public Square(Vector2 uv, bool isTriFlipped, OWNER_ID owner)
    {
        uvBasis = uv;
        triangleFlipped = isTriFlipped;
        ownerID = owner;
    }

    public override string ToString()
    {
        return $"  UV Basis: ({uvBasis.x:F3}, {uvBasis.y:F3}) " +
               $"  Triangle Flipped: {triangleFlipped} " +
               $"  Owner ID: {ownerID}";
    }
}

public struct Vertex
{
    public float height;
    public Color color;

    public Vertex(float h, Color col)
    {
        height = h;
        color = col;
    }
}
#endregion


#region CASTLE STRUCTS

public enum OWNER_ID
{
    PLAYER,
    NPC_1,
    NPC_2,
    NPC_3,
    NPC_4,
    NPC_5,
    NPC_6,
    NPC_7,
    CITIZENS,
    UNOWNED,
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