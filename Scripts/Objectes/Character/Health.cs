using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct TakeDamageInfo
{
    public int damage;
    public bool isCritical;
    public SkillInfo skillInfo;

    public TakeDamageInfo(int damage, bool isCritical, SkillInfo skillInfo)
    {
        this.damage = damage;
        this.isCritical = isCritical;
        this.skillInfo = skillInfo;
    }
}

public class Health : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected AudioSource audioSource;

    protected Character character;

    protected BoxCollider2D boxCollider;


    public AudioClip dieClip;

    //public GameObject incomingGold;
    public IncomeGoldPool IncomeP;
    private FloatingPool floatingP;

    Shader shaderGUI;
    Shader shaderSprite;

    protected bool isAlive = true;
    public bool IsAlive { get { return isAlive; } }

    public string animName_hurt;
    public string animName_die;

    PrefixParticle[] prefixParticle;

    static TMP_ColorGradient damageColor;
    static TMP_ColorGradient criticalDamageColor;
    static TMP_ColorGradient dotDamageColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<Character>();
        boxCollider = GetComponent<BoxCollider2D>();
        IncomeP = GetComponent<IncomeGoldPool>();
        floatingP = GetComponent<FloatingPool>();

        shaderGUI = Shader.Find("GUI/Text Shader");
        init();
    }

    virtual protected void init()
    {
        shaderSprite = GetComponent<SpriteRenderer>().material.shader;

        boxCollider.enabled = true;

        isAlive = true;
    } // 초기화할 부분

    private void Start()
    {
        if (damageColor == null)
        {
            damageColor = Database.Instance.GetGradient(GradientName.DamageGradient.ToString()); ;
            criticalDamageColor = Database.Instance.GetGradient(GradientName.CriticalDamageGradient.ToString());
            dotDamageColor = Database.Instance.GetGradient(GradientName.DotDamageGradient.ToString());
        }
    }

    protected void takeHeal(int amount)
    {
        if (character.buffSystem.havingDotDamanage)
            amount /= 2;

       if (amount <= 0)
            return;

        var floatingTextTMP = GetFloatingTextBasis(amount);
        floatingTextTMP.colorGradientPreset = Database.Instance.GetGradient(GradientName.HealGradient.ToString());
        floatingTextTMP.gameObject.SetActive(true);
        
        character.characterInfo.currentHP += amount;
        if (character.characterInfo.currentHP > character.characterInfo.maxHP)
            character.characterInfo.currentHP = character.characterInfo.maxHP;
        return;
    }

    protected void takeAttack(TakeDamageInfo info)
    {
        if (!isAlive)
            return;

        int damage = info.damage;

        // 방어력 수식 변경 방어력 200일때 데미지 50%감소, 400일때 66.6% 감소
        int armor = (int)(character.characterInfo.armor + character.characterInfo.addedArmor);

        if (gameObject.tag == "unit")
        {
            // 계열 방어력 배율
            armor = (int)(armor * Database.Instance.unitMultipleDefDict[character.characterInfo.unitName]);
        }


            if (armor <= 0)
            armor = 0;

        damage = (int)(info.damage * 200 / (200 + armor));
        
        if (gameObject.tag == "unit")
        {
            

            // 유물 데미지 감소 계산
            damage = ArtifactManager.Instance.CalcTakeDamage(damage);
        }
        

        if (damage < 1)
            damage = 1;

        var floatingTextTMP = GetFloatingTextBasis(damage);//GetFloatingTextBasis(damage);
        
        switch (info.skillInfo.damageType)
        {
            // 마법데미지 추후작성 일단 노말이랑 같이 처리
            case DamageType.MagicDamage:
            case DamageType.Normal:
                if (!info.isCritical)
                {
                    floatingTextTMP.colorGradientPreset = damageColor;
                }
                else
                {
                    floatingTextTMP.colorGradientPreset = criticalDamageColor;
                }
                break;

            case DamageType.DotDamage:
                floatingTextTMP.colorGradientPreset = dotDamageColor;
                floatingTextTMP.fontSize = 50f;
                break;

            default:
                break;
        }

        floatingTextTMP.gameObject.SetActive(true);
        
        float remainHP = character.characterInfo.currentHP - damage;

        if (remainHP <= 0)
        {
            character.characterInfo.currentHP = 0;
            die();
        }
        else
        {
            character.characterInfo.currentHP = remainHP;
            /*
            if (Animator.StringToHash("Base Layer.Idle") == animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
            {
                animator.SetTrigger(animName_hurt);
            }*/
            StopAllCoroutines();
            StartCoroutine(takeAttackColorChangeCoroutine());
        }
    }// 공격맞을 때 처리할부분

    protected IEnumerator takeAttackColorChangeCoroutine()
    {
        spriteRenderer.material.shader = shaderGUI;
        for (float time = 0.1f; time >= 0; time -= Time.deltaTime * GameManager.Instance.GameSpeed)
        {
            yield return null;
        }

        spriteRenderer.material.shader = shaderSprite;
    }

    TextMeshPro GetFloatingTextBasis(int damage)
    {
        var temp = MasterFloatingTextPool.Instance.GetFromPool();//floatingP.GetFromPool();
        temp.transform.position = transform.localPosition + new Vector3(Random.Range(-2f, 2f), 30, 0);
        temp.text = damage.ToString();
        temp.fontSize = 80f;

        return temp;
    }

    protected void die()
    {
        if (gameObject.tag == "enemy")
        {
            int gettingGold = ArtifactManager.Instance.CalcGettingGold(character.characterInfo.gold);

            Database.Instance.gold += gettingGold;

            // 골드획득 임시수정
            var temp = MasterFloatingTextPool.Instance.GetFromPool();//IncomeP.GetFromPool();
            temp.colorGradientPreset = Database.Instance.GetGradient(GradientName.IncomeGoldGradient.ToString());
            temp.transform.position = transform.localPosition;
            temp.text = "+ " + gettingGold.ToString() + " G";
            temp.fontSize = 70f;
            temp.gameObject.SetActive(true);
           
            QuestEventManager.Instance.ReceiveEvent(gameObject, QuestEvent.EnemyDie, 1);

            if(character.characterInfo.characterName == "그리핀"){
                // 보스는 유물을 준다
                ArtifactSelect.Instance.ShowArtifactSelect();

                QuestEventManager.Instance.ReceiveEvent(null, QuestEvent.BossDie, 1);
            }
            else if(character.characterInfo.characterName == "다크그리핀")
            {
                GameManager.Instance.GameClear();

                QuestEventManager.Instance.ReceiveEvent(null, QuestEvent.BossDie, 1);
            }
        }
        else if (gameObject.tag == "unit")
        {
            QuestEventManager.Instance.ReceiveEvent(gameObject, QuestEvent.MyUnitDie, 1);
        }


        boxCollider.enabled = false;
        audioSource.clip = dieClip;
        audioSource.PlayDelayed(0.1f);
        isAlive = false;

        animator.Rebind();
        animator.SetTrigger(animName_die);

        StartCoroutine(dieCoroutine());
    } // 죽을 때 처리하기


    IEnumerator dieCoroutine()
    {
        for (float time = 2f; time >= 0; time -= Time.deltaTime * GameManager.Instance.GameSpeed)
        {
            yield return null;
        }

        audioSource.Stop();
        character.DisableCharacter();
    }

    private void OnEnable()
    {
        init();
    }

    private void Update()
    {
        int heal, damage;

        if (character.buffSystem == null)
            return;

        if (character.characterInfo.currentHP <= 0)
            return;

        character.buffSystem.TickBuffsForHalth(out heal, out damage);

        if (heal > 0)
            takeHeal(heal);
        if (damage > 0)
        {
            SkillInfo skillInfo = new SkillInfo();
            skillInfo.damageType = DamageType.DotDamage;
            takeAttack(new TakeDamageInfo(damage, false, skillInfo));
        }
    }
}
