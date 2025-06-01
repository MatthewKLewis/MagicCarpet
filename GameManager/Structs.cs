using UnityEngine;

public enum CASTLE_ID
{
    NONE = 0,
    PLAYER = 1,
    ENEMY_1 = 2,
    ENEMY_2 = 3,
    ENEMY_3 = 4,
}

public struct Square
{
    public Vector2 uvBasis;
    public bool triangleFlipped;
    public CASTLE_ID castleID;

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
    public CASTLE_ID castleID;

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