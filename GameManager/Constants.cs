using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants
{
    //RED IS HEIGHT
    //GREEN IS OWNERID
    //BLUE IS UVBASIS

    public static int TILE_WIDTH = 2;
    public static int TILE_SPRITES = 8;
    public static int CHUNK_WIDTH = 32;
    //public static float MAX_HEIGHT = 64f;

    public static Vector2 GetUVBasisFromUVIndex(int index)
    {
        Vector2 retVec2 = Vector2.zero;
        retVec2.x = index % 8;
        retVec2.y = (int)(index / 8);
        return retVec2 / TILE_SPRITES;
    }
}