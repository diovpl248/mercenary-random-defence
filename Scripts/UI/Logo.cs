using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
        animator.SetTrigger("LogoStart");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
