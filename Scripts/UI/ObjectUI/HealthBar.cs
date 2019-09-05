using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour {

    public Canvas canvas;
    public Character character;
	public Slider slider;
	public Text nameText;
    public TextMeshProUGUI hpText;

	void Start () {
        canvas = GetComponent<Canvas>();
        character = transform.parent.gameObject.GetComponent<Character>();
		slider = GetComponentInChildren<Slider>();
		nameText = GetComponentInChildren<Text>();
        hpText = slider.gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        setZOrder();
        SetName(character.characterInfo.printName);
        SetHPBar(character.characterInfo.currentHP, character.characterInfo.maxHP);
    }

    protected void setZOrder()
    {
        Vector3 position = transform.position;
        position.z = position.y * 1f;
        transform.position = position;
        //canvas.sortingOrder = (int)transform.position.y * -1;
    }// 이미지들간의 위에그려질 우선순위 설정

    public void SetHPBar(float current, float max)
	{
		slider.maxValue = max;
		slider.value = current;
        hpText.text = ((int)current).ToString();
    }

	public void SetName(string name)
	{
		nameText.text = name;
    }
}
