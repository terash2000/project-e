using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviourSingletonPersistent<SceneChanger>
{
    private Stack<string> _nextSceneStack = new Stack<string>();
    private string _previousScene = "MainMenuScene";
    private AsyncOperation _sceneLoading;
    public Stack<string> NextSceneStack
    {
        get { return _nextSceneStack; }
        set { _nextSceneStack = value; }
    }

    public AsyncOperation SceneLoading
    {
        get { return _sceneLoading; }
        set { _sceneLoading = value; }
    }

    public void LoadScene(string scenename)
    {
        Time.timeScale = 1f;
        _previousScene = SceneManager.GetActiveScene().name;
        if (scenename == "MainMenuScene")
        {
            _nextSceneStack.Clear();
            DialogManager.NextRoot.Clear();
        }
        if (NeedLoadScene(scenename))
        {
            SceneManager.LoadSceneAsync("LoadingScene");
            _sceneLoading = SceneManager.LoadSceneAsync(scenename);
            StartCoroutine(GetSceneLoadProgress());
        }
        else SceneManager.LoadScene(scenename);
    }

    public bool NeedLoadScene(string scenename)
    {
        return scenename == "CombatScene";
    }

    public IEnumerator GetSceneLoadProgress()
    {
        while (!_sceneLoading.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    public void NextScene()
    {
        if (NextSceneStack.Count > 0)
        {
            LoadScene(NextSceneStack.Peek());
            NextSceneStack.Pop();
        }
        else LoadScene("MapScene");
    }

    public void PreviousScene()
    {
        LoadScene(_previousScene);
    }

    public void ExitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
