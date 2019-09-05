using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public enum Prefix
{
    Upgrade,
    Glass,
    Giant,
    HighSpeed,
    SingleBlow,
    Tough,
    Weak,
    Assassin,
    None,
}

public enum Rarity
{
    Normal,
    Rare,
    Unique,
    Legend,
    Epic,
}

[System.Serializable]
public class CharacterInfo : ICloneable
{
    public string characterName;
    public string printName;

    public UnitName unitName;
    public Rarity rarity;
    public Prefix prefix;
    public float attackSpeed;

    public float maxHP;
    //[HideInInspector]
    public float currentHP;
    public float maxMP;
    //[HideInInspector]
    public float currentMP;

    public float damage;
    public float armor;
    public float gold;
    [HideInInspector]
    public float attackDelay;
    [HideInInspector]
    public float currentDelay;

    public float attackDist;

    public float criticalPercent;
    public float criticalDamagePercent;

    [HideInInspector]
    public int addedDamage;
    [HideInInspector]
    public int addedArmor;
    [HideInInspector]
    public float multipleMovingSpeed;
    [HideInInspector]
    public float multipleAttackSpeed;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
} // 나중에 public UnitName unitName; 추가해야한다.

public class Character : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    //public QuickSlotIcon quickSlotIcon;

    public Animator animator;
    public Attack myAttack;
    public BuffSystem buffSystem;
    public Movement myMovement;
    public CharacterInfo characterInfo;
    public Health myHealth;

    private HealthBar myHealthbar;
    public HealthBar healthBar
    {
        get
        {
            if (myHealthbar == null)
                myHealthbar = GetComponentInChildren<HealthBar>();
            return myHealthbar;
        }
    }
    


    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
        buffSystem = GetComponent<BuffSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myAttack = GetComponent<Attack>();
        myHealth = GetComponent<Health>();
    }
    
    protected void setZOrder()
    {
        Vector3 position = transform.position;
        position.z = position.y * 1f;
        transform.position = position;
        //spriteRenderer.sortingOrder = (int)transform.position.y * -1;
    }// 이미지들간의 위에그려질 우선순위 설정

    public void CorrectSpriteDirection(GameObject targetObject)
	{
		Vector2 relativeEnemyPosition = targetObject.transform.position - this.transform.position;

		if (relativeEnemyPosition.x < 0)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;
		}
	} // 출력되는 이미지의 방향을 올바르게 맞춰준다.

    public void EnableCharacter()
    {
        buffSystem.BuffReset();
        CharacterInfoScrollView.Instance.AddToList(characterInfo.printName,gameObject);
        gameObject.SetActive(true);

        if (tag == "unit")
            UnitPool.Instance.EnableCharacter(this);
        else
            EnemyPool.Instance.EnableCharacter(this);
    }

    public void DisableCharacter()
    {
        if(buffSystem != null)
            buffSystem.BuffReset();
        CharacterInfoScrollView.Instance.DeleteToList(gameObject);
        gameObject.SetActive(false);

        if (tag == "unit")
            UnitPool.Instance.DisableCharacter(this);
        else
            EnemyPool.Instance.DisableCharacter(this);
    }

    private void Update()
    {
        setZOrder();

        // 일시정지 상태일때 동작시키지 않음
        if (GameManager.Instance.GameSpeed == 0)
            return;

        if (buffSystem == null)
            return;

        // 버프에따른 업데이트
        characterInfo.addedDamage = (int)buffSystem.AttackAmount;
        characterInfo.addedArmor = (int)buffSystem.ArmorAmount;
        characterInfo.multipleMovingSpeed = buffSystem.MovingSpeedAmount;
        characterInfo.multipleAttackSpeed = buffSystem.AttackSpeedAmount;
    }
}
