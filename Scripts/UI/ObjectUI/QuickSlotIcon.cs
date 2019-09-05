using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotIcon : MonoBehaviour
{
    public UnitName unitName;
    public CharacterInfo characterInfo;

    public void ShowInfoInCombinationList(Text text)
    {
        string infoText = "이름: " + characterInfo.characterName +
              "  등급: " + characterInfo.rarity.ToString() + "\n" +
              "공격력: " + (int)characterInfo.damage +
              "  공격속도: " + characterInfo.attackSpeed +
              "  체력: " + (int)characterInfo.currentHP + "/" + (int)characterInfo.maxHP +
              "  방어력: " + (int)characterInfo.armor;
        text.text = infoText;
    }
}
