using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static string previousScene = "MainMenuScene";
    public static List<string> nextScene = new List<string>();

    public static void LoadScene(string scenename)
    {
        Time.timeScale = 1f;
        SceneChanger.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);
    }

    public static void NextScene()
    {
        if (nextScene.Count > 0)
        {
            LoadScene(nextScene[0]);
            nextScene.RemoveAt(0);
        }
        else LoadScene("MapScene");
    }

    public static void PreviousScene()
    {
        LoadScene(SceneChanger.previousScene);
    }



    public static void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }


}
