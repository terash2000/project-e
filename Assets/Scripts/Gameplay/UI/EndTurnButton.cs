using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour, ITurnHandler
{
    public void OnEndTurn()
    {
        GetComponent<Button>().interactable = false;
    }

    public void OnStartTurn()
    {
        GetComponent<Button>().interactable = true;
    }
}
