using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{
    /*
    private MultiDictionary<string, Character> unitDict = new MultiDictionary<string, Character>();
    public MultiDictionary<string, Character> UnitDict
    {
        get
        {
            return unitDict;
        }
    }*/
    public List<Character> unitTotalList = new List<Character>();

    Stack<Character> unitList = new Stack<Character>();

    private static UnitPool instance = null;
    public static UnitPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnitPool>();
            }
            return instance;
        }
    }
    /*
    private void Start()
    {
        // 미리 캐릭터 10마리 만들어놓는다.
        var tempName = UnitName.unit_archer;
        for (int i = 0; i < 10; i++)
        {
            CharacterDataset prefabCharacterDataset = Database.Instance.GetUnitDataset(tempName);
            GameObject newGameObject = Instantiate(prefabCharacterDataset.character.gameObject);
            newGameObject.name = prefabCharacterDataset.character.gameObject.name;

            newGameObject.SetActive(false);
            newGameObject.transform.SetParent(transform);

            AddNewUnit(newGameObject.GetComponent<Character>());
        }
    }*/

    void AddNewUnit(Character character)
    {
        unitList.Push(character);
        //unitTotalList.Add(character);
    }

    public void EnableCharacter(Character character)
    {
        unitTotalList.Add(character);
        character.gameObject.SetActive(true);
    }

    public void DisableCharacter(Character character)
    {
        unitList.Push(character);
        unitTotalList.Remove(character);
        character.gameObject.SetActive(false);
    }

    public List<Character> GetActiveCharacters()
    {
        return unitTotalList;
        /*
        List<Character> characters = new List<Character>();
        foreach (var unit in unitTotalList)
        {
            if (unit.gameObject.activeInHierarchy && unit.characterInfo.currentHP > 0)
            {
                characters.Add(unit);
            }
        }
        return characters;*/
    }

    public Character GetFromPool(CharacterInfo charInfo)
    {
        Character character = GetFromPool(charInfo.unitName, charInfo.rarity);
        character.characterInfo = (CharacterInfo)charInfo.Clone();
        return character;
    }

    public Character GetFromPool(UnitName unitName, Rarity rarity)
    {
        if (unitList.Count <= 0)
        {
            // 풀에 유닛이 없는경우 5개 한번에 만들어놓음
            var tempName = UnitName.unit_archer;
            for (int i = 0; i < 5; i++)
            {
                CharacterDataset prefabCharacterDataset = Database.Instance.GetUnitDataset(tempName);
                GameObject newGameObject = Instantiate(prefabCharacterDataset.character.gameObject);
                newGameObject.name = prefabCharacterDataset.character.gameObject.name;

                newGameObject.SetActive(false);
                newGameObject.transform.SetParent(transform);

                AddNewUnit(newGameObject.GetComponent<Character>());
            }
        }

        Character character = unitList.Pop();
        CharacterDataset baseCharacterDataset = Database.Instance.GetUnitDataset(unitName);

        character.myHealth.dieClip = baseCharacterDataset.character.myHealth.dieClip;
        character.spriteRenderer.material = Database.Instance.rarityMaterial[(int)rarity];
        character.animator.runtimeAnimatorController = baseCharacterDataset.character.animator.runtimeAnimatorController;
        character.characterInfo = (CharacterInfo)baseCharacterDataset.infos[rarity].Clone();

        // 스킬복사
        if (character.myAttack.skills.Length != baseCharacterDataset.character.myAttack.skills.Length)
        {
            character.myAttack.skills = new Skill[baseCharacterDataset.character.myAttack.skills.Length];
        }

        for (int i = 0; i < baseCharacterDataset.character.myAttack.skills.Length; i++)
        {
            character.myAttack.skills[i] = (Skill)baseCharacterDataset.character.myAttack.skills[i].Clone();
        }

        character.gameObject.name = character.characterInfo.unitName.ToString();

        return character;
    }

    /*
    public void setStat(ref CharacterInfo charInfo, CharacterInfo descInfo)
    {
        charInfo = descInfo;
    }*/
}
