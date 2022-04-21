using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

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
        data.deck = DeckToString(PlayerData.Deck);

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
            PlayerData.Deck = StringToDeck(data.deck);
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
            OptionMenu.AutoEndTurn = true;
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

            List<Card> allNonSpecialCards = CardCollection.Instance.GetNonSpecialCards();
            if (data.unlockDict.Count == allNonSpecialCards.Count &&
                    data.unlockDict.Keys.SequenceEqual(allNonSpecialCards.Select(card => card.CardName)))
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

    private static List<KeyValuePair<string, bool>> DeckToString(List<InGameCard> deck)
    {
        List<KeyValuePair<string, bool>> deckString = deck.Select(card =>
        {
            return new KeyValuePair<string, bool>(card.BaseCard.CardName, card.IsUpgraded);
        }).ToList();

        return deckString;
    }

    private static List<InGameCard> StringToDeck(List<KeyValuePair<string, bool>> deckString)
    {
        List<InGameCard> deck = deckString.Select(pair =>
        {
            return new InGameCard(CardCollection.Instance.FindCardByName(pair.Key), pair.Value);
        }).ToList();

        return deck;
    }
}
