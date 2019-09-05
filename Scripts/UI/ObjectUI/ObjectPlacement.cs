using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectPlacement : MonoBehaviour
{
    private static ObjectPlacement instance;
    public static ObjectPlacement Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ObjectPlacement>();
            return instance;
        }
    }

    //public GameObject charPrefab;
	public GameObject greenCircle;
	public GameObject redCircle;
    //public GameObject newObject;
    public GameObject unitInfo;
    public GameObject scrollView;
    public GameObject attackRange;
    /*
    private Text unitInfoText;
    private CharacterInfo newObjectInfo;
    private GameObject target;
	private BoxCollider2D newObjectColider;
    private Vector3 mousePosition;
    private Slot slot;
    private QuickSlotIcon icon;
    private CanvasGroup canvasGroup;
    bool draging = false;
	bool impossible = false;
    bool circleOn = false;//원을 켜고 끄는 용도*/
    /*
	private void Awake()
	{
        slot = new Slot();
        canvasGroup = GetComponent<CanvasGroup>();
        init();
	}

	private void init()
	{
		mousePosition = Vector3.zero;

		newObject = null;

		impossible = false;
		draging = false;
        unitInfo.SetActive(false);
		greenCircle.SetActive(false);
		redCircle.SetActive(false);
        scrollView.gameObject.GetComponent<ScrollRect>().horizontal = true;
    }
    
    void Update()
	{
        Collider2D hitCol = null;
        Collider2D hitQuickSlot = null;
        Collider2D hitScrollView = null;
        Collider2D hitLuckyBox = null;

        if (Input.GetMouseButtonDown(0))
		{
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            hitCol = Physics2D.OverlapPoint(mousePosition,LayerMask.GetMask("quickSlot"));
            hitLuckyBox = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("gacha"));
            if (hitCol != null && hitCol.tag == "quickSlot" && hitLuckyBox == null)
			{
                target = hitCol.gameObject;
                Slot.Instance.OnClickIcon(target);
                icon = hitCol.gameObject.GetComponent<QuickSlotIcon>();
                newObject = UnitPool.Instance.GetFromPool(icon.unitName, icon.characterInfo);
				newObjectColider = newObject.GetComponent<BoxCollider2D>();
                newObjectInfo = newObject.GetComponent<Character>().characterInfo;
                unitInfoText = unitInfo.GetComponentInChildren<Text>();
                icon.ShowInfo(unitInfoText);
                draging = true;
            }
        }
		else if (draging)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            newObject.transform.position = mousePosition;
			greenCircle.transform.position = mousePosition;
			redCircle.transform.position = mousePosition;
			int mask = 1 << LayerMask.NameToLayer("unit");
			hitCol = Physics2D.OverlapCircle(mousePosition, 1.0f, mask);
            hitQuickSlot = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("quickSlot"));
            hitScrollView = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("scrollView"));
            if (hitScrollView != null && hitScrollView.tag != "scrollView")
                scrollView.gameObject.GetComponent<ScrollRect>().horizontal = true;
            else
                scrollView.gameObject.GetComponent<ScrollRect>().horizontal = false;
            if (hitQuickSlot != null && hitQuickSlot.tag == "quickSlot")
            {
                unitInfo.transform.position =  new Vector3(target.transform.position.x + 45, target.transform.position.y + 35, 0);
                unitInfo.SetActive(true);
                circleOn = false;
            }
            else if(hitScrollView == null)
            {
                unitInfo.SetActive(false);
                circleOn = true;
            }
			if (hitCol != null && circleOn) // 일정범위안에 유닛이 있다면
			{
				impossible = true;
				greenCircle.SetActive(false);
				redCircle.SetActive(true);
			}
			else if(circleOn)
			{
				impossible = false;
				greenCircle.SetActive(true);
				redCircle.SetActive(false);
			}
            else
            {
                impossible = true;
                greenCircle.SetActive(false);
                redCircle.SetActive(false);
            }
		}

        if (Input.GetMouseButtonUp(0)) //Drop
        {
            if(impossible)
                Slot.Instance.EndClickIcon(target);
            if (!impossible && draging)
            {
                CharacterInfoScrollView.Instance.AddToList(newObjectInfo.characterName, newObject);
                newObject.SetActive(true);
                Slot.Instance.DeleteItem(target);
            }              
            init();
        }
    }*/
}