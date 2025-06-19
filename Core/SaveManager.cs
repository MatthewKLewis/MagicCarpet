using System.IO;
using UnityEngine;

public static class ImageSaver
{
    public static void SaveImageFromHeights(Vertex[,] vertexMap, Square[,] squareMap)
    {
        Texture2D heightMap = new Texture2D(vertexMap.GetLength(0), vertexMap.GetLength(1), TextureFormat.RGBA32, false);
        for (int z = 0; z < vertexMap.GetLength(1) - 1; z++)
        {
            for (int x = 0; x < vertexMap.GetLength(0) - 1; x++)
            {
                float heightValue = vertexMap[x, z].height / (256f * Constants.HEIGHT_MAP_MULTIPLIER);
                heightMap.SetPixel(x, z, new Color(heightValue, heightValue, heightValue));
            }
        }
        var dirPath = "D:\\Unity\\Magic Carpet 3\\Assets\\Images\\Level Images\\Working\\";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        byte[] heightBytes = heightMap.EncodeToPNG();
        File.WriteAllBytes(dirPath + "Heights.png", heightBytes);



        //TODO - VERTEX COLORS (aka triplananar - UVtexture mask)
        Texture2D vertexColorMap = new Texture2D(vertexMap.GetLength(0), vertexMap.GetLength(1), TextureFormat.RGBA32, false);
        for (int z = 0; z < vertexMap.GetLength(1) - 1; z++)
        {
            for (int x = 0; x < vertexMap.GetLength(0) - 1; x++)
            {
                Color vertexColor = vertexMap[x, z].color;
                vertexColorMap.SetPixel(x, z, vertexColor);
            }
        }
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        byte[] colorBytes = vertexColorMap.EncodeToPNG();
        File.WriteAllBytes(dirPath + "VertexColors.png", colorBytes);



        //TODO - UVBASES (aka uv texture subsection)
        Texture2D uvBaseMap = new Texture2D(squareMap.GetLength(0), squareMap.GetLength(1), TextureFormat.RGBA32, false);
        for (int z = 0; z < squareMap.GetLength(1); z++)
        {
            for (int x = 0; x < squareMap.GetLength(0); x++)
            {
                int uvBasis = squareMap[x, z].uvBasis;
                uvBaseMap.SetPixel(x, z, new Color((float)uvBasis / 256f, (float)uvBasis / 256f, (float)uvBasis / 256f));
            }
        }
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        byte[] uvBytes = uvBaseMap.EncodeToPNG();
        File.WriteAllBytes(dirPath + "UVIndices.png", uvBytes);



        //TODO - triangleflips
    }
}
