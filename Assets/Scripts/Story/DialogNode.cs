using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogNode : ScriptableObject
{
    public NodeType type;
    public NonPlayerCharacter character;
    public string quote;
    public Sprite background;
    public Sprite sprite;
    public List<DialogNode> child;
    public List<string> choice;
}
