using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static void Save(SaveData data, string s){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.dataPath, s + ".bin");
        FileStream stream = File.Create(path);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData Load(string s){
        try{
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.dataPath, s + ".bin");
        FileStream stream = File.OpenRead(path);
        SaveData data = (SaveData) formatter.Deserialize(stream);
        stream.Close();
        return data;
        }
        catch(Exception e){
            Debug.Log(e.Message);
            return null;
        }
    }
}
