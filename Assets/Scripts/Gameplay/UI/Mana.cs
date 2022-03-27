using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mana : MonoBehaviour, ITurnHandler
{
    private TextMeshProUGUI ManaText;
    public void onEndTurn()
    {

    }

    public void onStartTurn()
    {
        PlayerData.Mana = PlayerData.MaxMana;
    }

    // Start is called before the first frame update
    void Start()
    {
        ManaText = gameObject.transform.Find("Mana Text").gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        ManaText.text = PlayerData.Mana.ToString() + '/' + PlayerData.MaxMana.ToString();
    }
}
