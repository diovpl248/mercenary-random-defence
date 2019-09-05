using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour {
    /*
    private MultiDictionary<string, Character> enemyDict = new MultiDictionary<string, Character>();
    public MultiDictionary<string, Character> EnemyDict
    {
        get
        {
            return enemyDict;
        }
    }*/
    //public Dictionary<string, Stack<Character>> enemyDict = new Dictionary<string, Stack<Character>>();

    public List<Character> enemyTotalList = new List<Character>();

    Stack<Character> enemyList = new Stack<Character>();

    private static EnemyPool instance = null;
    public static EnemyPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyPool>();
            }
            return instance;
        }
    }
    /*
    private void Start()
    {
        // 미리 적 10마리 만들어놓는다.
        string tempName = EnemyName.enemy_imp.ToString();
        for (int i = 0; i < 10; i++)
        {
            Character prefabCharacter = Database.Instance.GetEnemyPrefab(tempName);
            GameObject newGameObject = Instantiate(prefabCharacter.gameObject);
            newGameObject.name = prefabCharacter.gameObject.name;

            newGameObject.SetActive(false);
            newGameObject.transform.parent = this.transform;

            AddNewEnemy(newGameObject.GetComponent<Character>());
        }
    }*/

    public void AddNewEnemy(Character character)
    {
        enemyList.Push(character);
        //enemyTotalList.Add(character);
    }

    public void EnableCharacter(Character character)
    {
        enemyTotalList.Add(character);
        character.gameObject.SetActive(true);
    }

    public void DisableCharacter(Character character)
    {
        enemyList.Push(character);
        enemyTotalList.Remove(character);
        character.gameObject.SetActive(false);
    }

    public List<Character> GetActiveCharacters()
    {
        return enemyTotalList;
        /*
        List<Character> characters = new List<Character>();
        foreach(var enemy in enemyTotalList)
        {
            if(enemy.gameObject.activeInHierarchy && enemy.characterInfo.currentHP > 0)
            {
                characters.Add(enemy);
            }
        }
        return characters;*/
    }

    public Character GetFromPool(string name)
    {
        if (enemyList.Count <= 0)
        {
            // 풀에 적이 없는경우 5개 한번에 만들어놓음
            for (int i=0;i<5;i++)
            {
                Character prefabCharacter = Database.Instance.GetEnemyPrefab(name);
                GameObject newGameObject = Instantiate(prefabCharacter.gameObject);
                newGameObject.name = prefabCharacter.gameObject.name;

                newGameObject.SetActive(false);
                newGameObject.transform.SetParent(transform);

                AddNewEnemy(newGameObject.GetComponent<Character>());
            }
        }

        Character character = enemyList.Pop();
        Character baseCharacter = Database.Instance.GetEnemyPrefab(name);

        character.gameObject.name = baseCharacter.gameObject.name;
        if (character.myMovement != null)
            character.myMovement.movingSpeed = baseCharacter.myMovement.movingSpeed;
        character.myHealth.dieClip = baseCharacter.myHealth.dieClip;
        character.animator.runtimeAnimatorController = baseCharacter.animator.runtimeAnimatorController;
        character.characterInfo = (CharacterInfo)baseCharacter.characterInfo.Clone();

        // 스킬복사
        if(character.myAttack.skills.Length != baseCharacter.myAttack.skills.Length)
        {
            character.myAttack.skills = new Skill[baseCharacter.myAttack.skills.Length];
        }

        for(int i=0;i< baseCharacter.myAttack.skills.Length;i++)
        {
            character.myAttack.skills[i] = (Skill)baseCharacter.myAttack.skills[i].Clone();
        }
        
        var rect = character.healthBar.slider.GetComponent<RectTransform>();
        if (name == EnemyName.enemy_griffin.ToString() || name == EnemyName.enemy_blackgriffin.ToString())
        {
            rect.sizeDelta = new Vector3(200, rect.sizeDelta.y);
        }
        else
        {
            rect.sizeDelta = new Vector3(80, rect.sizeDelta.y);
        }
        
        return character;
    }

    /*
    public GameObject GetFromPool(string name)
    {
        if (!enemyDict.ContainsKey(name))
            enemyDict.Add(name, new List<Character>());

        List<Character> enemyList = enemyDict[name];

        foreach (var enemy in enemyList)
        {
            if(!enemy.gameObject.activeInHierarchy)
            {
                return enemy.gameObject;
            }
        }
        

        GameObject prefab = Database.Instance.GetEnemyPrefab(name);
        GameObject newGameObject = Instantiate(prefab);
        newGameObject.name = prefab.name;

        newGameObject.SetActive(false);
        newGameObject.transform.parent = this.transform;

        enemyDict[name].Add(newGameObject.GetComponent<Character>());

        return newGameObject;
    }*/

}
