using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card List", menuName = "Card List")]
public class CardList : ScriptableObject
{
    public List<Card> cards;
}
