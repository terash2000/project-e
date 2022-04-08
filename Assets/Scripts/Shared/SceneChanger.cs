using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private static Stack<string> _nextSceneStack = new Stack<string>();
    private static string _previousScene = "MainMenuScene";
    public static Stack<string> NextSceneStack
    {
        get { return _nextSceneStack; }
        set { _nextSceneStack = value; }
    }

    public static void LoadScene(string scenename)
    {
        Time.timeScale = 1f;
        SceneChanger._previousScene = SceneManager.GetActiveScene().name;
        if (scenename == "MainMenuScene")
        {
            _nextSceneStack.Clear();
            DialogManager.NextRoot.Clear();
        }

        SceneManager.LoadScene(scenename);
    }

    public static void NextScene()
    {
        if (NextSceneStack.Count > 0)
        {
            LoadScene(NextSceneStack.Peek());
            NextSceneStack.Pop();
        }
        else LoadScene("MapScene");
    }

    public static void PreviousScene()
    {
        LoadScene(SceneChanger._previousScene);
    }

    public static void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
