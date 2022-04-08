using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class NewGame : MonoBehaviour
{
    [SerializeField] private int _starterHealth;
    [SerializeField] private int _starterMana;
    [SerializeField] private int _starterGold;
    [SerializeField] private GameObject _confirmationPopup;
    [SerializeField] private DialogNode _prologue;
    [SerializeField] private Wave _tutorialWave;
    private bool skipPrologue = true;

    public void StartNewGame(bool confirm = false)
    {
        if (confirm || !File.Exists(Application.persistentDataPath + "/SaveData.dat"))
        {
            MakeNewPlayerData();
            if (!skipPrologue)
            {
                DialogManager.NextRoot.Push(_prologue);
                SceneChanger.LoadScene("StoryScene");
            }
            else
            {
                MonsterManager.Wave = _tutorialWave;
                SceneChanger.LoadScene("CombatScene");
            }

            SaveSystem.DeleteSave();
        }
        else
        {
            GameObject newPopup = Instantiate(_confirmationPopup, transform.parent);
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
        PlayerData.Health = PlayerData.MaxHealth = _starterHealth;
        PlayerData.Mana = PlayerData.MaxMana = _starterMana;
        PlayerData.Gold = _starterGold;
        PlayerData.SeedJSON = JsonUtility.ToJson(Random.state);
        PlayerData.Path = null;
    }
}
