using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public GameObject confirmationPopup;

    public void StartNewGame(bool confirm = false)
    {
        if (confirm || !File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            List<int> starterDeck = new List<int>();

            PlayerData.gold = 0;
            PlayerData.deck = starterDeck;
            SaveSystem.Save();

            SceneChanger.previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            GameObject newPopup = Instantiate(confirmationPopup, transform.parent);
            string message = "Are you sure you want to erase your existing run to start a new one?";
            UnityAction action = () => StartNewGame(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }
}
