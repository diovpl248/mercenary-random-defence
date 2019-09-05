using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestEvent
{   //퀘스트 참조 변수들
    MyUnitDie,
    EnemyDie,
    BossDie,
    Gacha,
    BronzeGacha,
    SilverGacha,
    GoldGacha,
    PlatinumGacha,
    DiamondGacha,
    Gold,
    Stage,
    Artifact,
    Totem,
    Combination,
    ClearVal,
    Reset,
}

// 퀘스트 관리자
public class QuestEventManager : MonoBehaviour
{
    public List<Quest> quests;

    public QuestText questText;

    public delegate void QuestFunc(QuestEvent inputEvent, int amount);
    public static event QuestFunc questFunc;

    private void Awake()
    {
        if(questFunc != null)
            questFunc(QuestEvent.Reset, 0);
        AddQuest();
        QuestInfoView.Instance.UpdateQuestList(quests);
    }

    private static QuestEventManager instance = null;
    public static QuestEventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestEventManager>();
            }
            return instance;
        }
    }

    //외부에서 퀘스트 변수 호출
    public void ReceiveEvent(GameObject gameObject, QuestEvent questEvent, int amount)
    {
        questFunc(questEvent, amount);
    }

    public void QuestPrint(string name, int reward)
    {
        questText.ShowQuestText(name, reward);
    }

    public void AddQuest()
    {
        //적유닛 퀘스트
        Quest enemyDie1 = new Quest(QuestEvent.EnemyDie, "방금 그 빨간게 바로 적입니다", "적을 1번 처치합니다", 1, 100);
        quests.Add(enemyDie1);
        QuestInfoView.Instance.MakeQuestInfoContent(enemyDie1);

        Quest enemyDie50 = new Quest(QuestEvent.EnemyDie, "빨간친구들과 신나는 시간", "적을 50번 처치합니다", 50, 500);
        quests.Add(enemyDie50);
        QuestInfoView.Instance.MakeQuestInfoContent(enemyDie50);

        Quest enemyDie100 = new Quest(QuestEvent.EnemyDie, "이제 좀 몸이 풀린것 같아", "적을 100번 처치합니다", 100, 1000);
        quests.Add(enemyDie100);
        QuestInfoView.Instance.MakeQuestInfoContent(enemyDie100);

        Quest enemyDie500 = new Quest(QuestEvent.EnemyDie, "빨간친구들이 우리를 싫어합니다", "적을 500번 처치합니다", 500, 3000);
        quests.Add(enemyDie500);
        QuestInfoView.Instance.MakeQuestInfoContent(enemyDie500);

        Quest enemyDie1000 = new Quest(QuestEvent.EnemyDie, "빨간친구들이 우리를 무서워합니다", "적을 1000번 처지합니다", 1000, 5000);
        quests.Add(enemyDie1000);
        QuestInfoView.Instance.MakeQuestInfoContent(enemyDie1000);

        Quest enemyDie5000 = new Quest(QuestEvent.EnemyDie, "주객 전도", "적을 5000번 처치합니다", 5000, 10000);
        quests.Add(enemyDie5000);
        QuestInfoView.Instance.MakeQuestInfoContent(enemyDie5000);

        //보스 퀘스트
        Quest bossDie1 = new Quest(QuestEvent.BossDie, "방금 그게 보스입니다", "보스를 처음 처치합니다", 1, 500);
        quests.Add(bossDie1);
        QuestInfoView.Instance.MakeQuestInfoContent(bossDie1);

        Quest bossDie2 = new Quest(QuestEvent.BossDie, "처음이 어렵지 두번은 쉬워", "보스를 두번 처치합니다", 2, 600);
        quests.Add(bossDie2);
        QuestInfoView.Instance.MakeQuestInfoContent(bossDie2);

        Quest bossDie5 = new Quest(QuestEvent.BossDie, "그리핀 사냥꾼", "보스를 5번 처치합니다", 5, 1500);
        quests.Add(bossDie5);
        QuestInfoView.Instance.MakeQuestInfoContent(bossDie5);

        Quest bossDie10 = new Quest(QuestEvent.BossDie, "거대 생물 전문가", "보스를 10번 처치합니다", 10, 7000);
        quests.Add(bossDie10);
        QuestInfoView.Instance.MakeQuestInfoContent(bossDie10);

        Quest bossDie15 = new Quest(QuestEvent.BossDie, "괴물의 대가", "보스를 15번 처치합니다", 15, 10000);
        quests.Add(bossDie15);
        QuestInfoView.Instance.MakeQuestInfoContent(bossDie15);

        Quest bossDie19 = new Quest(QuestEvent.BossDie, "거대 생물 멸종 위기", "보스를 19번 처치합니다", 19, 15000);
        quests.Add(bossDie19);
        QuestInfoView.Instance.MakeQuestInfoContent(bossDie19);

        //가챠 퀘스트
        Quest gacha1 = new Quest(QuestEvent.Gacha, "이게 바로 뽑기 입니다", "가챠를 1번 돌린다 (등급 상관없음)", 1, 100);
        quests.Add(gacha1);
        QuestInfoView.Instance.MakeQuestInfoContent(gacha1);

        Quest gacha10 = new Quest(QuestEvent.Gacha, "문방구 뽑기좀 해봤구나?", "가챠를 10번 돌린다 (등급 상관없음)", 10, 1000);
        quests.Add(gacha10);
        QuestInfoView.Instance.MakeQuestInfoContent(gacha10);

        Quest gacha50 = new Quest(QuestEvent.Gacha, "뽑기 중독자", "가챠를 50번 돌린다 (등급 상관없음)", 50, 5000);
        quests.Add(gacha50);
        QuestInfoView.Instance.MakeQuestInfoContent(gacha50);

        Quest gacha100 = new Quest(QuestEvent.Gacha, "뽑기왕", "가챠를 100번 돌린다 (등급 상관없음)", 100, 10000);
        quests.Add(gacha100);
        QuestInfoView.Instance.MakeQuestInfoContent(gacha100);

        Quest bronzeGacha10 = new Quest(QuestEvent.BronzeGacha, "깡통차기", "브론즈 가차를 10번 돌린다", 10, 500);
        quests.Add(bronzeGacha10);
        QuestInfoView.Instance.MakeQuestInfoContent(bronzeGacha10);

        Quest bronzeGacha50 = new Quest(QuestEvent.BronzeGacha, "동수저", "브론즈 가차를 30번 돌린다", 30, 2500);
        quests.Add(bronzeGacha50);
        QuestInfoView.Instance.MakeQuestInfoContent(bronzeGacha50);

        Quest silverGacha10 = new Quest(QuestEvent.SilverGacha, "은수집가", "실버 가차를 10번 돌린다", 10, 1000);
        quests.Add(silverGacha10);
        QuestInfoView.Instance.MakeQuestInfoContent(silverGacha10);

        Quest silverGacha50 = new Quest(QuestEvent.SilverGacha, "은수저", "실버 가차를 30번 돌린다", 30, 5000);
        quests.Add(silverGacha50);
        QuestInfoView.Instance.MakeQuestInfoContent(silverGacha50);

        Quest goldGacha10 = new Quest(QuestEvent.GoldGacha, "금수집가", "골드 가차를 10번 돌린다", 10, 5000);
        quests.Add(goldGacha10);
        QuestInfoView.Instance.MakeQuestInfoContent(goldGacha10);

        Quest goldGacha50 = new Quest(QuestEvent.GoldGacha, "금수저", "골드 가차를 30번 돌린다", 30, 25000);
        quests.Add(goldGacha50);
        QuestInfoView.Instance.MakeQuestInfoContent(goldGacha50);

        Quest platinumGacha10 = new Quest(QuestEvent.PlatinumGacha, "백금수집가", "플래티넘 가차를 10번 돌린다", 10, 25000);
        quests.Add(platinumGacha10);
        QuestInfoView.Instance.MakeQuestInfoContent(platinumGacha10);

        Quest platinumGacha50 = new Quest(QuestEvent.PlatinumGacha, "플레티넘수저", "플레티넘 가차를 30번 돌린다", 30, 125000);
        quests.Add(platinumGacha50);
        QuestInfoView.Instance.MakeQuestInfoContent(platinumGacha50);

        Quest diamondGacha10 = new Quest(QuestEvent.DiamondGacha, "다이아 수집가", "다이아 가챠를 10번 돌린다", 10, 125000);
        quests.Add(diamondGacha10);
        QuestInfoView.Instance.MakeQuestInfoContent(diamondGacha10);

        Quest diamondGacha50 = new Quest(QuestEvent.DiamondGacha, "다이아수저", "다이아 가챠를 30번 돌린다", 30, 626000);
        quests.Add(diamondGacha50);
        QuestInfoView.Instance.MakeQuestInfoContent(diamondGacha50);

        //조합
        Quest combi1 = new Quest(QuestEvent.Combination, "이게 바로 조합입니다", "조합을 1회 성공한다",1, 500);
        quests.Add(combi1);
        QuestInfoView.Instance.MakeQuestInfoContent(combi1);

        Quest combi5 = new Quest(QuestEvent.Combination, "프랑켄슈타인", "조합을 5회 성공한다", 5, 2000);
        quests.Add(combi5);
        QuestInfoView.Instance.MakeQuestInfoContent(combi5);

        Quest combi10 = new Quest(QuestEvent.Combination, "끼워맞춘 블럭", "조합을 10회 성공한다", 10, 4000);
        quests.Add(combi10);
        QuestInfoView.Instance.MakeQuestInfoContent(combi10);

        //스테이지 퀘스트
        Quest stage5 = new Quest(QuestEvent.Stage, "걸음마", "5스테이지에 도달한다", 5, 500);
        quests.Add(stage5);
        QuestInfoView.Instance.MakeQuestInfoContent(stage5);

        Quest stage10 = new Quest(QuestEvent.Stage, "이제 조금 알것 같아","10스테이지에 도달한다",10,1000);
        quests.Add(stage10);
        QuestInfoView.Instance.MakeQuestInfoContent(stage10);

        Quest stage20 = new Quest(QuestEvent.Stage,"이제부터 항복이 가능합니다", "20스테이지에 도달한다", 20, 2000);
        quests.Add(stage20);
        QuestInfoView.Instance.MakeQuestInfoContent(stage20);

        Quest stage30 = new Quest(QuestEvent.Stage, "저들의 정체는 도대체 뭐길래..", "30스테이지에 도달한다", 30, 4000);
        quests.Add(stage30);
        QuestInfoView.Instance.MakeQuestInfoContent(stage30);

        Quest stage40 = new Quest(QuestEvent.Stage, "도대체 얼마나 잡아야하는거야", "40스테이지에 도달한다", 40, 5000);
        quests.Add(stage40);
        QuestInfoView.Instance.MakeQuestInfoContent(stage40);

        Quest stage50 = new Quest(QuestEvent.Stage, "반환점", "50스테이지에 도달한다", 50, 7000);
        quests.Add(stage50);
        QuestInfoView.Instance.MakeQuestInfoContent(stage50);

        Quest stage60 = new Quest(QuestEvent.Stage, "아직도 많이 남았어요", "60스테이지에 도달한다", 60, 9000);
        quests.Add(stage60);
        QuestInfoView.Instance.MakeQuestInfoContent(stage60);

        Quest stage70 = new Quest(QuestEvent.Stage, "끝이 보이긴 할까요?", "70스테이지에 도달한다", 70, 11000);
        quests.Add(stage70);
        QuestInfoView.Instance.MakeQuestInfoContent(stage70);

        Quest stage80 = new Quest(QuestEvent.Stage, "80계단", "80스테이지에 도달한다", 80, 15000);
        quests.Add(stage80);
        QuestInfoView.Instance.MakeQuestInfoContent(stage80);

        Quest stage90 = new Quest(QuestEvent.Stage, "고지가 보인다", "90스테이지에 도달한다", 90, 17000);
        quests.Add(stage90);
        QuestInfoView.Instance.MakeQuestInfoContent(stage90);

        Quest stage100 = new Quest(QuestEvent.Stage, "정상에 오르다", "100스테이지에 도달한다", 100, 20000);
        quests.Add(stage100);
        QuestInfoView.Instance.MakeQuestInfoContent(stage100);

        //유물 5스테이지마다 하나씩생김
        Quest artifact5 = new Quest(QuestEvent.Artifact, "고대의 유물", "유물을 5개 수집한다", 5, 1500);
        quests.Add(artifact5);
        QuestInfoView.Instance.MakeQuestInfoContent(artifact5);

        Quest artifact10 = new Quest(QuestEvent.Artifact, "유물 수집가", "유물을 10개 수집한다", 10, 3000);
        quests.Add(artifact10);
        QuestInfoView.Instance.MakeQuestInfoContent(artifact10);

        //토템
        Quest totem1 = new Quest(QuestEvent.Totem, "이게 바로 토템입니다", "토템을 1회 사용한다", 1, 100);
        quests.Add(totem1);
        QuestInfoView.Instance.MakeQuestInfoContent(totem1);

        Quest totem5 = new Quest(QuestEvent.Totem, "돗자리 깔기", "토템을 5회 사용한다", 5, 500);
        quests.Add(totem5);
        QuestInfoView.Instance.MakeQuestInfoContent(totem5);

        Quest totem10 = new Quest(QuestEvent.Totem, "장판 시공 전문가", "토템을 10회 사용한다", 10, 1000);
        quests.Add(totem10);
        QuestInfoView.Instance.MakeQuestInfoContent(totem10);

        Quest totem50 = new Quest(QuestEvent.Totem, "깔아서 나쁠건 없잖아?", "토템을 30회 사용한다", 30, 5000);
        quests.Add(totem50);
        QuestInfoView.Instance.MakeQuestInfoContent(totem50);

        //아군유닛 퀘스트

        Quest myUnitDie1 = new Quest(QuestEvent.MyUnitDie, "방금 아군이 죽었어요!", "1명의 아군 유닛이 죽습니다", 1, 100);
        quests.Add(myUnitDie1);
        QuestInfoView.Instance.MakeQuestInfoContent(myUnitDie1);

        Quest myUnitDie10 = new Quest(QuestEvent.MyUnitDie, "작은 희생", "10명의 아군 유닛이 죽습니다", 10, 1000);
        quests.Add(myUnitDie10);
        QuestInfoView.Instance.MakeQuestInfoContent(myUnitDie10);

        Quest myUnitDie50 = new Quest(QuestEvent.MyUnitDie, "그들이 있었기에 우리는 살았다", "50명의 아군 유닛이 죽습니다", 50, 5000);
        quests.Add(myUnitDie50);
        QuestInfoView.Instance.MakeQuestInfoContent(myUnitDie50);

        Quest myUnitDie100 = new Quest(QuestEvent.MyUnitDie, "국가유공자", "100명의 아군 유닛이 죽습니다", 100, 10000);
        quests.Add(myUnitDie100);
        QuestInfoView.Instance.MakeQuestInfoContent(myUnitDie100);


    }
}

[System.Serializable]
public class Quest
{
    public string questName;
    public string questInfo;
    public int count;
    public long targetCount;
    public int reward;
    public QuestEvent questEvent;
    public Quest quest;


    public Quest(QuestEvent inputEvent, string name, string desc, long inputCount, int gold)
    {
        this.questEvent = inputEvent;
        questName = name;
        questInfo = desc;
        count = 0;
        reward = gold;
        this.targetCount = inputCount;
        QuestEventManager.questFunc += MyQuestFunc;
    }

    public void MyQuestFunc(QuestEvent inputEvent, int amount)
    {
        if(inputEvent == QuestEvent.Reset)
        {
            QuestEventManager.questFunc -= MyQuestFunc;
            return;
        }

        if (this.questEvent != inputEvent) return;
        count += amount;
        if (this.questEvent == QuestEvent.Gold) count = amount;
        if (count >= targetCount)
        {
            QuestEventManager.Instance.QuestPrint(questName, reward);
            Database.Instance.gold = Database.Instance.gold + reward;
            QuestEventManager.questFunc -= MyQuestFunc;
        }
    }

    public void LoadCount(int count)
    {
        this.count = count;
        if(count >= targetCount)
        {
            QuestEventManager.questFunc -= MyQuestFunc;
        }
    }
}




