using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCollection : MonoBehaviour
{
    public static CardCollection singleton;
    public List<Card> commonCards;
    public List<Card> rareCards;
    [HideInInspector] public List<Card> allCards;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        allCards = commonCards.Concat(rareCards).ToList();
    }
}
