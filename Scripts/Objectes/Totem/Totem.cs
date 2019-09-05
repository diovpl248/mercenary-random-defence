using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class TotemDataset
{
    public string totemName;
    public Sprite sprite;
    public float radius;

    public float durationTime;
    public float pastingTime;

    public bool affectMyUnit;
    public bool affectEnemy;

    public Buff buff;

    public TotemDataset(string totemName, Sprite sprite, float radius, float durationTime, float pastingTime, bool affectMyUnit, bool affectEnemy, Buff buff)
    {
        this.totemName = totemName;
        this.sprite = sprite;
        this.radius = radius;
        this.durationTime = durationTime;
        this.pastingTime = pastingTime;
        this.affectMyUnit = affectMyUnit;
        this.affectEnemy = affectEnemy;
        this.buff = buff;
    }

    public TotemDataset Clone()
    {
        TotemDataset totemDataset = new TotemDataset(totemName, sprite, radius, durationTime, pastingTime
            , affectMyUnit, affectEnemy, buff.Clone());
        return totemDataset;
    }
}

public class Totem : MonoBehaviour
{
    Animator animator;

    public TotemDataset totemDataset;

    public TextMeshProUGUI timeText;
    public SpriteRenderer totemSpriteRenderer;

    public SpriteRenderer bottomCircle;
    public SpriteRenderer rangeCircle;

    public Color positiveColor;
    public Color negativeColor;

    bool placementCheck = false;

    Ellipse ellipse;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(TotemDataset totemDataset)
    {
        StopAllCoroutines();
        this.totemDataset = totemDataset;

        placementCheck = false;

        totemSpriteRenderer.sprite = totemDataset.sprite;

        ellipse = new Ellipse(transform, totemDataset.radius);
        rangeCircle.transform.localScale = new Vector3(totemDataset.radius / 2.5f, totemDataset.radius / 2.5f, 1);

        if (totemDataset.buff.isPositive)
        {
            Color color = positiveColor;
            bottomCircle.color = color;
            color.a = rangeCircle.color.a;
            rangeCircle.color = color;
        }
        else
        {
            Color color = negativeColor;
            bottomCircle.color = color;
            color.a = rangeCircle.color.a;
            rangeCircle.color = color;
        }

        gameObject.SetActive(true);
        /*
        if (totemDataset.buff.durationTime == 0)
            StartCoroutine(totemPassiveCoroutine());
        else
            StartCoroutine(totemActiveCoroutine());
        */
    }

    protected void setZOrder()
    {
        Vector3 position = transform.position;
        position.z = position.y * 1f;
        transform.position = position;
    }// 이미지들간의 위에그려질 우선순위 설정

    public IEnumerator totemActiveCoroutine()
    {
        while (true)
        {
            if (ellipse.Radius != totemDataset.radius)
            {
                ellipse.SetRadius(totemDataset.radius);
                rangeCircle.transform.localScale = new Vector3(totemDataset.radius / 2.5f, totemDataset.radius / 2.5f, 1);
            }

            List<Character> unitList = null;
            List<Character> enemyList = null;

            if (totemDataset.affectMyUnit)
                unitList = UnitPool.Instance.GetActiveCharacters();
            if (totemDataset.affectEnemy)
                enemyList = EnemyPool.Instance.GetActiveCharacters();

            if (unitList != null)
            {
                foreach (var character in unitList)
                {
                    if (ellipse.InEllipse(character.transform))
                    {
                        if (character.buffSystem != null)
                            character.buffSystem.AddBuff(totemDataset.buff.Clone());
                    }
                }
            }

            if (enemyList != null)
            {
                foreach (var character in enemyList)
                {
                    if (ellipse.InEllipse(character.transform))
                    {
                        if (character.buffSystem != null)
                            character.buffSystem.AddBuff(totemDataset.buff.Clone());
                    }
                }
            }

            for (float time = totemDataset.buff.durationTime + 0.01f; time > 0; time -= Time.deltaTime * GameManager.Instance.GameSpeed)
            {
                yield return null;
            }
        }
    }

    // 최적화가 요구된다
    public IEnumerator totemPassiveCoroutine()
    {
        while (true)
        {
            if (ellipse.Radius != totemDataset.radius)
                ellipse.SetRadius(totemDataset.radius);

            List<Character> unitList = null;
            List<Character> enemyList = null;

            if (totemDataset.affectMyUnit)
                unitList = UnitPool.Instance.GetActiveCharacters();
            if (totemDataset.affectEnemy)
                enemyList = EnemyPool.Instance.GetActiveCharacters();

            if (unitList != null)
            {
                foreach (var character in unitList)
                {
                    if (ellipse.InEllipse(character.transform))
                    {
                        if (character.buffSystem == null)
                            continue;

                        if (ellipse.InEllipse(character.transform))
                            character.buffSystem.AddBuff(totemDataset.buff);
                        else
                            character.buffSystem.DeleteBuff(totemDataset.buff);
                    }
                }
            }

            if (enemyList != null)
            {
                foreach (var character in enemyList)
                {
                    if (ellipse.InEllipse(character.transform))
                    {
                        if (character.buffSystem == null)
                            continue;

                        if (ellipse.InEllipse(character.transform))
                            character.buffSystem.AddBuff(totemDataset.buff);
                        else
                            character.buffSystem.DeleteBuff(totemDataset.buff);
                    }
                }
            }

            for (float time = 0.2f; time > 0; time -= Time.deltaTime * GameManager.Instance.GameSpeed)
            {
                yield return null;
            }
        }
    }

    private void Update()
    {
        if (!placementCheck)
            return;

        animator.speed = GameManager.Instance.GameSpeed;
        setZOrder();

        totemDataset.pastingTime += Time.deltaTime * GameManager.Instance.GameSpeed;

        float remainTime = totemDataset.durationTime - totemDataset.pastingTime;
        if (remainTime < 0)
            remainTime = 0;

        int min = (int)(remainTime / 60);
        int sec = (int)(remainTime % 60);

        timeText.text = min.ToString("00") + ":" + sec.ToString("00");

        if (totemDataset.pastingTime >= totemDataset.durationTime)
        {
            StopAllCoroutines();
            animator.SetTrigger("Destroy");
        }
    }

    private void DestroyTotem()
    {
        List<Character> unitList = null;
        List<Character> enemyList = null;

        if (totemDataset.affectMyUnit)
            unitList = UnitPool.Instance.GetActiveCharacters();
        if (totemDataset.affectEnemy)
            enemyList = EnemyPool.Instance.GetActiveCharacters();

        if (unitList != null)
        {
            foreach (var character in unitList)
            {
                if (ellipse.InEllipse(character.transform))
                {
                    if (character.buffSystem != null)
                        character.buffSystem.DeleteBuff(totemDataset.buff);
                }
            }
        }

        if (enemyList != null)
        {
            foreach (var character in enemyList)
            {
                if (ellipse.InEllipse(character.transform))
                {
                    if (character.buffSystem != null)
                        character.buffSystem.DeleteBuff(totemDataset.buff);
                }
            }
        }

        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    private void OnDisable()
    {
        TotemManager.Instance.totems.Remove(this);
        StopAllCoroutines();
    }

    public void Cancel()
    {
        SpellDragAndDrop.Instance.totemInfo.gameObject.SetActive(false);
        transform.Find("Canvas/YesButton").gameObject.SetActive(false);
        transform.Find("Canvas/NoButton").gameObject.SetActive(false);
        gameObject.SetActive(false);
        TotemManager.Instance.isDragging = false;
    }

    public void confirm()
    {
        SpellDragAndDrop.Instance.totemInfo.gameObject.SetActive(false);
        TotemManager.Instance.PlacementSuccess();

        foreach (Image cool in TotemManager.Instance.cooltimeImage)
        {
            cool.fillAmount = 1;
        }
        TotemManager.Instance.isDragging = false;
        animator.SetTrigger("Init");
        transform.Find("Totem").gameObject.SetActive(true);
        transform.Find("Canvas/Time").gameObject.SetActive(true);
        if (totemDataset.buff.durationTime == 0)
            StartCoroutine(totemPassiveCoroutine());
        else
            StartCoroutine(totemActiveCoroutine());

        totemDataset.pastingTime = 0;

        transform.Find("Canvas/YesButton").gameObject.SetActive(false);
        transform.Find("Canvas/NoButton").gameObject.SetActive(false);
        placementCheck = true;

        TotemManager.Instance.totems.Add(this);
        QuestEventManager.Instance.ReceiveEvent(null, QuestEvent.Totem, 1);
    }

    public void LoadSavedTotem()
    {
        animator.SetTrigger("Init");

        transform.Find("Canvas/YesButton").gameObject.SetActive(false);
        transform.Find("Canvas/NoButton").gameObject.SetActive(false);

        transform.Find("Totem").gameObject.SetActive(true);
        transform.Find("Canvas/Time").gameObject.SetActive(true);
        
        if (totemDataset.buff.durationTime == 0)
            StartCoroutine(totemPassiveCoroutine());
        else
            StartCoroutine(totemActiveCoroutine());

        placementCheck = true;

        TotemManager.Instance.totems.Add(this);
    }
}