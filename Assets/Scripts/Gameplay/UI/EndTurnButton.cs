using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour, ITurnHandler
{
    public void onEndTurn()
    {
        GetComponent<Button>().interactable = false;
    }

    public void onStartTurn()
    {
        GetComponent<Button>().interactable = true;
    }
}
