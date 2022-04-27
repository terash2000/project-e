using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoryEvent : ScriptableObject
{
    public DialogNode DialogNode;
    public string Text;
    public Sprite Banner;
    public List<StoryEvent> PreviousEvents;
    public int UnlockCost;
}
