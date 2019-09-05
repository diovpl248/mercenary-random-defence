using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuffType
{
    AttackUP,
    AttackDown,
    ArmorUP,
    ArmorDown,
    SpeedUP,
    SpeedDown,
    DotHeal,
    DotDamage,
    AttackSpeedUP,
    AttackSpeedDown,
}

[System.Serializable]
public class Buff
{
    public BuffType buffType;
    public bool isPositive;
    public string buffName;
    public string buffDesc;
    public float amount;
    public float durationTime;
    public float tickTime;
    public float pastTime;
    private float lastTickTime;
    public float orgAmount;

    public Buff(Buff buff)
    {
        this.buffType = buff.buffType;
        this.isPositive = buff.isPositive;
        this.buffName = buff.buffName;
        this.buffDesc = buff.buffDesc;
        this.amount = buff.amount;
        orgAmount = buff.amount;
        this.durationTime = buff.durationTime;
        this.tickTime = buff.tickTime;
        pastTime = buff.pastTime;
        
        lastTickTime = 0;
    }

    public Buff(BuffType buffType, bool isPositive, string buffName, string buffDesc, float amount,float durationTime, float tickTime, float pastTime=0)
    {
        this.buffType = buffType;
        this.isPositive = isPositive;
        this.amount = amount;
        this.buffName = buffName;
        this.buffDesc = buffDesc;
        this.durationTime = durationTime;
        this.tickTime = tickTime;
        this.pastTime = pastTime;

        lastTickTime = 0;
    }

    public void Update()
    {
        pastTime += Time.deltaTime * GameManager.Instance.GameSpeed;
    }

    public bool CheckTickCount()
    {
        if(pastTime - lastTickTime >= tickTime)
        {
            lastTickTime = pastTime;
            return true;
        }
        return false;
    }

    public bool IsEnded()
    {
        if (durationTime == 0)
            return false;

        if(durationTime <= pastTime)
        {
            return true;
        }
        return false;
    }

    public Buff Clone()
    {
        Buff clone = new Buff(this);
        return clone;
    }
}

public class BuffSystem : MonoBehaviour
{
    public Dictionary<Buff, GameObject> buffs = new Dictionary<Buff, GameObject>();

    public GameObject buffIconPrefab;
    public GameObject buffPanel;
    public BuffPool buffP;

    private float attackAmount;
    public float AttackAmount { get { return attackAmount; } }

    private float armorAmount;
    public float ArmorAmount { get { return armorAmount; } }

    private float movingSpeedAmount;
    public float MovingSpeedAmount { get { return movingSpeedAmount; } }

    private float attackSpeedAmount;
    public float AttackSpeedAmount { get { return attackSpeedAmount; } }

    private int dotDamageCount = 0;
    public bool havingDotDamanage { get { return dotDamageCount >= 1; } }

    private void Awake()
    {
        buffP = GetComponent<BuffPool>();
    }
    void Update()
    {
        List<Buff> buffList = new List<Buff>(buffs.Keys);
        foreach(Buff buff in buffList)
        {
            BuffUpdate(buff);
        }

        tickBuffs();
    }

    public void BuffReset()
    {
        List<Buff> buffList = new List<Buff>(buffs.Keys);
        foreach (Buff buff in buffList)
        {
            DeleteBuff(buff);
        }
        buffs.Clear();
    }

    public void BuffUpdate(Buff buff)
    {
        buff.Update();
        if (buff.IsEnded())
            DeleteBuff(buff);
    }

    public void AddBuff(Buff buff)
    {
        if (buffs.ContainsKey(buff))
            return;

        if (buff.buffType == BuffType.DotDamage)
            dotDamageCount++;
        
        GameObject temp = buffP.GetFromPool();
        temp.transform.position = buffPanel.transform.position;
        temp.gameObject.SetActive(true);
        temp.GetComponent<Image>().sprite = Database.Instance.buffIcon[(int)buff.buffType];
        buffs.Add(buff, temp);
        
    }

    public void DeleteBuff(Buff buff)
    {
        if(buffs.ContainsKey(buff))
        {
            if (buff.buffType == BuffType.DotDamage)
                dotDamageCount--;

            //Destroy(buffs[buff]);
            buffs[buff].gameObject.SetActive(false);
            buffs.Remove(buff);
        }
    }

    private void tickBuffs()
    {
        float tempAttackAmount = 0f;
        float tempArmorAmount = 0f;
        float tempMovingSpeedAmount = 1.0f;
        float tempAttackSpeedAmount = 1.0f;
        
        foreach (var buffPair in buffs)
        {
            Buff buff = buffPair.Key;
            switch (buff.buffType)
            {
                case BuffType.AttackUP:
                    tempAttackAmount += buff.amount;
                    break;
                case BuffType.ArmorUP:
                    tempArmorAmount += buff.amount;
                    break;
                case BuffType.SpeedUP:
                    tempMovingSpeedAmount *= (1f + buff.amount);
                    break;
                case BuffType.AttackSpeedUP:
                    tempAttackSpeedAmount *= (1f+buff.amount);
                    break;

                case BuffType.AttackDown:
                    tempAttackAmount -= buff.amount;
                    break;
                case BuffType.ArmorDown:
                    tempArmorAmount -= buff.amount;
                    break;
                case BuffType.SpeedDown:
                    tempMovingSpeedAmount *= (1f-buff.amount);
                    break;
                case BuffType.AttackSpeedDown:
                    tempAttackSpeedAmount *= (1f-buff.amount);
                    break;
            }
        }

        attackAmount = tempAttackAmount;
        armorAmount = tempArmorAmount;
        movingSpeedAmount = tempMovingSpeedAmount;
        attackSpeedAmount = tempAttackSpeedAmount;
    }

    public void TickBuffsForHalth(out int heal, out int damage)
    {
        heal = 0;
        damage = 0;
        
        foreach (var buffPair in buffs)
        {
            Buff buff = buffPair.Key;

            if (buff.buffType == BuffType.DotHeal)
            {
                if (buff.CheckTickCount())
                    heal += (int)buff.amount;
            }

            if (buff.buffType == BuffType.DotDamage)
            {
                if (buff.CheckTickCount())
                    damage += (int)buff.amount;
            }
        }

    }
}
