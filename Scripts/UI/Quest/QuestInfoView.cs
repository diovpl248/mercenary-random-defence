using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoView : MonoBehaviour
{
    public GameObject questInfoPrefab;
    public QuestEventManager questEventManager;


    public Dictionary<Quest, GameObject> questViewList = new Dictionary<Quest, GameObject>();
    public Text[] textList;
    public Image[] images;
    public List<Quest> quests;
    int count;

    private static QuestInfoView instance;
    public static QuestInfoView Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestInfoView>();
            }
            return instance;
        }
    }

    void Start()
    {
        StartCoroutine(UpdateCoroutine());
    }


    public void MakeQuestInfoContent(Quest quest)
    {
        //var q = quest;
        GameObject go = Instantiate(questInfoPrefab, transform);
        textList = go.GetComponentsInChildren<Text>();
        images = go.GetComponentsInChildren<Image>();
        textList[0].text = quest.questName.ToString();
        textList[1].text = quest.questInfo.ToString();
        textList[2].text = quest.count.ToString() + " / " + quest.targetCount.ToString();
        textList[3].text = "미완료";
        if (quest.count >= quest.targetCount)
        {
            textList[3].text = "완료";
            images[2].color = new Color(233/255,183/255,29/255);
        }
        questViewList.Add(quest, go);
    }

    IEnumerator UpdateCoroutine()
    {
        
        while (true)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                Quest quest = quests[i];
                GameObject go = questViewList[quest];
                textList = go.GetComponentsInChildren<Text>();
                images = go.GetComponentsInChildren<Image>();
                textList[2].text = quest.count.ToString() + " / " + quest.targetCount.ToString();
                textList[3].text = "미완료";
                if (quest.count >= quest.targetCount)
                {
                    textList[3].text = "완료";
                    images[2].color = new Color32(243,183,29,255);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void UpdateQuestList(List<Quest> list)
    {
        quests = list;
    }
}
