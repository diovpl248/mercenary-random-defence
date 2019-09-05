using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public GameObject settingUI;

    public void ReStart()
    {
        if (GameManager.Instance.infiniteMode)
            Loading.Instance.InfGameSceneLoadNewGame();
        else
            Loading.Instance.GameSceneLoadNewGame();
    }

    public void ReturnMainScene()
    {
        if(!GameManager.Instance.isGameOver)
        {
            if (GameManager.Instance.infiniteMode)
            {
                SaveSystemInfinity.Instance.Save();
            }
            else
            {
                SaveSystem.Instance.Save();
            }
        }

        Loading.Instance.MainScene();
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(settingUI.activeInHierarchy)
            {
                settingUI.SetActive(false);
                GameManager.Instance.MenuButtonPrevPause();
            }
            else if(!GameManager.Instance.menuPopup)
            {
                settingUI.SetActive(true);
                GameManager.Instance.MenuButtomPause();
            }
        }   
    }
}
