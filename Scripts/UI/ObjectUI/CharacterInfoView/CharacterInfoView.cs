using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoView : MonoBehaviour
{
    public GameObject attackRange;

    private Material original;
    public Material selected;

    int characterInfoLayer;
    public GameObject characterInfoForm;

    public Text nameText;
    public Text rarityText;

    public Text attackText;
    public Text attackSpeedText;
    public Text attackMulText;
    public Text criticalText;
    public Text hpText;
    public Text armorText;
    public Text armorMulText;

    public Slider criticalSlider;
    public Slider hpSlider;

    

    public GameObject[] stars;

    Character target;
    SpriteRenderer spriteRenderer;


    private static CharacterInfoView instance;
    public static CharacterInfoView Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CharacterInfoView>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        characterInfoLayer = LayerMask.NameToLayer("characterInfo");
        StartCoroutine(CharInfoViewUpdateCoroutine());
    }

    private void Update()
    {
        CharacterClickCheck();
    }

    IEnumerator CharInfoViewUpdateCoroutine()
    {
        while (true)
        {
            if (target != null && target.gameObject.activeInHierarchy)
            {
                SetCharacterInfo(target.characterInfo);
                Move(target.transform.position + new Vector3(65, -10, 0));
                attackRange.transform.position = target.transform.position;

                if (!characterInfoForm.activeInHierarchy)
                    characterInfoForm.SetActive(true);

                if (target.characterInfo.currentHP <= 0)
                {
                    Exit();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void SetCharacterInfo(CharacterInfo charInfo)
    {
        // 등급에 따라 마름모갯수
        int index = 0;
        for (index = 0; index <= (int)charInfo.rarity; index++)
            stars[index].SetActive(true);
        for (; index < System.Enum.GetValues(typeof(Rarity)).Length; index++)
            stars[index].SetActive(false);

        nameText.text = charInfo.printName;
        nameText.color = Database.Instance.colors[(int)charInfo.rarity];

        rarityText.text = charInfo.rarity.ToString();
        rarityText.color = Database.Instance.colors[(int)charInfo.rarity];

        attackText.text = ((int)charInfo.damage).ToString() + "(+" + charInfo.addedDamage + ")";
        attackMulText.text = ((int)(Database.Instance.unitMultipleAtkDict[charInfo.unitName]*100)) + "%";
        attackSpeedText.text = ((int)(charInfo.attackSpeed * 100)).ToString() + "(" + (charInfo.multipleAttackSpeed - 1f >= 0 ? "+" : "") + (int)((charInfo.multipleAttackSpeed - 1f) * 100) + ")%";
        criticalText.text = ((int)(charInfo.criticalPercent)).ToString() + "%";
        //criticalSlider.value = (float)charInfo.criticalPercent / 100f;

        armorText.text = ((int)charInfo.armor).ToString() + "(" + (charInfo.addedArmor >= 0 ? "+" : "") + charInfo.addedArmor + ")"; ;
        armorMulText.text = ((int)(Database.Instance.unitMultipleDefDict[charInfo.unitName] * 100)) + "%";
        hpText.text = ((int)charInfo.currentHP) + " / " + ((int)charInfo.maxHP);
        hpSlider.value = (float)charInfo.currentHP / charInfo.maxHP;


        /*
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendFormat("이름: {0}\n", charInfo.printName);
        stringBuilder.AppendFormat("공격력: {0}(+{1})\n", (int)charInfo.damage, charInfo.addedDamage);
        stringBuilder.AppendFormat("공격속도: {0}\n", charInfo.attackSpeed);
        stringBuilder.AppendFormat("치명타 확률: {0}%\n", charInfo.criticalPercent);
        stringBuilder.AppendFormat("치명타 배율: {0}%\n", charInfo.criticalDamagePercent * 100f);
        stringBuilder.AppendFormat("체력: {0}/{1}\n", (int)charInfo.currentHP, (int)charInfo.maxHP);
        stringBuilder.AppendFormat("방어력: {0}(+{1})", (int)charInfo.armor, charInfo.addedArmor);

        characterInfoText.text = stringBuilder.ToString();*/
    }

    public void HideAround(Character selectedCharacter)
    {
        foreach (var character in UnitPool.Instance.GetActiveCharacters())
        {
            if (character != selectedCharacter)
            {
                if (Vector3.Distance(selectedCharacter.transform.position, character.transform.position) < 50f)
                {
                    Color color = character.spriteRenderer.color;
                    color.a = 0.4f;
                    character.spriteRenderer.color = color;
                }
            }
        }
    }

    public void UnHideAround()
    {
        foreach (var character in UnitPool.Instance.GetActiveCharacters())
        {
            Color color = character.spriteRenderer.color;
            color.a = 1f;
            character.spriteRenderer.color = color;
        }
    }

    public void Select(Character character)
    {
        Exit();

        target = character;

        spriteRenderer = target.GetComponent<SpriteRenderer>();
        original = spriteRenderer.material;
        spriteRenderer.material = selected;

        Move(character.transform.position + new Vector3(300, -5, 0));
        
        attackRange.transform.localScale = new Vector3(character.characterInfo.attackDist * 2.15f, character.characterInfo.attackDist * 2.15f, 1);
        attackRange.transform.position = character.transform.position;
        attackRange.SetActive(true);

        HideAround(character);
    }

    void Move(Vector3 position)
    {
        position.z = 0f;
        if (position.y <= -45f)
            position.y = -45f;

        characterInfoForm.transform.position = position;
    }

    void Exit()
    {
        if (target != null)
        {
            spriteRenderer.material = original;
            target = null;
        }

        attackRange.SetActive(false);
        characterInfoForm.SetActive(false);

        UnHideAround();
    }


    public void ReturnToQuickslot()
    {
        if (target.characterInfo.currentHP > 0)
        {
            target.DisableCharacter();
            //target.gameObject.SetActive(false);

            CharacterInfo charInfo = target.characterInfo;
            Slot.Instance.AddItem(charInfo);

            Exit();
        }
    }

    void CharacterClickCheck()
    {
        Collider2D hitCol = null;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = -100;

            // 누른게 회수버튼인지 확인
            if (Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("characterReturnButton")) != null)
                return;

            Collider2D[] hitCols = Physics2D.OverlapPointAll(mousePosition);

            foreach (Collider2D col in hitCols)
            {
                if (col.gameObject.layer == characterInfoLayer)
                {
                    if (hitCol == null)
                        hitCol = col;
                }
                else if (col.gameObject.tag == "backPanel")
                {
                    Exit();
                    return;
                }
            }

            if (hitCol == null || hitCol.gameObject.layer != characterInfoLayer)
            {
                Exit();
            }
            else if (hitCol != null)
            {
                var character = hitCol.GetComponentInParent<Character>();
                if (character != null)
                {
                    Select(character);
                }
            }
        }
    }
}
