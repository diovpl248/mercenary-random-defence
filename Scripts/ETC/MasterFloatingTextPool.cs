using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MasterFloatingTextPool : MonoBehaviour
{
    public GameObject floatingBasis;

    Stack<TextMeshPro> pool = new Stack<TextMeshPro>();

    private static MasterFloatingTextPool instance = null;
    public static MasterFloatingTextPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MasterFloatingTextPool>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 데미지 10개정도 미리 초기화
        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(floatingBasis);
            TextMeshPro tmp = go.GetComponent<TextMeshPro>();
            go.SetActive(false);
            pool.Push(tmp);
        }
    }

    public TextMeshPro GetFromPool()
    {
        if (pool.Count <= 0)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject go = Instantiate(floatingBasis);
                TextMeshPro tmp = go.GetComponent<TextMeshPro>();
                go.SetActive(false);
                pool.Push(tmp);
            }
        }

        return pool.Pop();
    }

    public void ReturnToPool(TextMeshPro tmp)
    {
        tmp.gameObject.SetActive(false);
        pool.Push(tmp);
    }
    
}
