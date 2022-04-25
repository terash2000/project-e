using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviourSingletonPersistent<SceneChanger>
{
    public AudioClip MainBGM;
    public AudioClip CombatBGM;

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

        SoundController.PlayBGM(GetSceneBGM(scenename));
    }

    public bool NeedLoadScene(string scenename)
    {
        return scenename == "CombatScene"
            || (scenename == "MapScene" && (_previousScene == "MainMenuScene" || PlayerData.Path == null))
            || scenename == "MainMenuScene";
    }

    public IEnumerator GetSceneLoadProgress()
    {
        while (!_sceneLoading.isDone)
        {
            yield return null;
        }

        //SceneManager.UnloadSceneAsync("LoadingScene");
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

    private AudioClip GetSceneBGM(string scenename)
    {
        switch (scenename)
        {
            case "CombatScene":
                return CombatBGM;
            default:
                return MainBGM;
        }
    }
}
