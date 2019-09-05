using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System;

public enum EnemyName
{
    enemy_imp,
    enemy_vampire,
    enemy_spearman,
    enemy_assassin,

    enemy_whiteimp,
    enemy_whitevampire,
    enemy_whitespearman,
    enemy_blackassassin,

    enemy_griffin,
    enemy_blackgriffin,
}

public enum UnitType
{
    unit_warrior,
    unit_archer,
    unit_halberdier,
    unit_barbarian,
    unit_monk,
    unit_swordmaster,
}

public enum UnitName
{
    unit_warrior,
    unit_archer,
    unit_halberdier,
    unit_barbarian,
    unit_monk,
    unit_swordmaster,

    unit_highbarbarian,
    unit_highwarrior,
    unit_higharcher,
    unit_highhalberdier,
    unit_highmonk,
    unit_highswordmaster,

    unit_ultimatebarbarian,
    unit_ultimatewarrior,
    unit_ultimatearcher,
    unit_ultimatehalberdier,
    unit_ultimatemonk,
    unit_ultimateswordmaster,

    unit_fish,
    unit_null,
}

public enum GradientName
{
    CriticalDamageGradient,
    DamageGradient,
    DotDamageGradient,
    HealGradient,
    IncomeGoldGradient,
}

[System.Serializable]
public class CharacterDataset
{
    public Character character;
    public Dictionary<Rarity, CharacterInfo> infos;
}

[System.Serializable]
public class UnitNameAndSprite
{
    public UnitName unitName;
    public Sprite sprite;
}

public class Database : MonoBehaviour
{

    // 임시로 재화표현용
    public int gold = 0;

    // 인스팩터창에서 프리팹 지정용
    [SerializeField]
    private GameObject[] enemyList;
    [SerializeField]
    private GameObject[] unitList;

    public TMP_ColorGradient[] GradientList;

    // 실제 저장은 Json파일을 거쳐서 딕셔너리로 저장됨
    Dictionary<string, Character> enemyDict = new Dictionary<string, Character>();
    Dictionary<UnitName, CharacterDataset> unitDict = new Dictionary<UnitName, CharacterDataset>();
    Dictionary<string, TMP_ColorGradient> gradientDict = new Dictionary<string, TMP_ColorGradient>();

    // 머티리얼 저장용
    public Material[] rarityMaterial;

    // 버프 아이콘 저장용
    public Sprite[] buffIcon;

    // 등급별 색상 저장용
    public Color[] colors;

    // 떠있는 텍스트 저장용
    public GameObject floatingBasis;
    public GameObject incomeGold;

    // 유닛아이콘 스프라이트 저장용
    [SerializeField]
    UnitNameAndSprite[] unitIconSprites;
    public Dictionary<UnitName, Sprite> unitIconSpritesDict = new Dictionary<UnitName, Sprite>();

    // 유닛애니메이션 저장용
    [SerializeField]
    UnitAnimator[] unitAnimators;
    public Dictionary<UnitName, RuntimeAnimatorController> unitAnimatorsDict = new Dictionary<UnitName, RuntimeAnimatorController>();


    // 계열별 공격력, 방어력 곱 적용값
    public Dictionary<UnitName, float> unitMultipleAtkDict = new Dictionary<UnitName, float>();
    public Dictionary<UnitName, float> unitMultipleDefDict = new Dictionary<UnitName, float>();

    // 싱글톤
    private static Database instance = null;
    public static Database Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Database>();
            }
            return instance;
        }
    }

    // 딕셔너리에 지정한 프리팹들 넣어둔다.
    private void Awake()
    {
        foreach (var data in GradientList)
        {
            gradientDict.Add(data.name, data);
        }

        foreach (var unitIconSprite in unitIconSprites)
        {
            unitIconSpritesDict.Add(unitIconSprite.unitName, unitIconSprite.sprite);
        }

        foreach (var unitAnimator in unitAnimators)
        {
            unitAnimatorsDict.Add(unitAnimator.unitName, unitAnimator.runtimeAnimatorController);
        }

        SetUnitTypeAddedStat();

        StartCoroutine(LoadJsonDataCoroutine());
    }

    IEnumerator LoadJsonDataCoroutine()
    {
        yield return new WaitUntil(() => JsonManager.Instance.ready);

        LoadUnitsDataFromJson(JsonManager.Instance.jsonCharacters);
        LoadEnemiesDataFromJson(JsonManager.Instance.jsonEnemies);
    }


    public void LoadEnemiesDataFromJson(JsonEnemy[] jsonEnemies)
    {
        foreach (var jsonEnemy in jsonEnemies)
        {
            GameObject prefab = null;
            foreach (var enemy in enemyList)
            {
                if (jsonEnemy.enemy_type_name == enemy.name)
                {
                    prefab = enemy;
                    break;
                }
            }

            if (prefab == null)
                continue;

            Character character = prefab.GetComponent<Character>();

            character.characterInfo.characterName = jsonEnemy.enemy_name;
            character.characterInfo.attackSpeed = jsonEnemy.attack_speed;
            character.characterInfo.maxHP = jsonEnemy.max_hp;
            character.characterInfo.maxMP = jsonEnemy.max_mp;
            character.characterInfo.damage = jsonEnemy.damage;
            character.characterInfo.armor = jsonEnemy.armor;
            character.characterInfo.gold = jsonEnemy.gold;
            character.characterInfo.attackDist = jsonEnemy.attack_dist;
            character.characterInfo.criticalPercent = jsonEnemy.critical_percent;
            character.characterInfo.criticalDamagePercent = jsonEnemy.critical_damage_percent;

            prefab.GetComponent<Movement>().movingSpeed = jsonEnemy.moving_speed;

            enemyDict.Add(prefab.name, character);
        }
    }

    public void LoadUnitsDataFromJson(JsonCharacter[] jsonCharacters)
    {
        CharacterDataset[] characterDatasets = new CharacterDataset[unitList.Length];
        for (int i = 0; i < characterDatasets.Length; i++)
        {
            characterDatasets[i] = new CharacterDataset();
            characterDatasets[i].character = unitList[i].GetComponent<Character>();
            characterDatasets[i].infos = new Dictionary<Rarity, CharacterInfo>();

            unitDict.Add((UnitName)Enum.Parse(typeof(UnitName), unitList[i].name), characterDatasets[i]);
        }

        foreach (var jsonCharacter in jsonCharacters)
        {
            GameObject prefab = null;
            foreach (var unit in unitList)
            {
                if (jsonCharacter.character_type_name == unit.name)
                {
                    prefab = unit;
                    break;
                }
            }

            if (prefab == null)
                continue;

            CharacterInfo charInfo = new CharacterInfo();

            charInfo.characterName = jsonCharacter.character_name;
            charInfo.unitName = (UnitName)Enum.Parse(typeof(UnitName), jsonCharacter.character_type_name);
            charInfo.rarity = (Rarity)Enum.Parse(typeof(Rarity), jsonCharacter.rarity);
            charInfo.attackSpeed = jsonCharacter.attack_speed;
            charInfo.maxHP = jsonCharacter.max_hp;
            charInfo.maxMP = jsonCharacter.max_mp;
            charInfo.damage = jsonCharacter.damage;
            charInfo.armor = jsonCharacter.armor;
            charInfo.attackDist = jsonCharacter.attack_dist;
            charInfo.criticalPercent = jsonCharacter.critical_percent;
            charInfo.criticalDamagePercent = jsonCharacter.critical_damage_percent;

            unitDict[charInfo.unitName].infos.Add(charInfo.rarity, charInfo);
        }
    }


    // 외부에서 프리팹정보 가져올 때 사용하는부분
    public CharacterDataset GetUnitDataset(UnitName unitName)
    {
        return unitDict[unitName];
    }

    // 외부에서 프리팹정보 가져올 때 사용하는부분
    public Character GetEnemyPrefab(string name)
    {
        return enemyDict[name];
    }

    public TMP_ColorGradient GetGradient(string name)
    {
        return gradientDict[name];
    }

    public void SetUnitTypeAddedStat()
    {
        // 계열별 공격력, 방어력 곱 적용값을 가져온다.

        //예외처리
        unitMultipleAtkDict[UnitName.unit_fish] = 1.0f;
        unitMultipleDefDict[UnitName.unit_fish] = 1.0f;

        foreach (var unitType in Enum.GetValues(typeof(UnitType)))
        {
            string atk = unitType + "_atk";
            string def = unitType + "_def";

            if (!PlayerPrefs.HasKey(atk))
            {
                PlayerPrefs.SetInt(atk, 1);
            }

            float val = 1f + (PlayerPrefs.GetInt(atk, 1) -1) * 0.1f;
            

            switch (unitType)
            {
                case UnitType.unit_archer:
                    unitMultipleAtkDict[UnitName.unit_archer] = val;
                    unitMultipleAtkDict[UnitName.unit_higharcher] = val;
                    unitMultipleAtkDict[UnitName.unit_ultimatearcher] = val;
                    break;
                case UnitType.unit_barbarian:
                    unitMultipleAtkDict[UnitName.unit_barbarian] = val;
                    unitMultipleAtkDict[UnitName.unit_highbarbarian] = val;
                    unitMultipleAtkDict[UnitName.unit_ultimatebarbarian] = val;
                    break;
                case UnitType.unit_halberdier:
                    unitMultipleAtkDict[UnitName.unit_halberdier] = val;
                    unitMultipleAtkDict[UnitName.unit_highhalberdier] = val;
                    unitMultipleAtkDict[UnitName.unit_ultimatehalberdier] = val;
                    break;
                case UnitType.unit_monk:
                    unitMultipleAtkDict[UnitName.unit_monk] = val;
                    unitMultipleAtkDict[UnitName.unit_highmonk] = val;
                    unitMultipleAtkDict[UnitName.unit_ultimatemonk] = val;
                    break;
                case UnitType.unit_swordmaster:
                    unitMultipleAtkDict[UnitName.unit_swordmaster] = val;
                    unitMultipleAtkDict[UnitName.unit_highswordmaster] = val;
                    unitMultipleAtkDict[UnitName.unit_ultimateswordmaster] = val;
                    break;
                case UnitType.unit_warrior:
                    unitMultipleAtkDict[UnitName.unit_warrior] = val;
                    unitMultipleAtkDict[UnitName.unit_highwarrior] = val;
                    unitMultipleAtkDict[UnitName.unit_ultimatewarrior] = val;
                    break;
            }


            if (!PlayerPrefs.HasKey(def))
            {
                PlayerPrefs.SetInt(def, 1);
            }

            val = 1f + (PlayerPrefs.GetInt(def, 1)-1) * 0.1f;

            switch (unitType)
            {
                case UnitType.unit_archer:
                    unitMultipleDefDict[UnitName.unit_archer] = val;
                    unitMultipleDefDict[UnitName.unit_higharcher] = val;
                    unitMultipleDefDict[UnitName.unit_ultimatearcher] = val;
                    break;
                case UnitType.unit_barbarian:
                    unitMultipleDefDict[UnitName.unit_barbarian] = val;
                    unitMultipleDefDict[UnitName.unit_highbarbarian] = val;
                    unitMultipleDefDict[UnitName.unit_ultimatebarbarian] = val;
                    break;
                case UnitType.unit_halberdier:
                    unitMultipleDefDict[UnitName.unit_halberdier] = val;
                    unitMultipleDefDict[UnitName.unit_highhalberdier] = val;
                    unitMultipleDefDict[UnitName.unit_ultimatehalberdier] = val;
                    break;
                case UnitType.unit_monk:
                    unitMultipleDefDict[UnitName.unit_monk] = val;
                    unitMultipleDefDict[UnitName.unit_highmonk] = val;
                    unitMultipleDefDict[UnitName.unit_ultimatemonk] = val;
                    break;
                case UnitType.unit_swordmaster:
                    unitMultipleDefDict[UnitName.unit_swordmaster] = val;
                    unitMultipleDefDict[UnitName.unit_highswordmaster] = val;
                    unitMultipleDefDict[UnitName.unit_ultimateswordmaster] = val;
                    break;
                case UnitType.unit_warrior:
                    unitMultipleDefDict[UnitName.unit_warrior] = val;
                    unitMultipleDefDict[UnitName.unit_highwarrior] = val;
                    unitMultipleDefDict[UnitName.unit_ultimatewarrior] = val;
                    break;
            }
        }
    }


}
