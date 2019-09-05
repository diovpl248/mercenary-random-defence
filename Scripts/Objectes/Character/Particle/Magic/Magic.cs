using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MagicCollisionType
{
    Single,
    Range,
    Penetration,
}

public enum MagicType
{
    Missile,
    Around,
    TimeAround,
}

public class Magic : MonoBehaviour
{
    // 마법의 타입들을 지정한다.
    public MagicCollisionType magicCollisionType; // 마법이 적과 충돌했을때 어떻게 처리할지
    public MagicType magicType; // 마법의 종류 (투사체, 내주변 적, 주변적 지정시간동안 공격)

    // 마법의 속성을 지정한다
    public float speed; // MagicType이 Missile일 때 투사체 속도 (초당 이동할 거리)
    public float range; // MagicCollisionType이 Range이거나 MagicType이 Around, TimeAround일 때 범위
    public float maxDist; // MagicType이 Missile일 때 투사체가 어디까지 날아가서 터질지

    private float pastTime;
    private float lastTickTime;
    public float tickTime; // MagicType이 TimeAround일 때 데미지 입힐 시간간격
    public float timeLimit; // MagicType이 TimeAround일 때 지속할 시간

    Character user;

    GameObject targetObject;
    string targetTag;

    Vector3 startPosition;
    Vector3 targetPosition;
    Vector3 normVector;

    [HideInInspector]
    TakeDamageInfo takeDamageInfo; // 적을 공격할때 데미지 입힐 스킬정보가 담겨있는곳

    public string enemyHitParticleName; // 적이 공격받을때 생길 파티클
    public string explosionParticleName; // 시간초과 or Missile이 벽에 밪을때 생길 파티클

    Rigidbody2D rigidbody2d;
    ParticleSystem ps;
    Ellipse ellipse;

    bool ready = false;

    List<Character> characters;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        ps = GetComponent<ParticleSystem>();
    }

    protected void setZOrder()
    {
        Vector3 position = transform.position;
        position.z = position.y * 1f;
        transform.position = position;
    }

    void FixedUpdate()
    {
        if (!ready)
            return;

        // Z오더
        setZOrder();

        var main = ps.main;
        main.simulationSpeed = GameManager.Instance.GameSpeed;

        switch(magicType)
        {
            case MagicType.Missile:
                Vector3 delta = speed * normVector * Time.deltaTime * GameManager.Instance.GameSpeed;

                rigidbody2d.velocity = delta;

                // 범위벗어나면 바로 터지게함
                if (Vector3.Distance(transform.position, startPosition) >= maxDist)
                {
                    createExplosionParticle(transform.position);
                    // Missile-Range 타입일 경우 그자리 주변에 있는 적들에게도 피해를 준다.
                    if (magicCollisionType == MagicCollisionType.Range)
                    {
                        explode(gameObject);
                    }
                    gameObject.SetActive(false);
                    return;
                }

                transform.Translate(delta);
                break;
            case MagicType.Around:
                // range범위에 있는 적에게 데미지를 입힙
                ellipse = new Ellipse(transform, range);

                InEllipseAttack(ellipse);
                
                ready = false;
                StartCoroutine(timeOutDisableCoroutine(1.5f));
                //gameObject.SetActive(false);
                break;
            case MagicType.TimeAround:
                pastTime += Time.deltaTime * GameManager.Instance.GameSpeed;
                if (pastTime >= timeLimit)
                {
                    gameObject.SetActive(false);
                    break;
                }

                if (pastTime < lastTickTime + tickTime)
                    break;

                lastTickTime = pastTime;

                ellipse = new Ellipse(transform, range);

                InEllipseAttack(ellipse);

                break;
        }
    }

    public void InEllipseAttack(Ellipse ellipse)
    {
        if (targetTag == "enemy")
            characters = EnemyPool.Instance.GetActiveCharacters();
        else
            characters = UnitPool.Instance.GetActiveCharacters();

        foreach (var character in characters)
        {
            if (character.characterInfo.currentHP <= 0)
                continue;

            if (ellipse.InEllipse(character.transform))
            {
                //createEnemyHitParticle(character.transform.position);
                EnemyAttack(character);
            }
        }
    }

    void EnemyAttack(Character character)
    {
        if(takeDamageInfo.skillInfo.doUseBuff)
        {
            Buff newBuff = takeDamageInfo.skillInfo.buff.Clone();

            // 버프 계수 계산
            switch (newBuff.buffType)
            {
                case BuffType.ArmorDown:
                case BuffType.ArmorUP:
                case BuffType.AttackDown:
                case BuffType.AttackUP:
                case BuffType.DotDamage:
                case BuffType.DotHeal:
                    newBuff.amount *= takeDamageInfo.damage;
                    break;
            }

            if(character.buffSystem != null)
                character.buffSystem.AddBuff(newBuff);
        }

        float bloodPercent = takeDamageInfo.skillInfo.bloodPercent;

        if (targetTag != "unit")
        {
            bloodPercent = ArtifactManager.Instance.CalcBloodSucking(bloodPercent);
        }

        if (takeDamageInfo.skillInfo.bloodSucking)
        {
            if(user.gameObject.activeInHierarchy && user.characterInfo.currentHP > 0)
                user.SendMessage("takeHeal", (takeDamageInfo.damage * bloodPercent));
        }

        character.SendMessage("takeAttack", takeDamageInfo);
    }

    void EnemyAttack(GameObject gameObject)
    {
        if (takeDamageInfo.skillInfo.doUseBuff)
        {
            Buff newBuff = takeDamageInfo.skillInfo.buff.Clone();

            // 버프 계수 계산
            switch (newBuff.buffType)
            {
                case BuffType.ArmorDown:
                case BuffType.ArmorUP:
                case BuffType.AttackDown:
                case BuffType.AttackUP:
                case BuffType.DotDamage:
                case BuffType.DotHeal:
                    newBuff.amount *= takeDamageInfo.damage;
                    break;
            }

            BuffSystem buffSystem = gameObject.GetComponent<BuffSystem>();
            if (buffSystem != null)
                buffSystem.AddBuff(newBuff);
        }

        float bloodPercent = takeDamageInfo.skillInfo.bloodPercent;

        if (targetTag != "unit")
        {
            bloodPercent = ArtifactManager.Instance.CalcBloodSucking(bloodPercent);
        }

        if (takeDamageInfo.skillInfo.bloodSucking)
        {
            if (user.gameObject.activeInHierarchy && user.characterInfo.currentHP > 0)
                user.SendMessage("takeHeal", (takeDamageInfo.damage * bloodPercent));
        }

        gameObject.SendMessage("takeAttack", takeDamageInfo);
    }

    private void init()
    {
        pastTime = 0;
        lastTickTime = 0;

        ready = false;

        rigidbody2d.velocity = Vector3.zero;
        startPosition = Vector3.zero;
        targetPosition = Vector3.zero;
        normVector = Vector3.zero;
    }

    // 캐릭터의 Attack.cs 에서 호출하는 함수
    public void MagicAttackStart(Character user, GameObject target, TakeDamageInfo takeDamageInfo)
    {
        init();

        this.user = user;

        targetObject = target;
        targetTag = target.tag;

        startPosition = transform.position;
        targetPosition = target.transform.position;
        this.takeDamageInfo = takeDamageInfo;

        var particle = GetComponent<ParticleSystem>().shape;
        var rotation = particle.rotation;

        if(startPosition.x >= targetPosition.x)
        {
            rotation.y = 180;
            particle.rotation = rotation;
        }
        else
        {
            rotation.y = 0;
            particle.rotation = rotation;
        }
        normVector = Vector3.Normalize(targetPosition - gameObject.transform.position);

        ready = true;
    }

    public void MagicAttackStart(Character user, Vector3 targetPosition, TakeDamageInfo takeDamageInfo)
    {
        init();

        this.user = user;

        if(user.tag == "unit")
        {
            targetTag = "enemy";
        }
        else
        {
            targetTag = "unit";
        }

        targetObject = null;
        startPosition = transform.position;
        this.targetPosition = targetPosition;
        this.takeDamageInfo = takeDamageInfo;

        var particle = GetComponent<ParticleSystem>().shape;
        var rotation = particle.rotation;

        if (startPosition.x >= targetPosition.x)
        {
            rotation.y = 180;
            particle.rotation = rotation;
        }
        else
        {
            rotation.y = 0;
            particle.rotation = rotation;
        }
        normVector = Vector3.Normalize(this.targetPosition - gameObject.transform.position);

        ready = true;
    }

    void createEnemyHitParticle(Vector3 explosionPosition)
    {
        var go = MasterObjectPool.Instance.GetFromPoolOrNull(enemyHitParticleName);
        go.transform.position = explosionPosition;
        go.SetActive(true);
    }

    void createExplosionParticle(Vector3 explosionPosition)
    {
        var go = MasterObjectPool.Instance.GetFromPoolOrNull(explosionParticleName);
        go.transform.position = explosionPosition;
        go.SetActive(true);
    }

    void explode(GameObject coliisionObject)
    {
        switch (magicCollisionType)
        {
            case MagicCollisionType.Single:
                if(coliisionObject != null && coliisionObject.tag == targetTag)
                {
                    EnemyAttack(coliisionObject);
                }
                ready = false;
                gameObject.SetActive(false);
                break;

            case MagicCollisionType.Range:
                ellipse = new Ellipse(transform, range);
                InEllipseAttack(ellipse);
                ready = false;
                gameObject.SetActive(false);
                break;

            case MagicCollisionType.Penetration:
                if (coliisionObject != null && coliisionObject.tag == targetTag)
                {
                    EnemyAttack(coliisionObject);
                }
                break;
        }
    }

    IEnumerator timeOutDisableCoroutine(float time)
    {
        for(;time >= 0; time-=Time.deltaTime*GameManager.Instance.GameSpeed)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (magicType != MagicType.Missile)
            return;

        if (collision.gameObject.tag == targetTag || collision.gameObject.tag == "wall")
        {
            createExplosionParticle(transform.position);
            explode(collision.gameObject);
            return;
        }
    }
}
