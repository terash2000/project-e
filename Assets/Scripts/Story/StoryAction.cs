using System;
using UnityEngine;

[Serializable]
public class StoryAction
{
    public ActionType type;
    public int amount;
    public string name;

    public enum ActionType
    {
        None,
        UnlockCard,
        ChangeHP
    }

    public void Trigger()
    {
        switch (type)
        {
            case ActionType.UnlockCard:
                CardCollection.singleton.UnlockCard(name);
                break;
            case ActionType.ChangeHP:
                PlayerData.Health += amount;
                break;
        }
    }
}
