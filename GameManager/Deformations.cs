using UnityEngine;

public static class Deformations
{
    public static Deformation returnCastleDeformation()
    {
        Deformation castleDeformation = new Deformation();
        castleDeformation.flattenFirst = true;

        //FENCEPOSTS
        castleDeformation.heightOffsets = new float[5, 5] {
            { 0, 0, 0, 0, 0 },
            { 0, 5, 5, 5, 0 },
            { 0, 5, 5.5f, 5, 0 },
            { 0, 5, 5, 5, 0 },
            { 0, 0, 0, 0, 0 },
        };
        castleDeformation.colorChanges = new Color[5, 5] {
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.gray, Color.gray, Color.gray, Color.gray },
        };

        //FENCESPANS
        castleDeformation.uvBasisRemaps = new Vector2[4, 4]
        {
            {Vector2.zero, Vector2.zero, Vector2.zero,Vector2.zero,},
            {Vector2.zero, Vector2.zero, Vector2.zero,Vector2.zero,},
            {Vector2.zero, Vector2.zero, Vector2.zero,Vector2.zero,},
            {Vector2.zero, Vector2.zero, Vector2.zero,Vector2.zero,},
        };
        castleDeformation.triangleFlips = new bool[4, 4]
        {
            {true, false, false, false},
            {false, true, false, false},
            {false, false, true, false},
            {false, false, false, true},
        };
        return castleDeformation;
    }

    public static Deformation returnPockMark()
    {
        Deformation castleDeformation = new Deformation();
        castleDeformation.flattenFirst = false;

        //FENCEPOSTS
        castleDeformation.heightOffsets = new float[2, 2] {
            { -0.5f, -0.5f,},
            { -0.5f, -0.5f,},
        };
        castleDeformation.colorChanges = new Color[2, 2] {
            { Color.pink, Color.pink},
            { Color.pink, Color.pink},
        };

        //FENCESPANS
        castleDeformation.uvBasisRemaps = new Vector2[1, 1]
        {
            {new Vector2(2,2)}
        };
        castleDeformation.triangleFlips = new bool[1, 1]
        {
            {true}
        };
        return castleDeformation;
    }
}
