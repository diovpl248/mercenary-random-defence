using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool infiniteMode = false;

    public bool menuPopup = false;
    private bool pause = false;
    bool prevPause;
    public bool isGameOver = false;

    public GameObject gameOver;

    public GameObject gameClear;

    int gettingFragment;

    public Text fragmentText;

    [SerializeField]
    private float gameSpeed = 1.0f;
    public float GameSpeed
    {
        set
        {
            gameSpeed = value;
        }
        get
        {
            if (pause)
                return 0f;

            return gameSpeed;
        }
    }

    public Text text;

    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public float NextGameSpeed()
    {
        if (gameSpeed >= 3f)
            //if (gameSpeed >= 3f)
            gameSpeed = 1f;
        else
            gameSpeed += 1f;

        return gameSpeed;
    }

    public void Pause()
    {
        pause = !pause;
    }

    public void MenuButtomPause()
    {
        prevPause = pause;
        pause = true;
        menuPopup = true;
    }

    public void MenuButtonPrevPause()
    {
        pause = prevPause;
        menuPopup = false;
    }

    public void GameOver()
    {
        string path = Application.persistentDataPath;

        if (infiniteMode)
            path += "/MRDsaveInf.bin";
        else
            path += "/MRDsave.bin";

        System.IO.File.Delete(path);
        gameOver.SetActive(true);
        isGameOver = true;

        if (infiniteMode)
        {
            gettingFragment = (int)(Mathf.Pow(100, 1.0f + 0.007f * InfiniteSpawnManager.Instance.stage));
        }
        else
        {
            gettingFragment = (int)(Mathf.Pow(100, 1.0f + 0.01f * EnemySpawnManager.Instance.stage));
        }

        AddFragment(gettingFragment);
    }

    public void GameClear()
    {
        string path = Application.persistentDataPath + "/MRDsave.bin";
        System.IO.File.Delete(path);
        isGameOver = true;

        gameClear.SetActive(true);

        gettingFragment = 20000;

        AddFragment(gettingFragment);
    }

    private void Awake()
    {
        // 안드로이드 화면 안꺼지게 설정
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start()
    {
        if (SaveSystem.Instance == null)
            infiniteMode = true;

        // 메인메뉴에서 실행안할경우 초기로드로 설정
        if (FindObjectOfType<Loading>() == null)
        {
            if(infiniteMode)
            {
                SaveSystemInfinity.Instance.NewLoad();
            }
            else
            {
                SaveSystem.Instance.NewLoad();
            }
        }
            
    }

    public void AddFragment(int n)
    {
        fragmentText.text = "<size=150><b>" + gettingFragment + "</b></size>\n조각을 획득했습니다.";

        if (!PlayerPrefs.HasKey("fragment"))
        {
            PlayerPrefs.SetInt("fragment",n);
        }
        else
        {
            int fragment = PlayerPrefs.GetInt("fragment");
            PlayerPrefs.SetInt("fragment", fragment + n);
        }
    }

    void Update()
    {
        text.text = Database.Instance.gold.ToString();
#if UNITY_EDITOR
        // 치트
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Database.Instance.gold += 1000000;
        }

        if(Input.GetKeyDown(KeyCode.F3))
        {
            Gacha.Instance.CustomGacha();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            SaveSystem.Instance.Save();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.Instance.Load();
        }

        if(Input.GetKeyDown(KeyCode.F6))
        {
            PlayerPrefs.DeleteAll();
        }

        if(Input.GetKeyDown(KeyCode.F7))
        {
            EnemySpawnManager.Instance.stage++;
        }
#endif
    }
    
}
