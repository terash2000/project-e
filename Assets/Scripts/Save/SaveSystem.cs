using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");

        SaveData data = new SaveData();
        data.maxHealth = PlayerData.MaxHealth;
        data.health = PlayerData.Health;
        data.maxMana = PlayerData.MaxMana;
        data.gold = PlayerData.Gold;
        data.seedJSON = PlayerData.SeedJSON;
        data.path = PlayerData.Path;

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

            PlayerData.MaxHealth = data.maxHealth;
            PlayerData.Health = data.health;
            PlayerData.MaxMana = data.maxMana;
            PlayerData.Gold = data.gold;
            PlayerData.SeedJSON = data.seedJSON;
            PlayerData.Path = data.path;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            File.Delete(Application.persistentDataPath + "/SaveData.dat");
        }
    }

    public static void SaveOptionMenu()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Setting.dat");

        UserSetting setting = new UserSetting();
        setting.autoEndTurn = OptionMenu.AutoEndTurn;
        setting.showMonstersAttackArea = OptionMenu.ShowMonstersAttackArea;

        formatter.Serialize(file, setting);
        file.Close();
    }

    public static void LoadOptionMenu()
    {
        if (File.Exists(Application.persistentDataPath + "/Setting.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath
                    + "/Setting.dat", FileMode.Open);
            UserSetting setting = (UserSetting)formatter.Deserialize(file);
            file.Close();

            OptionMenu.AutoEndTurn = setting.autoEndTurn;
            OptionMenu.ShowMonstersAttackArea = setting.showMonstersAttackArea;
        }
        else
        {
            OptionMenu.AutoEndTurn = false;
            OptionMenu.ShowMonstersAttackArea = true;
        }
    }

    public static void SaveUnlockData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/UnlockData.dat");

        UnlockData data = new UnlockData();
        data.unlockDict = CardCollection.UnlockDict;
        data.completedStory = StoryMenu.CompletedStory;

        formatter.Serialize(file, data);
        file.Close();
    }

    public static void LoadUnlockData()
    {
        if (File.Exists(Application.persistentDataPath + "/UnlockData.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath
                    + "/UnlockData.dat", FileMode.Open);
            UnlockData data = (UnlockData)formatter.Deserialize(file);
            file.Close();

            if (data.unlockDict.Count == CardCollection.Instance.AllCards.Count)
            {
                CardCollection.UnlockDict = data.unlockDict;
                StoryMenu.CompletedStory = data.completedStory;
            }
            else
            {
                // corrupted data or old verision
                File.Delete(Application.persistentDataPath + "/UnlockData.dat");
                CardCollection.UnlockDict = CardCollection.Instance.StarterCards();
                StoryMenu.CompletedStory = new List<string>();
            }
        }
        else
        {
            CardCollection.UnlockDict = CardCollection.Instance.StarterCards();
            StoryMenu.CompletedStory = new List<string>();
        }
    }
}
