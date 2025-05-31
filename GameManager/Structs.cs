using UnityEngine;

public struct Square
{
    public Vector2 uv00;
    public Vector2 uv10;
    public Vector2 uv01;
    public Vector2 uv11;
    public bool triangleFlipped;

    //public Vector2 uv200;
    //public Vector2 uv210;
    //public Vector2 uv201;
    //public Vector2 uv211;

    public override string ToString()
    {
        return $"Square:\n" +
               $"  Primary UVs:\n" +
               $"    uv00: {uv00}, uv10: {uv10}, uv01: {uv01}, uv11: {uv11}\n" +
               //$"  Secondary UVs:\n" +
               //$"    uv200: {uv200}, uv210: {uv210}, uv201: {uv201}, uv211: {uv211}\n" +
               $"  Triangle Flipped: {triangleFlipped}";
    }
}

public struct Vertex
{
    public float height;
    public Color color;
}

public struct TileDeformation
{
    float vec00Adj;
    float vec10Adj;
    float vec01Adj;
    float vec11Adj;
    Color vec00Color;
    Color vec10Color;
    Color vec01Color;
    Color vec11Color;
    Vector2 uv00;
    Vector2 uv10;
    Vector2 uv01;
    Vector2 uv11;
}