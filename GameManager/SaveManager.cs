//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEngine;

//[System.Serializable]
//public class SerializableColor
//{
//    public float[] colorStore = new float[4] { 1F, 1F, 1F, 1F };
//    public Color Color
//    {
//        get { return new Color(colorStore[0], colorStore[1], colorStore[2], colorStore[3]); }
//        set { colorStore = new float[4] { value.r, value.g, value.b, value.a }; }
//    }

//    //makes this class usable as Color, Color normalColor = mySerializableColor;
//    public static implicit operator Color(SerializableColor instance)
//    {
//        return instance.Color;
//    }

//    //makes this class assignable by Color, SerializableColor myColor = Color.white;
//    public static implicit operator SerializableColor(Color color)
//    {
//        return new SerializableColor { Color = color };
//    }
//}

//[System.Serializable]
//public class LevelGeoData
//{
//    //Unpacks to Squares
//    public bool[,] triFlips;
//    public byte[,] uvBases;
//    public OWNER_ID[,] owners;

//    //Unpacks to Vertices
//    public float[,] heights;
//    public SerializableColor[,] colors;

//    public LevelGeoData(Square[,] sqs, Vertex[,] verts)
//    {
//        //SQ
//        triFlips = new bool[sqs.GetLength(0), sqs.GetLength(1)];
//        uvBases = new byte[sqs.GetLength(0), sqs.GetLength(1)];
//        owners = new OWNER_ID[sqs.GetLength(0), sqs.GetLength(1)];

//        //VERT
//        heights = new float[verts.GetLength(0), verts.GetLength(1)];
//        colors = new SerializableColor[verts.GetLength(0), verts.GetLength(1)];

//        for (int z = 0; z < sqs.GetLength(0); z++)
//        {
//            for (int x = 0; x < sqs.GetLength(1); x++)
//            {
//                triFlips[x, z] = sqs[x, z].triangleFlipped;

//                //int uvX = (int)(sqs[x, z].uvBasis.x * 8);
//                //int uvZ = (int)(sqs[x, z].uvBasis.y * 8);
//                uvBases[x, z] = 0; //(byte)((uvZ * 8) + uvX); //TODO - unpack this properly
//                owners[x, z] = sqs[x, z].ownerID;
//            }
//        }

//        for (int z = 0; z < verts.GetLength(0); z++)
//        {
//            for (int x = 0; x < verts.GetLength(1); x++)
//            {
//                heights[x, z] = (verts[x, z].height);
//                colors[x, z] = (verts[x, z].color); //TODO - test this 
//            }
//        }
//    }
//}

//public static class SaveManager
//{
//    public static bool SaveLevel(int levelNumber, Square[,] squares, Vertex[,] verts)
//    {
//        BinaryFormatter bF = new BinaryFormatter();

//        string filePath = Application.persistentDataPath + "/level_9.txt";
//        FileStream fS = new FileStream(filePath, FileMode.Create);

//        LevelGeoData lgData = new LevelGeoData(squares, verts);
//        bF.Serialize(fS, lgData);
//        fS.Close();
//        return true;
//    }

//    public static LevelGeoData LoadLevel(int levelNumber)
//    {
//        string filePath = Application.persistentDataPath + "/level_9.txt";
//        if (File.Exists(filePath)) 
//        {
//            BinaryFormatter bF = new BinaryFormatter();
//            FileStream fS = new FileStream(filePath, FileMode.Open);

//            LevelGeoData lgData = bF.Deserialize(fS) as LevelGeoData;
//            fS.Close();
//            return lgData;
//        }
//        else
//        {
//            return null;
//        }        
//    }
//}
