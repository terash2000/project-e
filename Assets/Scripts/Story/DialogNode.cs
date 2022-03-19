using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogNode: ScriptableObject
{
    public NodeType type;
    public NonPlayerCharacter character;
    public string quote;
    public List<DialogNode> child;
    public List<string> choice;
}
