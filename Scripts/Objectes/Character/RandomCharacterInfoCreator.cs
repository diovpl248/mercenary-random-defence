using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCharacterInfoCreator : MonoBehaviour
{
    private static RandomCharacterInfoCreator instance;
    public static RandomCharacterInfoCreator Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RandomCharacterInfoCreator>();
            return instance;
        }
    }

    public CharacterInfo CreateRandomStat(UnitName unitName,Rarity rarity, Prefix prefix)
    {
        CharacterDataset dataset = Database.Instance.GetUnitDataset(unitName);
        CharacterInfo datasetCharInfo = (CharacterInfo)dataset.infos[rarity].Clone();

        datasetCharInfo.printName = datasetCharInfo.characterName;

        switch (rarity)
        {
            case Rarity.Normal:
                datasetCharInfo.printName = "일반 " + datasetCharInfo.printName;
                break;

            case Rarity.Rare:
                datasetCharInfo.printName = "희귀 " + datasetCharInfo.printName;
                break;

            case Rarity.Unique:
                datasetCharInfo.printName = "유일 " + datasetCharInfo.printName;
                break;

            case Rarity.Legend:
                datasetCharInfo.printName = "전설 " + datasetCharInfo.printName;
                break;

            case Rarity.Epic:
                datasetCharInfo.printName = "신화 " + datasetCharInfo.printName;
                break;
            default:
                break;
        }

        datasetCharInfo.damage *= Random.Range(0.9f, 1.1f);
        datasetCharInfo.maxHP *= Random.Range(0.9f, 1.1f);
        datasetCharInfo.maxMP *= Random.Range(0.9f, 1.1f);
        datasetCharInfo.armor *= Random.Range(0.9f, 1.1f);

        datasetCharInfo.currentHP = datasetCharInfo.maxHP;

        datasetCharInfo.prefix = prefix;
        switch (prefix)
        {
            case Prefix.HighSpeed:
                datasetCharInfo.printName = "재빠른 " + datasetCharInfo.printName;
                datasetCharInfo.attackSpeed *= 1.5f;
                break;
            case Prefix.SingleBlow:
                datasetCharInfo.printName = "일격의 " + datasetCharInfo.printName;
                datasetCharInfo.damage *= 1.5f;
                datasetCharInfo.attackSpeed *= 0.8f;
                break;
            case Prefix.Tough:
                datasetCharInfo.printName = "단단한 " + datasetCharInfo.printName;
                datasetCharInfo.armor = (datasetCharInfo.armor + 10) * 1.5f;
                datasetCharInfo.maxHP *= 1.5f;
                datasetCharInfo.currentHP = datasetCharInfo.maxHP;
                break;
            case Prefix.Weak:
                datasetCharInfo.printName = "나약한 " + datasetCharInfo.printName;
                datasetCharInfo.maxHP *= 4f/5f;
                datasetCharInfo.currentHP = datasetCharInfo.maxHP;
                break;
            case Prefix.Assassin:
                datasetCharInfo.printName = "암살의 " + datasetCharInfo.printName;
                datasetCharInfo.criticalDamagePercent += 0.5f;
                datasetCharInfo.criticalPercent += 30f;
                break;
                /*
            case Prefix.Random:
                datasetCharInfo.printName = "인생도박꾼 " + datasetCharInfo.printName;
                datasetCharInfo.damage *= Random.Range(0.7f, 1.1f);
                datasetCharInfo.maxHP *= Random.Range(0.7f, 1.1f);
                datasetCharInfo.maxMP *= Random.Range(0.7f, 1.1f);
                datasetCharInfo.armor *= Random.Range(0.7f, 1.1f);  
                datasetCharInfo.attackSpeed += Random.Range(0.1f, 0.4f);
                datasetCharInfo.currentHP = datasetCharInfo.maxHP;
                break;
            case Prefix.FakeExpert:
                datasetCharInfo.printName = "X문가 " + datasetCharInfo.printName;
                datasetCharInfo.criticalPercent += 100f;
                datasetCharInfo.criticalDamagePercent -= 0.9f;
                break;
            case Prefix.Carefull:
                datasetCharInfo.printName = "신중한 " + datasetCharInfo.printName;
                datasetCharInfo.damage *= 4;
                datasetCharInfo.attackSpeed *= 2f/3f;
                datasetCharInfo.criticalPercent *= 0.3f;
                break;
            case Prefix.Careless:
                datasetCharInfo.printName = "덤벙거리는 " + datasetCharInfo.printName;
                datasetCharInfo.attackSpeed *= 1.3f;
                datasetCharInfo.damage *= 0.7f;
                datasetCharInfo.criticalPercent *= 2f/3f;
                break;
            case Prefix.Insight:
                datasetCharInfo.printName = "천리안 " + datasetCharInfo.printName;
                datasetCharInfo.attackDist *= 1.5f;
                datasetCharInfo.criticalDamagePercent += 0.5f;
                datasetCharInfo.criticalPercent += 10f;
                break;*/
            case Prefix.Upgrade:
                datasetCharInfo.printName = "강화된 " + datasetCharInfo.printName;
                datasetCharInfo.maxHP *= 1.25f;
                datasetCharInfo.damage *= 1.25f;
                datasetCharInfo.attackSpeed *= 1.25f;
                datasetCharInfo.armor *= 1.25f;
                datasetCharInfo.criticalDamagePercent += 0.3f;
                datasetCharInfo.criticalPercent += 5f;
                datasetCharInfo.currentHP = datasetCharInfo.maxHP;
                break;
            case Prefix.Glass:
                datasetCharInfo.printName = "유리같은 " + datasetCharInfo.printName;
                datasetCharInfo.maxHP *= 0.75f;
                datasetCharInfo.damage *= 1.5f;
                datasetCharInfo.armor *= 0.75f;
                datasetCharInfo.currentHP = datasetCharInfo.maxHP;
                break;
                /*
            case Prefix.Pacifist:
                datasetCharInfo.printName = "평화주의자 " + datasetCharInfo.printName;
                datasetCharInfo.maxHP *= 3f;
                datasetCharInfo.damage *= 0.2f;
                datasetCharInfo.armor *= 3f;
                datasetCharInfo.currentHP = datasetCharInfo.maxHP;
                break;*/
            case Prefix.None:
                //datasetCharInfo.printName = datasetCharInfo.characterName;
                break;
            default:
                break;
        }

        return datasetCharInfo;
    }
}
