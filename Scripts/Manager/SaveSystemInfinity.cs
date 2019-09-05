﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystemInfinity : MonoBehaviour
{
    public GameObject mainQuest;

    // 싱글톤
    private static SaveSystemInfinity instance = null;
    public static SaveSystemInfinity Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveSystemInfinity>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
    }

    public void Save()
    {
        //Time.timeScale = 0f;
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/MRDsaveInf.bin";
            FileStream stream = new FileStream(path, FileMode.Create);

            // 골드 정보
            int gold = Database.Instance.gold;

            // 유닛과 적군들 정보
            UnitData fish = new UnitData(Heart.Instance.character);

            List<UnitData> units = new List<UnitData>();
            List<UnitData> enemies = new List<UnitData>();

            foreach (var character in UnitPool.Instance.GetActiveCharacters())
            {
                if (character.characterInfo.unitName != UnitName.unit_fish && character.characterInfo.currentHP > 0)
                    units.Add(new UnitData(character));
            }

            foreach (var character in EnemyPool.Instance.GetActiveCharacters())
            {
                if (character.characterInfo.currentHP > 0)
                    enemies.Add(new UnitData(character));
            }

            // 토템배치 쿨타임 정보
            float pastTotemCoolTime = TotemManager.Instance.pastTime;

            // 배치된 토템들 정보
            List<TotemData> totems = new List<TotemData>();
            var placementedTotems = TotemManager.Instance.totems;
            foreach (var totem in placementedTotems)
            {
                if (totem.gameObject.activeInHierarchy)
                {
                    totems.Add(new TotemData(totem.totemDataset.totemName,
                        totem.totemDataset.pastingTime, totem.transform.position,
                        totem.totemDataset.buff.amount));
                }
            }

            // 유물 정보들
            List<ArtifactData> artifacts = new List<ArtifactData>();
            var havingArtifacts = ArtifactManager.Instance.artifactList;
            foreach (var artifact in havingArtifacts)
            {
                artifacts.Add(new ArtifactData(artifact.type, artifact.name));
            }

            // 인벤토리 정보
            List<InventoryData> inventoryDatas = new List<InventoryData>();
            var havingInventoryItem = Slot.Instance.viewList.Keys;
            foreach (var item in havingInventoryItem)
            {
                inventoryDatas.Add(new InventoryData(item));
            }

            // 스테이지 정보
            StageData stageData = new StageData();
            stageData.stage = InfiniteSpawnManager.Instance.stage;
            stageData.currentSpawnCount = InfiniteSpawnManager.Instance.currentSpawnCount;
            stageData.currentWaveCount = InfiniteSpawnManager.Instance.currentWaveCount;

            // 퀘스트정보
            List<QuestData> questDatas = new List<QuestData>();
            foreach (var quest in QuestEventManager.Instance.quests)
            {
                questDatas.Add(new QuestData(quest));
            }

            // 유물 선택중인지 저장
            var artifactSelectingData = new ArtifactSelectData(ArtifactSelect.Instance.isSelecting, ArtifactSelect.Instance.artifact);

            SaveFile saveFile = new SaveFile(gold, fish, units, enemies, totems, pastTotemCoolTime, artifacts, inventoryDatas, stageData, questDatas, artifactSelectingData);

            formatter.Serialize(stream, saveFile);
            stream.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void Load()
    {
        try
        {
            string path = Application.persistentDataPath + "/MRDsaveInf.bin";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                var saveFile = formatter.Deserialize(stream) as SaveFile;

                // 골드
                Database.Instance.gold = saveFile.gold;

                // 심장
                var heartCharacter = Heart.Instance.character;
                heartCharacter.characterInfo.currentHP = saveFile.fish.currentHP;

                // 아군유닛
                foreach (var item in saveFile.units)
                {
                    CharacterInfo charInfo = new CharacterInfo();

                    charInfo.characterName = item.unitName;
                    charInfo.rarity = (Rarity)item.rarity;
                    charInfo.prefix = (Prefix)item.prefix;

                    charInfo.attackSpeed = item.attackSpeed;
                    charInfo.attackDist = item.attackDist;

                    charInfo.maxHP = item.maxHP;
                    charInfo.currentHP = item.currentHP;
                    charInfo.maxMP = item.maxMP;
                    charInfo.currentMP = item.currentMP;

                    charInfo.damage = item.damage;
                    charInfo.armor = item.armor;

                    charInfo.unitName = (UnitName)item.unitNameType;

                    charInfo.printName = charInfo.characterName;

                    charInfo.criticalPercent = item.criticalPercent;
                    charInfo.criticalDamagePercent = item.criticalDamagePercent;

                    switch (charInfo.rarity)
                    {
                        case Rarity.Normal:
                            charInfo.printName = "일반 " + charInfo.printName;
                            break;

                        case Rarity.Rare:
                            charInfo.printName = "희귀 " + charInfo.printName;
                            break;

                        case Rarity.Unique:
                            charInfo.printName = "유일 " + charInfo.printName;
                            break;

                        case Rarity.Legend:
                            charInfo.printName = "전설 " + charInfo.printName;
                            break;

                        case Rarity.Epic:
                            charInfo.printName = "신화 " + charInfo.printName;
                            break;
                        default:
                            break;
                    }

                    switch (charInfo.prefix)
                    {
                        case Prefix.HighSpeed:
                            charInfo.printName = "신속의 " + charInfo.printName;
                            break;
                        case Prefix.SingleBlow:
                            charInfo.printName = "일격의 " + charInfo.printName;
                            break;
                        case Prefix.Tough:
                            charInfo.printName = "강인한 " + charInfo.printName;
                            break;
                        case Prefix.Weak:
                            charInfo.printName = "나약한 " + charInfo.printName;
                            break;
                        case Prefix.Assassin:
                            charInfo.printName = "암살의 " + charInfo.printName;
                            break;
                        case Prefix.None:
                        default:
                            break;
                    }

                    Character character = UnitPool.Instance.GetFromPool(charInfo);

                    Vector3 position;
                    position.x = item.position[0];
                    position.y = item.position[1];
                    position.z = item.position[2];

                    character.transform.position = position;

                    character.EnableCharacter();
                    //UnitPool.Instance.EnableCharacter(character);

                    // 체력설정
                    character.characterInfo.currentHP = item.currentHP;
                }

                // 적군유닛
                foreach (var item in saveFile.enemies)
                {
                    Character character = EnemyPool.Instance.GetFromPool(item.name);

                    Vector3 position;
                    position.x = item.position[0];
                    position.y = item.position[1];
                    position.z = item.position[2];

                    character.transform.position = position;

                    character.characterInfo.characterName = item.unitName;
                    character.characterInfo.printName = item.unitName;
                    character.characterInfo.attackSpeed = item.attackSpeed;
                    character.characterInfo.attackDist = item.attackDist;

                    character.characterInfo.maxHP = item.maxHP;
                    character.characterInfo.currentHP = item.currentHP;
                    character.characterInfo.maxMP = item.maxMP;
                    character.characterInfo.currentMP = item.currentMP;

                    character.characterInfo.damage = item.damage;
                    character.characterInfo.armor = item.armor;

                    character.characterInfo.criticalPercent = item.criticalPercent;
                    character.characterInfo.criticalDamagePercent = item.criticalDamagePercent;

                    //character.gameObject.SetActive(true);
                    EnemyPool.Instance.EnableCharacter(character);

                    // 체력설정
                    character.characterInfo.currentHP = item.currentHP;
                }


                // 유물 정보들
                foreach (var artifact in saveFile.artifacts)
                {
                    ArtifactManager.Instance.AddArtifact((ArtifactType)artifact.artifactType, artifact.artifactName);
                }

                // 인벤토리 정보
                foreach (var item in saveFile.inventoryDatas)
                {
                    Slot.Instance.AddItem(item.charInfo);
                }

                // 스테이지 정보
                InfiniteSpawnManager.Instance.LoadStage(saveFile.stageData.stage, saveFile.stageData.currentWaveCount,
                    saveFile.stageData.currentSpawnCount);

                // 토템배치 쿨타임 정보
                TotemManager.Instance.pastTime = saveFile.totemPastCoolTime;

                // 배치된 토템들 정보
                foreach (var totem in saveFile.totems)
                {
                    var go = TotemManager.Instance.CreateTotemOrNull(totem.totemName);
                    var totemScript = go.GetComponent<Totem>();

                    Vector3 position = new Vector3(totem.position[0], totem.position[1], totem.position[2]);
                    go.transform.position = position;
                    totemScript.totemDataset.pastingTime = totem.pastTime;
                    totemScript.totemDataset.buff.amount = totem.amount;

                    totemScript.LoadSavedTotem();
                }

                // 퀘스트 정보
                var quests = QuestEventManager.Instance.quests;
                foreach (var questData in saveFile.questDatas)
                {
                    foreach (var quest in quests)
                    {
                        if (questData.questName.Equals(quest.questName))
                        {
                            quest.LoadCount(questData.count);
                        }
                    }
                }

                // 유물 선택중인지
                if (saveFile.artifactSelectData.isSelecting)
                {
                    ArtifactSelect.Instance.LoadArtifactSelect(saveFile.artifactSelectData.artifactName);
                }

                stream.Close();
            }
            else
            {
                // 세이브파일 없음
                NewLoad();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void NewLoad()
    {
        GameManager.Instance.Pause();
        mainQuest.SetActive(true);
        InfiniteSpawnManager.Instance.LoadStage(1, InfiniteSpawnManager.Instance.initWaveCount, InfiniteSpawnManager.Instance.initSpawnCount);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && !GameManager.Instance.isGameOver)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        if (!GameManager.Instance.isGameOver)
            Save();
    }
}