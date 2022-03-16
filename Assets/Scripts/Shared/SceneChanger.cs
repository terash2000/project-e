using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static string previousScene = "MainMenuScene";

    public void LoadScene(string scenename) {  
        SceneChanger.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);  
    }

    public void PreviousScene(string scenename) {  
        SceneManager.LoadScene(SceneChanger.previousScene);  
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }


}
