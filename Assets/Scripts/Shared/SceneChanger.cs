using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static string previousScene = "MainMenuScene";
    public static Stack<string> nextScene = new Stack<string>();

    public static void LoadScene(string scenename)
    {
        Time.timeScale = 1f;
        SceneChanger.previousScene = SceneManager.GetActiveScene().name;
        if (scenename == "MainMenuScene")
        {
            nextScene.Clear();
            DialogManager.nextRoot.Clear();
        }

        SceneManager.LoadScene(scenename);
    }

    public static void NextScene()
    {
        if (nextScene.Count > 0)
        {
            LoadScene(nextScene.Peek());
            nextScene.Pop();
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
