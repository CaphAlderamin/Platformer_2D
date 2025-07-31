using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveDataSystem
{
    
    public static void SaveData(HeroKnight Player, StatLevelSystem StatLevelSystem, string SaveFileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SaveFileName + ".veas";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(Player, StatLevelSystem);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("save success");
    }

    public static PlayerData LoadData(string SaveFileName)
    {
        string path = Application.persistentDataPath + "/" + SaveFileName + ".veas";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            Debug.Log("load success");
            return data;
        }
        else
        {
            Debug.LogError("Save file is not found in" + path);
            return null;
        }
    }
}
