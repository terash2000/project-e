using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class NewGame : MonoBehaviour
{
    [SerializeField] private int starterHealth;
    [SerializeField] private int starterMana;
    [SerializeField] private int starterGold;
    [SerializeField] private GameObject confirmationPopup;
    [SerializeField] private DialogNode prologue;
    [SerializeField] private Wave tutorialWave;
    private bool skipPrologue = true;

    public void StartNewGame(bool confirm = false)
    {
        if (confirm || !File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            MakeNewPlayerData();
            if (!skipPrologue)
            {
                DialogManager.nextRoot.Push(prologue);
                SceneChanger.LoadScene("StoryScene");
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

    public void TogSkipPrologue(bool tog)
    {
        skipPrologue = tog;
    }

    private void MakeNewPlayerData()
    {
        PlayerData.Health = PlayerData.MaxHealth = starterHealth;
        PlayerData.Mana = PlayerData.MaxMana = starterMana;
        PlayerData.Gold = starterGold;
        PlayerData.seedJSON = JsonUtility.ToJson(Random.state);
        PlayerData.path = null;
    }
}
