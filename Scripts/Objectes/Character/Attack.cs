using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageType
{
    Normal,
    DotDamage,
    MagicDamage,
}

public enum RangeAttackType
{
    None,
    TargetCircle,
    AroundCircle,
    Cone,
    Line,
}

[System.Serializable]
public class Skill : System.ICloneable
{
    public string name;
    public string skillDesc;

    public string animName;
    public int weight;

    public float coolTime;
    public float pastCoolTime;

    public SkillInfo[] skillInfos;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[System.Serializable]
public class SkillInfo
{
    public DamageType damageType;
    public float damagePercent; //계수

    public AudioClip sfxClip;

    public string magicName;

    public bool bloodSucking;
    public float bloodPercent;

    public RangeAttackType rangeAttackType;
    public float dgree;
    public float length;

    public bool doUseBuff;
    public Buff buff;
}

public class Attack : MonoBehaviour
{
    protected Character character;
    protected Health myHealth;

    protected Animator animator;
    protected AudioSource audioSource;
    protected AudioClip sfxClip;
    public bool isAttacking = false;

    EnemyPool theEnemyPool;
    UnitPool theUnitPool;

    //[HideInInspector]
    public GameObject targetObject;
    GameObject lastTargetObject;

    public Skill[] skills;

    private Skill currentSkill;

    [HideInInspector]
    public Ellipse attackEllipse;

    WaitForSeconds waitTime = new WaitForSeconds(0.1f);
    List<KeyValuePair<int, Skill>> usableSkills = new List<KeyValuePair<int, Skill>>();

    Vector3 lastTargetPosition;

    private void Awake()
    {
        character = GetComponent<Character>();
        myHealth = GetComponent<Health>();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        theEnemyPool = EnemyPool.Instance;
        theUnitPool = UnitPool.Instance;
    }

    protected void init()
    {
        attackEllipse = new Ellipse(transform, character.characterInfo.attackDist);

        isAttacking = false;
        StopAllCoroutines();

        string attackLayer = "";

        if (gameObject.tag == "unit")
            attackLayer = "enemy";
        else
            attackLayer = "unit";

        StartCoroutine(findNewTargetCoroutine(attackLayer));
    }

    public IEnumerator findNewTargetCoroutine(string layerName)
    {
        while (true)
        {
            targetObject = null;

            //MultiDictionary<string, Character> dict;
            List<Character> charList;
            if (layerName == "enemy")
            {
                //dict = theEnemyPool.EnemyDict;
                charList = theEnemyPool.enemyTotalList;
            }
            else
            {
                //dict = theUnitPool.UnitDict;
                charList = theUnitPool.unitTotalList;
            }

            float minDist = 99999f;
            foreach (var character in charList)
            {
                if (!character.gameObject.activeInHierarchy || character.characterInfo.currentHP <= 0)
                    continue;

                float dist = attackEllipse.GetDistance(character.transform);
                if (dist < minDist)
                {
                    minDist = dist;
                    targetObject = character.gameObject;
                }
            }
            yield return waitTime;
        }
    }

    protected void attack(GameObject target)
    {
        lastTargetObject = target;
        lastTargetPosition = lastTargetObject.transform.position;

        // 스킬 가중치에 따라 공격

        // 가진 모든 스킬의 가중치를 더함
        int totalWeight = 0;

        usableSkills.Clear();
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].pastCoolTime >= skills[i].coolTime)
            {
                totalWeight += skills[i].weight;
                usableSkills.Add(new KeyValuePair<int, Skill>(i, skills[i]));
            }
        }

        // 범위 사이의 랜덤한 가중치를 구한다.
        int randomWeight = Random.Range(0, totalWeight);

        // 해당 가중치부분에 있는 스킬을 지정한다.
        int index = 0;
        int sumWeight = 0;
        for (int i = 0; i < usableSkills.Count; i++)
        {
            sumWeight += usableSkills[i].Value.weight;
            if (randomWeight <= sumWeight)
            {
                index = usableSkills[i].Key;
                break;
            }
        }

        skills[index].pastCoolTime = 0;
        
        beginAttack(index);
    } // 공격할 때 처리할 부분

    protected void beginAttack(int skillIndex)
    {
        currentSkill = skills[skillIndex];
        animator.SetTrigger(currentSkill.animName);
        isAttacking = true;
    }

    protected void tickAttack(int attackIndex)
    {
        if (!gameObject.activeInHierarchy || character.characterInfo.currentHP <= 0)
            return;

        if (!myHealth.IsAlive || targetObject == null || !targetObject.activeInHierarchy)
            return;

        // 사정거리 체크
        float dist = Vector3.Distance(targetObject.transform.localPosition, transform.position);
        if (dist > character.characterInfo.attackDist)
            return;

        SkillInfo skillInfo = currentSkill.skillInfos[attackIndex];

        // 기초 데미지 계산
        int charDamage = (int)character.characterInfo.damage + character.characterInfo.addedDamage;

        // 크리티컬 계산
        bool isCritical = false;

        float criticalPercent = Random.value * 100f;
        float myCriticalPercent = ArtifactManager.Instance.CalcCriticalPercent(character.characterInfo.criticalPercent);
        if (myCriticalPercent >= criticalPercent)
        {
            isCritical = true;
            charDamage = (int)(charDamage * character.characterInfo.criticalDamagePercent);
        }

        if (targetObject != null && targetObject.activeInHierarchy)
        {
            Collider2D[] colliders;
            List<GameObject> targetList = new List<GameObject>();
            switch (skillInfo.rangeAttackType)
            {
                case RangeAttackType.Cone: // 원뿔 범위
                                           // 추후 작성 일단 단일타겟으로 넘김

                case RangeAttackType.None: // 단일타겟
                    targetList.Add(targetObject);
                    break;

                case RangeAttackType.TargetCircle: // 타겟으로부터 원형범위
                    Ellipse rangeAttackElipse = new Ellipse(targetObject.transform, skillInfo.length);
                    colliders = Physics2D.OverlapCircleAll(targetObject.transform.position, skillInfo.length,
                                                                            LayerMask.GetMask(targetObject.tag));
                    foreach (var collider in colliders)
                    {
                        if (rangeAttackElipse.InEllipse(collider.gameObject.transform))
                        {
                            targetList.Add(collider.gameObject);
                        }
                    }
                    break;

                case RangeAttackType.AroundCircle: // 내 주변 원형범위
                    Ellipse aroundRangeAttackElipse = new Ellipse(this.transform, skillInfo.length);
                    colliders = Physics2D.OverlapCircleAll(this.transform.position, skillInfo.length,
                                                                            LayerMask.GetMask(targetObject.tag));
                    foreach (var collider in colliders)
                    {
                        if (aroundRangeAttackElipse.InEllipse(collider.gameObject.transform))
                        {
                            targetList.Add(collider.gameObject);
                        }
                    }
                    break;

                case RangeAttackType.Line: // 직선 사각형 범위

                    Vector3 boxEndPosition = new Vector3(skillInfo.length, skillInfo.dgree / 2, 0);
                    if (character.spriteRenderer.flipX)
                        boxEndPosition.x *= -1;

                    colliders = Physics2D.OverlapAreaAll(targetObject.transform.localPosition - new Vector3(0, skillInfo.dgree / 2, 0),
                        transform.localPosition + boxEndPosition, LayerMask.GetMask(targetObject.tag));

                    foreach (var collider in colliders)
                    {
                        targetList.Add(collider.gameObject);
                    }
                    break;
            }

            foreach (GameObject target in targetList)
            {
                targetAttack(target, skillInfo, (int)(charDamage * skillInfo.damagePercent), isCritical);
                if (skillInfo.doUseBuff)
                {
                    targetGiveBuff(target, skillInfo.buff);
                }
            }

        }
    }

    protected void magicAttack(int attackIndex)
    {
        if (!gameObject.activeInHierarchy || character.characterInfo.currentHP <= 0)
            return;

        // 사정거리 체크
        float dist = Vector3.Distance(lastTargetPosition, transform.position);
        if (dist > character.characterInfo.attackDist)
            return;

        SkillInfo skillInfo = currentSkill.skillInfos[attackIndex];

        // 기초 데미지 계산
        int charDamage = (int)character.characterInfo.damage + character.characterInfo.addedDamage;

        if (gameObject.tag == "unit")
        {
            charDamage = (int)(charDamage * Database.Instance.unitMultipleAtkDict[character.characterInfo.unitName]); // 계열 증가 데미지 계산
            charDamage = ArtifactManager.Instance.CalcAttackDamage(charDamage); // 유물 증가 데미지 계산
        }

        // 크리티컬 계산
        bool isCritical = false;

        float criticalPercent = Random.value * 100f;
        float myCriticalPercent = ArtifactManager.Instance.CalcCriticalPercent(character.characterInfo.criticalPercent);
        if (myCriticalPercent >= criticalPercent)
        {
            isCritical = true;
            charDamage = (int)(charDamage * character.characterInfo.criticalDamagePercent);
        }

        var magicObject = MasterObjectPool.Instance.GetFromPoolOrNull(skillInfo.magicName);
        Magic magic = magicObject.GetComponent<Magic>();
        magic.transform.position = transform.position;

        if (lastTargetObject == null || !lastTargetObject.activeInHierarchy)
        {
            magic.MagicAttackStart(character, lastTargetPosition, new TakeDamageInfo((int)(charDamage * skillInfo.damagePercent), isCritical, skillInfo));
        }
        else
        {
            magic.MagicAttackStart(character, lastTargetObject, new TakeDamageInfo((int)(charDamage * skillInfo.damagePercent), isCritical, skillInfo));
        }
        

        magicObject.SetActive(true);
    }

    protected void playAttackSound(int attackIndex)
    {
        if (!gameObject.activeInHierarchy || character.characterInfo.currentHP <= 0)
            return;

        sfxClip = currentSkill.skillInfos[attackIndex].sfxClip;
        audioSource.clip = sfxClip;
        audioSource.Play();
    }

    protected void endAttack()
    {
        isAttacking = false;
    }

    protected void targetAttack(GameObject target, SkillInfo skillInfo, int damage, bool isCritical)
    {

        float bloodPercent = skillInfo.bloodPercent;

        if (gameObject.tag == "unit")
        {
            damage = (int)(damage * Database.Instance.unitMultipleAtkDict[character.characterInfo.unitName]); // 계열 증가 데미지 계산
            damage = ArtifactManager.Instance.CalcAttackDamage(damage); // 유물 증가 데미지 계산
            bloodPercent = ArtifactManager.Instance.CalcBloodSucking(skillInfo.bloodPercent);
        }

        if (skillInfo.bloodSucking)
        {
            gameObject.SendMessage("takeHeal", (damage * bloodPercent));
        }

        target.SendMessage("takeAttack", new TakeDamageInfo(damage, isCritical, skillInfo));
    }

    protected void targetGiveBuff(GameObject target, Buff buff)
    {
        int charDamage = (int)character.characterInfo.damage + character.characterInfo.addedDamage;

        Buff newBuff = buff.Clone();

        // 버프 계수 계산
        switch (buff.buffType)
        {
            case BuffType.ArmorDown:
            case BuffType.ArmorUP:
            case BuffType.AttackDown:
            case BuffType.AttackUP:
            case BuffType.DotDamage:
            case BuffType.DotHeal:
                newBuff.amount *= charDamage;
                break;
        }

        BuffSystem buffSystem = target.GetComponent<BuffSystem>();
        if (buffSystem != null)
            buffSystem.AddBuff(newBuff);
    }

    protected void attackLoop()
    {
        if (!myHealth.IsAlive)
            return;

        character.characterInfo.currentDelay += Time.deltaTime * GameManager.Instance.GameSpeed * character.buffSystem.AttackSpeedAmount;
        if (!isAttacking && targetObject != null && targetObject.activeInHierarchy)
        {
            if (character.characterInfo.currentDelay >= character.characterInfo.attackDelay &&
                 attackEllipse.InEllipse(targetObject.transform))
            {
                character.CorrectSpriteDirection(targetObject);
                character.characterInfo.currentDelay = 0.0f;
                attack(targetObject);
            }
        }
    } // update에서 작동할 공격루프

    void UpdateSkillCoolTime()
    {
        foreach (var skill in skills)
        {
            if (skill.pastCoolTime < skill.coolTime)
            {
                skill.pastCoolTime += Time.deltaTime * GameManager.Instance.GameSpeed;
            }
        }
    }

    protected void OnEnable()
    {
        init();
    }

    void Update()
    {
        animator.speed = character.characterInfo.attackSpeed * GameManager.Instance.GameSpeed;

        // 일시정지 상태일때 동작시키지 않음
        if (GameManager.Instance.GameSpeed == 0)
            return;

        UpdateSkillCoolTime();
        attackLoop();           // 상대와의 거리가 공격 사거리보다 작을 때 공격시도
    }
}
