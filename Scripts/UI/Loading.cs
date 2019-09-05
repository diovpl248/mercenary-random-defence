using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public enum SceneState
    {
        Tutorial,
        NewGame,
        Continue,
        InfiniteNewGame,
        InfiniteContinue,
        Main,
    }

    public GameObject loadingPopup;
    public Slider loadingBar;

    private static Loading instance;
    public static Loading Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Loading>();
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }   
    }

    public void GameSceneLoadWithSaveFile()
    {
        StartCoroutine(LoadAsyncCoroutine("GameScene", SceneState.Continue));
    }

    public void MainScene()
    {
        StartCoroutine(LoadAsyncCoroutine("MenuScene", SceneState.Main));
    }

    public void GameSceneLoadNewGame()
    {
        StartCoroutine(LoadAsyncCoroutine("GameScene", SceneState.NewGame));
    }

    public void InfGameSceneLoadWithSaveFile()
    {
        StartCoroutine(LoadAsyncCoroutine("InfGameScene", SceneState.InfiniteContinue));
    }

    public void InfGameSceneLoadNewGame()
    {
        StartCoroutine(LoadAsyncCoroutine("InfGameScene", SceneState.InfiniteNewGame));
    }

    public void GameSceneLoadTutorial()
    {
        StartCoroutine(LoadAsyncCoroutine("Tutorial", SceneState.Tutorial));
    }

    IEnumerator LoadAsyncCoroutine(string sceneName, SceneState state)
    {
        loadingPopup.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        switch(state)
        {
            case SceneState.Continue:
                SaveSystem.Instance.Load();
                break;

            case SceneState.NewGame:
                SaveSystem.Instance.NewLoad();
                break;

            case SceneState.Tutorial:
                break;

            case SceneState.InfiniteContinue:
                SaveSystemInfinity.Instance.Load();
                break;

            case SceneState.InfiniteNewGame:
                SaveSystemInfinity.Instance.NewLoad();
                break;

            case SceneState.Main:
                break;
        }

        loadingBar.value = 0f;
        loadingPopup.SetActive(false);
    }
}
