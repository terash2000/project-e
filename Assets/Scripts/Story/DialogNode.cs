using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogNode : ScriptableObject
{
    public NodeType Type;
    public NonPlayerCharacter Character;
    public string Quote;
    public Sprite Background;
    public Sprite Sprite;
    public StoryAction Action;
    public List<DialogNode> Child;
    public List<string> Choice;
    public Wave NextWave;
}
