using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    [SerializeField] private int starterHealth;
    [SerializeField] private int starterMana;
    [SerializeField] private int starterGold;
    [SerializeField] private GameObject confirmationPopup;

    public void StartNewGame(bool confirm = false)
    {
        if (confirm || !File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {

            PlayerData.health = PlayerData.maxHealth = starterHealth;
            PlayerData.mana = PlayerData.maxMana = starterMana;
            PlayerData.gold = starterGold;
            SaveSystem.Save();

            SceneChanger.previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("SampleCombatScene");
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
