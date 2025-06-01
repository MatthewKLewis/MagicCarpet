using UnityEngine;

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
    NPC_8 = 9,
}

public struct Castle
{
    public Castle(int x, int z, int l, OWNER_ID owner)
    {
        xOrigin = x;
        zOrigin = z;
        level = l;
        ownerID = owner;
    }

    public OWNER_ID ownerID;
    public int xOrigin;
    public int zOrigin;
    public int level;
}

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

public struct DestructionDeformation
{
    //Pre-process
    public bool noAnimation;

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

public struct BuildingDeformation
{

    //Informatation
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