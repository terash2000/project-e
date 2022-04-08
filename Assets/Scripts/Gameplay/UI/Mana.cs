using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mana : MonoBehaviour, ITurnHandler
{
    private TextMeshProUGUI _manaText;
    public void OnEndTurn()
    {

    }

    public void OnStartTurn()
    {
        PlayerData.Mana = PlayerData.MaxMana;
    }

    // Start is called before the first frame update
    void Start()
    {
        _manaText = gameObject.transform.Find("Mana Text").gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _manaText.text = PlayerData.Mana.ToString() + '/' + PlayerData.MaxMana.ToString();
    }
}
