using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ArtifactManager : MonoBehaviour
{
    public List<Artifact> artifactList = new List<Artifact>();

    private Dictionary<ArtifactType, List<Artifact>> artifactDatas = new Dictionary<ArtifactType, List<Artifact>>();
    public List<Artifact> artifactTotalList = new List<Artifact>();

    private Dictionary<string, Sprite[]> imageResources = new Dictionary<string, Sprite[]>();

    private int artifactCount = 0;

    private float damagePercent = 100;
    public float DamagePercent { get { return damagePercent; } }

    private float takeDamagePercent = 100;
    public float TakeDamagePercent { get { return takeDamagePercent; } }

    private float bloodSuckingPercent = 0;
    public float BloodSuckingPercent { get { return bloodSuckingPercent; } }

    private float goldDropPercent = 100;
    public float GoldDropPercent { get { return goldDropPercent; } }

    private float totemCoolDown = 0;
    public float TotemCoolDown { get { return totemCoolDown; } }

    private float enemyDefenseDownPercent = 100;
    public float EnemyDefenseDownPercent { get { return enemyDefenseDownPercent; } }

    private float criticalPercentUP = 0;
    public float CriticalPercentUP { get { return criticalPercentUP; } }


    private static ArtifactManager instance = null;
    public static ArtifactManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ArtifactManager>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // 유물 딕셔터리 초기화부분
        foreach (var artifactType in (ArtifactType[])Enum.GetValues(typeof(ArtifactType)))
        {
            artifactDatas.Add(artifactType, new List<Artifact>());
        }

        // 이미지 로드부분
        Sprite[] sprites = Resources.LoadAll<Sprite>("Image/Artifact/artifact_icon");
        imageResources.Add("artifact_icon", sprites);

        sprites = Resources.LoadAll<Sprite>("Image/Artifact/artifact_icon2");
        imageResources.Add("artifact_icon2", sprites);

        sprites = Resources.LoadAll<Sprite>("Image/Artifact/easter_icon");
        imageResources.Add("easter_icon", sprites);

        StartCoroutine(LoadJsonDataCoroutine());
    }

    IEnumerator LoadJsonDataCoroutine()
    {
        yield return new WaitUntil(() => JsonManager.Instance.ready);

        LoadArtifactsDataFromJson(JsonManager.Instance.jsonArtifacts);
    }

    // Json으로 읽어온 유물데이터 실제로 로드하는부분
    public void LoadArtifactsDataFromJson(JsonArtifact[] jsonArtifacts)
    {
        foreach (var jsonArtifact in jsonArtifacts)
        {
            ArtifactType type = (ArtifactType)Enum.Parse(typeof(ArtifactType), jsonArtifact.type);
            ArtifactRarity rarity = (ArtifactRarity)Enum.Parse(typeof(ArtifactRarity), jsonArtifact.rarity);

            Artifact artifact = new Artifact(
                    type,
                    rarity,
                    jsonArtifact.artifact_name,
                    jsonArtifact.desc,
                    jsonArtifact.amount,
                    imageResources[jsonArtifact.sprite_name][jsonArtifact.sprite_number]
                );
            
            artifactDatas[type].Add(artifact);
            artifactTotalList.Add(artifact);
        }
    }

    public int CalcAttackDamage(int damage)
    {
        return (int)(damagePercent / 100f * damage);
    }

    public int CalcTakeDamage(int damage)
    {
        return (int)(takeDamagePercent / 100f * damage);
    }

    public float CalcBloodSucking(float percent)
    {
        return bloodSuckingPercent/100f + percent;
    }

    public int CalcGettingGold(float gold)
    {
        return (int)(goldDropPercent / 100f * gold);
    }

    public float CalcTotemCoolTime(float startCoolTime)
    {
        return startCoolTime + totemCoolDown;
    }

    public float CalcCriticalPercent(float percent)
    {
        return criticalPercentUP + percent;
    }

    public void AddArtifact(ArtifactType artifactType, string name)
    {
        Artifact addingArtifact = null;
        foreach (var artifact in artifactDatas[artifactType])
        {
            if (artifact.name == name)
            {
                addingArtifact = artifact;
                break;
            }
        }
        if (addingArtifact == null)
            return;

        switch (artifactType)
        {
            case ArtifactType.DamageUP:
                damagePercent += addingArtifact.amount;
                break;

            case ArtifactType.DamageDown:
                damagePercent -= addingArtifact.amount;
                break;

            case ArtifactType.DefenseUP:
                takeDamagePercent -= addingArtifact.amount;
                break;

            case ArtifactType.DefenseDown:
                takeDamagePercent += addingArtifact.amount;
                break;

            case ArtifactType.BloodSucking:
                bloodSuckingPercent += addingArtifact.amount;
                break;

            case ArtifactType.GoldUP:
                goldDropPercent += addingArtifact.amount;
                break;

            case ArtifactType.TotemCoolDown:
                totemCoolDown += addingArtifact.amount;
                break;

            case ArtifactType.CriticalPercentUP:
                goldDropPercent += addingArtifact.amount;
                break;
        }

        if (totemCoolDown >= 20)
            totemCoolDown = 20;

        artifactList.Add(addingArtifact);

        QuestEventManager.Instance.ReceiveEvent(null, QuestEvent.Artifact, 1);
    }

    public void DeleteArtifact(string name)
    {
        int artifactTypeLength = Enum.GetValues(typeof(ArtifactType)).Length;

        Artifact deletingArtifact = null;
        foreach (var artifact in artifactList)
        {
            if (artifact.name == name)
            {
                deletingArtifact = artifact;
                break;
            }
        }

        if (deletingArtifact == null)
            return;

        switch (deletingArtifact.type)
        {
            case ArtifactType.DamageUP:
                damagePercent -= deletingArtifact.amount;
                break;

            case ArtifactType.DamageDown:
                damagePercent += deletingArtifact.amount;
                break;

            case ArtifactType.DefenseUP:
                takeDamagePercent += deletingArtifact.amount;
                break;

            case ArtifactType.DefenseDown:
                takeDamagePercent -= deletingArtifact.amount;
                break;

            case ArtifactType.BloodSucking:
                bloodSuckingPercent -= deletingArtifact.amount;
                break;

            case ArtifactType.GoldUP:
                goldDropPercent -= deletingArtifact.amount;
                break;

            case ArtifactType.TotemCoolDown:
                totemCoolDown -= deletingArtifact.amount;
                break;

            case ArtifactType.CriticalPercentUP:
                goldDropPercent += deletingArtifact.amount;
                break;
        }

        artifactList.Remove(deletingArtifact);
    }

    public bool FindArtifact(string name)
    {
        foreach(var artifact in artifactList)
        {
            if (artifact.name == name)
                return true;
        }
        return false;
    }

    // 임시로 랜덤으로 유물 획득
    //public void GetRandomArtifact()
    //{
    //    switch(UnityEngine.Random.Range(0,6))
    //    {
    //        case 0:
    //            AddArtifact(ArtifactType.DamageUP, "은빛 단검");
    //            break;
    //        case 1:
    //            AddArtifact(ArtifactType.DefenseUP, "금빛 방패");
    //            break;
    //        case 2:
    //            AddArtifact(ArtifactType.GoldUP, "에메랄드 보석");
    //            break;
    //        case 3:
    //            AddArtifact(ArtifactType.BloodSucking, "흡혈 목걸이");
    //            break;
    //        case 4:
    //            AddArtifact(ArtifactType.TotemCoolDown, "주문서");
    //            break;
    //        case 5:
    //            AddArtifact(ArtifactType.CriticalPercentUP, "나무 반지");
    //            break;
    //    }
    //}

    public Artifact GetRandomArtifact()
    {
        int index = UnityEngine.Random.Range(0, artifactTotalList.Count);
        return artifactTotalList[index];
    }

    public Artifact[] GetRandom3Artifact()
    {
        Artifact[] newArtifacts = new Artifact[3];
        int[] selectedNum = new int[3];

        for (int i = 0; i < 3; i++)
        {
            bool isSame = true;
            while (isSame)
            {
                isSame = false;
                selectedNum[i] = UnityEngine.Random.Range(0, artifactTotalList.Count);
                for (int j = 0; j < i; j++)
                {
                    if (selectedNum[i] == selectedNum[j])
                    {
                        isSame = true;
                        break;
                    }
                }
            }
            newArtifacts[i] = artifactTotalList[selectedNum[i]];
        }
        
        return newArtifacts;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GetRandomArtifact();
        }
#endif
    }
}
