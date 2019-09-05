using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[System.Serializable]
public class JsonCombination
{
    public int id;
    public string result;
    public string artifact;
    public string minRarity;
    public string material0;
    public string material1;
    public string material2;
    public string material3;
}

[System.Serializable]
public class JsonArtifact
{
    public int id;
    public string artifact_name;
    public string type;
    public string rarity;
    public string desc;
    public float amount;
    public string sprite_name;
    public int sprite_number;
}

[System.Serializable]
public class JsonCharacter
{
    public int id;
    public string character_name;
    public string character_type_name;
    public string rarity;
    public float attack_speed;
    public int max_hp;
    public int max_mp;
    public int damage;
    public int armor;
    public int attack_dist;
    public float critical_percent;
    public float critical_damage_percent;
}

[System.Serializable]
public class JsonEnemy
{
    public int id;
    public string enemy_name;
    public string enemy_type_name;
    public float attack_speed;
    public int max_hp;
    public int max_mp;
    public int damage;
    public int armor;
    public int attack_dist;
    public float critical_percent;
    public float critical_damage_percent;
    public int moving_speed;
    public int gold;
}

[System.Serializable]
public class JsonTotem
{
    public int id;
    public string totem_name;
    public string totem_type_name;
    public string sprite_name;
    public int radius;
    public float duration_time;
    public int affect_myunit;
    public int affect_enemy;
    public string buff_type;
    public int buff_positive;
    public string buff_name;
    public string buff_desc;
    public float buff_amount;
    public float buff_tick_time;
}


public class JsonManager : MonoBehaviour
{
    public bool ready = false;
    public JsonArtifact[] jsonArtifacts;
    public JsonCharacter[] jsonCharacters;
    public JsonEnemy[] jsonEnemies;
    public JsonTotem[] jsonTotems;
    public JsonCombination[] jsonCombinations;

    private static JsonManager instance;
    public static JsonManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<JsonManager>();
            }
            return instance;
        }
    }


    void Awake()
    {
        readJsonFiles();
        //setData();
    }

    void readJsonFiles()
    {
        // Artifact
        TextAsset textAsset = Resources.Load<TextAsset>("Json/Artifact");
        jsonArtifacts = JsonHelper.FromJson<JsonArtifact>(textAsset.text);

        // Character
        textAsset = Resources.Load<TextAsset>("Json/Character");
        jsonCharacters = JsonHelper.FromJson<JsonCharacter>(textAsset.text);

        // Enemy
        textAsset = Resources.Load<TextAsset>("Json/Enemy");
        jsonEnemies = JsonHelper.FromJson<JsonEnemy>(textAsset.text);

        // Totem
        textAsset = Resources.Load<TextAsset>("Json/Totem");
        jsonTotems = JsonHelper.FromJson<JsonTotem>(textAsset.text);

        // Combination
        // Totem
        textAsset = Resources.Load<TextAsset>("Json/Combination");
        jsonCombinations = JsonHelper.FromJson<JsonCombination>(textAsset.text);

        ready = true;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}