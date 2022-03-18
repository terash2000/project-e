using UnityEngine;

[CreateAssetMenu]
public class DialogNode: ScriptableObject
{
    public NonPlayerCharacter character;
    public string quote;
    public DialogNode child;
}
