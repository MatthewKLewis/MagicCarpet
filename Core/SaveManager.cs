using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static bool Save()
    {
        BinaryFormatter bF = new BinaryFormatter();

        string filePath = Application.persistentDataPath + "/playerSave.txt";
        FileStream fS = new FileStream(filePath, FileMode.Create);

        bF.Serialize(fS, "TODO - PLAYER INFO");
        fS.Close();
        return true;
    }

    public static string Load()
    {
        string filePath = Application.persistentDataPath + "/playerSave.txt";
        if (File.Exists(filePath))
        {
            BinaryFormatter bF = new BinaryFormatter();
            FileStream fS = new FileStream(filePath, FileMode.Open);

            string loadedData = bF.Deserialize(fS) as string;
            fS.Close();
            return loadedData;
        }
        else
        {
            return null;
        }
    }
}
