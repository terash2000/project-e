using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoryEvent : ScriptableObject
{
    public DialogNode DialogNode;
    public string Text;
    public List<StoryEvent> PreviousEvents;
}
