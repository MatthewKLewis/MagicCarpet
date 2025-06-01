using UnityEngine;

public struct Square
{
    public Vector2 uvBasis;
    public bool triangleFlipped;
    public int castleID;

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

public struct Deformation
{
    //Pre-process
    public bool flattenFirst;
    public bool markCastleID;

    //Vertex based
    public float[,] heightOffsets;
    public Color[,] colorChanges;

    //Square based
    public Vector2[,] uvBasisRemaps;
    public bool[,] triangleFlips;

    public override string ToString()
    {

        return $"Deformation(\n" +
               $"  flattenFirst: {flattenFirst},\n" +
               $"  heightOffsets: {heightOffsets[0, 0]},\n" +
               $"  colorChanges: {colorChanges[0, 0]},\n" +
               $"  uvBasisRemaps: {uvBasisRemaps[0, 0]},\n" +
               $"  triangleFlips: {triangleFlips[0, 0]}\n" +
               $")";
    }
}