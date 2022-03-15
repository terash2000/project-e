using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter(); 
	    FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat"); 

	    SaveData data = new SaveData();
        data.gold = PlayerData.gold;
        data.deck = PlayerData.deck;

	    formatter.Serialize(file, data);
	    file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
	    {
		    BinaryFormatter formatter = new BinaryFormatter();
		    FileStream file = File.Open(Application.persistentDataPath 
                    + "/SaveData.dat", FileMode.Open);
		    SaveData data = (SaveData)formatter.Deserialize(file);
		    file.Close();

            PlayerData.gold = data.gold;
            PlayerData.deck = data.deck;
	    }
    }

    public static void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
	    {
		    File.Delete(Application.persistentDataPath + "/SaveData.dat");
	    }
    }
}
