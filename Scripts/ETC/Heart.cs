using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    // 싱글톤
    private static Heart instance = null;
    public static Heart Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Heart>();
            }
            return instance;
        }
    }

    Health health;
    public Character character;

    private void Awake()
    {
        health = GetComponent<Health>();
        character = GetComponent<Character>();
        UnitPool.Instance.unitTotalList.Add(GetComponent<Character>());
        StartCoroutine(gameOverCheckCoroutine());
    }

    IEnumerator gameOverCheckCoroutine()
    {
        while (true)
        {
            if(character.characterInfo.currentHP <= 0)
            {
                GameManager.Instance.GameOver();
                break;
            }
            
            for(float time = 1f; time>=0;time-=Time.deltaTime*GameManager.Instance.GameSpeed)
            {
                yield return null;
            }
        }
    }
}
