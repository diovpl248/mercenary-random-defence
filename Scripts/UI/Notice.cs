using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour
{
    public Animator animator;
    public Text text;

    // 싱글톤
    private static Notice instance = null;
    public static Notice Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Notice>();
            }
            return instance;
        }
    }

    public void ShowNotice(string noticeText)
    {
        text.text = noticeText;
        animator.SetTrigger("Notice");
    }
}
