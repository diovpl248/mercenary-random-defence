using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPrefixCreator : MonoBehaviour
{

    public Health health;
    private static EnemyPrefixCreator instance = null;
    public static EnemyPrefixCreator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyPrefixCreator>();
            }
            return instance;
        }
    }

    public void SetEnemyPrefix(Character enemy, Prefix prefix)
    {
        Prefix nowPrefix = prefix;

        Transform canvasTransform = enemy.healthBar.transform;

        enemy.transform.localScale = new Vector3(1, 1, 1);
        canvasTransform.transform.localScale = new Vector3(1, 1, 1);

        switch (nowPrefix)
        {
            case Prefix.HighSpeed:
                enemy.characterInfo.printName = "신속의 " + enemy.characterInfo.characterName;
                enemy.characterInfo.attackSpeed *= 1.5f;
                break;
            case Prefix.SingleBlow:
                enemy.characterInfo.printName = "일격의 " + enemy.characterInfo.characterName;
                enemy.characterInfo.damage *= 3;
                enemy.characterInfo.attackSpeed *= 0.8f;
                break;
            case Prefix.Tough:
                enemy.characterInfo.printName = "강인한 " + enemy.characterInfo.characterName;
                enemy.characterInfo.armor = (enemy.characterInfo.armor + 10) * 1.5f;
                enemy.characterInfo.maxHP *= 2;
                enemy.characterInfo.currentHP = enemy.characterInfo.maxHP;
                break;
            case Prefix.Weak:
                enemy.characterInfo.printName = "나약한 " + enemy.characterInfo.characterName;
                enemy.characterInfo.damage *= 2f / 3f;
                enemy.characterInfo.maxHP *= 2f / 3f;
                enemy.characterInfo.currentHP = enemy.characterInfo.maxHP;
                break;
            case Prefix.Assassin:
                enemy.characterInfo.printName = "암살의 " + enemy.characterInfo.characterName;
                enemy.characterInfo.criticalDamagePercent += 0.5f;
                enemy.characterInfo.criticalPercent += 20f;
                break;
            case Prefix.Giant:
                enemy.characterInfo.printName = "거대한 " + enemy.characterInfo.characterName;

                enemy.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                canvasTransform.transform.localScale = new Vector3(0.67f, 0.67f, 0.67f);

                enemy.characterInfo.damage *= 2f;
                enemy.characterInfo.maxHP *= 2f;
                enemy.characterInfo.attackSpeed *= 0.75f;
                enemy.characterInfo.armor *= 1.5f;
                enemy.characterInfo.currentHP = enemy.characterInfo.maxHP;
                break;
            case Prefix.None:
                enemy.characterInfo.printName = enemy.characterInfo.characterName;
                break;
            default:
                break;
        }
    }

    public Prefix RandomEnemyPrefix()
    {
        Prefix enemyPrefix = Prefix.None;
        float prefixRand = Random.value;
        if (prefixRand < 0.7f)
        {
            enemyPrefix = Prefix.None;
        }
        else
        {
            enemyPrefix = (Prefix)Random.Range(System.Enum.GetValues(typeof(Prefix)).Length - 7, System.Enum.GetValues(typeof(Prefix)).Length - 1);
        }

        return enemyPrefix;
    }
}
