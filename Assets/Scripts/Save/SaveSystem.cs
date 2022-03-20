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
        data.maxHealth = PlayerData.maxHealth;
        data.health = PlayerData.health;
        data.gold = PlayerData.gold;

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

            PlayerData.maxHealth = data.maxHealth;
            PlayerData.health = data.health;
            PlayerData.gold = data.gold;
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
