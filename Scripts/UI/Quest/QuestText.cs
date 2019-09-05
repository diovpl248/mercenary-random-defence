using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestText : MonoBehaviour
{
    Animator animator;
    Text[] text;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        text = GetComponentsInChildren<Text>();
    }

    public void ShowQuestText(string name, int reward){
        text[0].text = name + " 퀘스트 완료";
        text[1].text = reward.ToString();
        animator.SetTrigger("ShowText");
    }
}
