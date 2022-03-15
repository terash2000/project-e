using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class Continue : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().interactable = 
            File.Exists(Application.persistentDataPath + "/SaveData.dat");
    }

    public void ContinueGame()
    {
        SaveSystem.Load();
        PlayerData.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("MapScene");  
    }
}
