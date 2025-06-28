using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants
{
    //public static int TILE_WIDTH = 2;
    //public static int TILE_SPRITES = 8;
    //public static int CHUNK_WIDTH = 32;
    //public static float HEIGHT_MAP_MULTIPLIER = 0.5f;
    //public static Vector2 GetUVBasisFromUVIndex(int index)
    //{
    //    Vector2 retVec2 = Vector2.zero;
    //    retVec2.x = index % 8;
    //    retVec2.y = (int)(index / 8);
    //    return retVec2 / TILE_SPRITES;
    //}

    public static T[,] FillArray<T>(T[,] array, T value)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = value;
            }
        }
        return array;
    }
}