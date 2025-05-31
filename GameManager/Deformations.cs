using UnityEngine;

public static class Deformations
{
    public static Deformation returnCastleDeformation()
    {
        //TODO - Make a public static resource class for various useful deforms
        Deformation castleDeformation = new Deformation();
        castleDeformation.flattenFirst = true;
        castleDeformation.heightOffsets = new float[3, 3] {
            { 5, 5, 5 },
            { 5, 5.5f, 5 },
            { 5, 5, 5 },
        };
        castleDeformation.colorChanges = new Color[3, 3] {
            { Color.gray, Color.gray, Color.gray },
            { Color.gray, Color.orange, Color.gray },
            { Color.gray, Color.gray, Color.gray }
        };
        castleDeformation.uvBasisRemaps = new Vector2[2, 2]
        {
            {Vector2.zero, Vector2.zero, },
            {Vector2.zero, Vector2.zero, },
        };
        castleDeformation.triangleFlips = new bool[2, 2]
        {
            {true, false, },
            {false, true, },
        };
        return castleDeformation;
    }

    public static Deformation returnCastleDeformation_NEW()
    {
        //TODO - Make a public static resource class for various useful deforms
        Deformation castleDeformation = new Deformation();
        castleDeformation.flattenFirst = true;
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
}
