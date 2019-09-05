using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArtifactType
{
    DamageUP,
    DamageDown,
    DefenseUP,
    DefenseDown,
    BloodSucking,
    GoldUP,
    TotemCoolDown,
    CriticalPercentUP,
    Combination,
}

public enum ArtifactRarity
{
    Normal,
    Rare,
    Unique,
    Legend,
}

[System.Serializable]
public class Artifact
{
    public ArtifactType type;
    public ArtifactRarity artifactRarity;
    public string name;
    public string description;
    public float amount;
    public Sprite sprite;

    public Artifact(ArtifactType type, ArtifactRarity rarity, string name, string desc, float amount, Sprite sprite)
    {
        this.type = type;
        this.name = name;
        this.artifactRarity = rarity;
        this.description = desc;
        this.amount = amount;
        this.sprite = sprite;
    }

    static public string ArtifactTypeToString(ArtifactType type)
    {
        string str="";

        switch(type)
        {
            case ArtifactType.BloodSucking:
                str = "흡혈량 증가";
                break;
            case ArtifactType.Combination:
                str = "조합재료";
                break;
            case ArtifactType.CriticalPercentUP:
                str = "크리티컬 확률 증가";
                break;
            case ArtifactType.DamageUP:
                str = "공격력 증가";
                break;
            case ArtifactType.DefenseUP:
                str = "방어력 증가";
                break;
            case ArtifactType.GoldUP:
                str = "골드 획득량 증가";
                break;
            case ArtifactType.TotemCoolDown:
                str = "토템 쿨타임 감소";
                break;
        }

        return str;
    }
}
