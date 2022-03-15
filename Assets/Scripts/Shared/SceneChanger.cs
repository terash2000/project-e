using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string scenename) {  
        PlayerData.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);  
    }

    public void PreviousScene(string scenename) {  
        SceneManager.LoadScene(PlayerData.previousScene);  
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }


}
