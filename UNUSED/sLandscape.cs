using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class sLandscape : MonoBehaviour
{
    private Terrain terrain;
    public Texture2D heightmapTexture;

    public float[,] heights;
    public float[,] targetHeights;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        heights = GetHeightArrayFromImage(heightmapTexture);
        targetHeights = GetHeightArrayFromImage(heightmapTexture);
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    void Update()
    {
        for (int x = 0; x < 64; x++)
        {
            for (int z = 0; z < 64; z++)
            {
                heights[x, z] = Mathf.Lerp(heights[x, z], targetHeights[x, z], 0.01f);
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);

    }

    private float[,] GetHeightArrayFromImage(Texture2D tex)
    {
        float[,] array = new float[tex.width + 1, tex.height + 1]; //inexplicably + 1
        for (int x = 0; x < tex.width; x++)
        {
            for (int z = 0; z < tex.height; z++)
            {
                array[x, z] = tex.GetPixel(x,z).r / 5;
            }
        }
        return array;
    }

    public void TakeSmallHit(Vector3 hitPoint)
    {
        int coordX = (int)(hitPoint.x / 15.625);
        int coordZ = (int)(hitPoint.z / 15.625);
        Debug.Log(coordX +", "+ coordZ);
        targetHeights[coordZ, coordX] = 0f;//heights[coordX, coordY] * 0.95f;
    }
}
