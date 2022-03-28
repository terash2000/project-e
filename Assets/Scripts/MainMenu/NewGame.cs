using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class NewGame : MonoBehaviour
{
    [SerializeField] private int starterHealth;
    [SerializeField] private int starterMana;
    [SerializeField] private int starterGold;
    [SerializeField] private GameObject confirmationPopup;
    [SerializeField] private List<string> tutorialSceneOrder;
    [SerializeField] private List<DialogNode> tutorial;
    [SerializeField] private Wave tutorialWave;
    private bool skipTutorial = true;

    public void StartNewGame(bool confirm = false)
    {
        if (confirm || !File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            MakeNewPlayerData();
            if (!skipTutorial)
            {
                StartTutorial();
            }
            else
            {
                MonsterManager.wave = tutorialWave;
                SceneChanger.LoadScene("CombatScene");
            }

            SaveSystem.DeleteSave();
        }
        else
        {
            GameObject newPopup = Instantiate(confirmationPopup, transform.parent);
            string message = "Are you sure you want to erase your existing run to start a new one?";
            UnityAction action = () => StartNewGame(true);
            newPopup.GetComponent<ConfirmationPopup>().Init(message, action);
        }
    }

    public void TogSkipTutorial(bool tog)
    {
        skipTutorial = tog;
    }

    private void MakeNewPlayerData()
    {
        PlayerData.Health = PlayerData.MaxHealth = starterHealth;
        PlayerData.Mana = PlayerData.MaxMana = starterMana;
        PlayerData.Gold = starterGold;
        PlayerData.seedJSON = JsonUtility.ToJson(Random.state);
    }

    private void StartTutorial()
    {
        SceneChanger.nextScene = tutorialSceneOrder;
        DialogManager.nextRoot = tutorial;
        MonsterManager.wave = tutorialWave;
        SceneChanger.NextScene();
    }
}
