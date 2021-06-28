using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MalnourishedMania
{
    public static class SaveSystem
    {
        public static void SaveData(PlayerData playerData) //create data on a gamemanager class or something, update it with new data then pass that into this
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/MMdata.txt";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, playerData);
            stream.Close();
        }

        public static PlayerData LoadData()
        {
            string path = Application.persistentDataPath + "/MMdata.txt";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                return data;
            }
            else
            {
                Debug.LogError("Save file not found in " + path);
                return null;
            }

        }
    }
}

