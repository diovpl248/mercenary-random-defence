using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellDragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{

    private GameObject go;
    private EnemySpawnManager spawnManger;
    private TotemManager totem;
    private TotemDataset tempTotemDataset;

    public GameObject totemInfo;

    public Text totemName;
    public Text durationTime;
    public Text range;
    public Text desc;

    bool isDragging;
    bool isUsable = false;
    bool impossible;
    bool dragCheck;

    private Vector3 position;

    private static SpellDragAndDrop instance = null;
    public static SpellDragAndDrop Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SpellDragAndDrop>();
            }
            return instance;
        }
    }

    public void Awake()
    {
        totem = TotemManager.Instance;
        spawnManger = EnemySpawnManager.Instance;

        impossible = false;
        dragCheck = false;
    }

    public void SetTotemInfo()
    {
        switch(name)
        {
            case "buff_dotheal":
                totemName.text = "힐 토템";
                durationTime.text = ((int)tempTotemDataset.durationTime).ToString() + "초";
                range.text = ((int)tempTotemDataset.durationTime).ToString();
                desc.text = "아군의 체력이 " + tempTotemDataset.buff.tickTime.ToString("0.00") + "초 마다 " + ((int)tempTotemDataset.buff.amount) + " 회복합니다.";
                break;

            case "buff_attackup":
                totemName.text = "공격력 토템";
                durationTime.text = ((int)tempTotemDataset.durationTime).ToString() + "초";
                range.text = ((int)tempTotemDataset.durationTime).ToString();
                desc.text = "아군의 공격력이 " + (int)tempTotemDataset.buff.amount + "증가합니다.";
                break;

            case "buff_dotdamage":
                totemName.text = "지속 피해 토템";
                durationTime.text = ((int)tempTotemDataset.durationTime).ToString() + "초";
                range.text = ((int)tempTotemDataset.durationTime).ToString();
                desc.text = "적군에게 " + tempTotemDataset.buff.tickTime.ToString("0.00") + "초 마다 " + ((int)tempTotemDataset.buff.amount) + "만큼 데미지를 입힙니다.";
                break;

            case "buff_speeddown":
                totemName.text = "슬로우 토템";
                durationTime.text = ((int)tempTotemDataset.durationTime).ToString() + "초";
                range.text = ((int)tempTotemDataset.durationTime).ToString();
                desc.text = "적군의 이동속도를 " + tempTotemDataset.buff.amount*100 + "% 감소시킵니다.";
                break;

            case "buff_defendup":
                totemName.text = "방어력 토템";
                durationTime.text = ((int)tempTotemDataset.durationTime).ToString() + "초";
                range.text = ((int)tempTotemDataset.durationTime).ToString();
                desc.text = "아군의 방어력이 " + ((int)tempTotemDataset.buff.amount) + "증가합니다.";
                break;
            }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isUsable = TotemManager.Instance.isUsable;
        isDragging = TotemManager.Instance.isDragging;
        if (!isUsable || isDragging)
            return;

        impossible = false;

        TotemManager.Instance.isDragging = true;

        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        totemInfo.gameObject.SetActive(true);
        totemInfo.transform.position = gameObject.transform.position;
        dragCheck = true;

        go.gameObject.SetActive(true);
        go.transform.position = position;
        go.transform.Find("Totem").gameObject.SetActive(false);
        go.transform.Find("Canvas/Time").gameObject.SetActive(false);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!isUsable || isDragging)
            return;

        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        go.transform.position = position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isUsable || isDragging)
            return;

        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int mask = 1 << LayerMask.NameToLayer("wall") | 1 << LayerMask.NameToLayer("scrollView") | 1 << LayerMask.NameToLayer("placementUI");

        var collider = Physics2D.OverlapPoint(position, mask);
        if (collider != null)
        {
            impossible = true;
        }

        if (impossible)
        {
            Notice.Instance.ShowNotice("토템을 설치할 수 없는 위치입니다.");
            TotemManager.Instance.isDragging = false;
            totemInfo.gameObject.SetActive(false);
            go.gameObject.SetActive(false);
        }
        else
        {
            go.transform.Find("Canvas/YesButton").gameObject.SetActive(true);
            go.transform.Find("Canvas/NoButton").gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        totemInfo.gameObject.SetActive(true);
        totemInfo.transform.position = gameObject.transform.position;
        dragCheck = false;

        if (name == "buff_dotheal")
        {

            go = totem.CreateTotemOrNull("도트힐토템1");
            go.gameObject.SetActive(false);
            tempTotemDataset = go.GetComponent<Totem>().totemDataset;
            var orgAmount = tempTotemDataset.buff.orgAmount;
            tempTotemDataset.buff.amount = Mathf.Pow(orgAmount+ spawnManger.stage, 1+ spawnManger.stage  * 0.01f); // 임시 수식;
        }
        else if (name == "buff_attackup")
        {

            go = totem.CreateTotemOrNull("공격력토템1");
            go.gameObject.SetActive(false);
            tempTotemDataset = go.GetComponent<Totem>().totemDataset;
            var orgAmount = tempTotemDataset.buff.orgAmount;
            tempTotemDataset.buff.amount = Mathf.Pow(orgAmount + spawnManger.stage/2, 1 + spawnManger.stage * 0.01f); // 임시 수식;

        }
        else if (name == "buff_dotdamage")
        {

            go = totem.CreateTotemOrNull("도트데미지토템1");
            go.transform.position = position;
            go.gameObject.SetActive(false);
            tempTotemDataset = go.GetComponent<Totem>().totemDataset;
            var orgAmount = tempTotemDataset.buff.orgAmount;
            tempTotemDataset.buff.amount = Mathf.Pow(orgAmount + spawnManger.stage * 5, 1 + spawnManger.stage * 0.01f); // 임시 수식;

        }
        else if (name == "buff_speeddown")
        {

            go = totem.CreateTotemOrNull("스피드저하토템1");
            go.transform.position = position;
            go.gameObject.SetActive(false);
            tempTotemDataset = go.GetComponent<Totem>().totemDataset;
            // 스피드 저하는 0.5 = 속도 50%저하 이런식이므로 다른것과는 아예 다른방식으로 접근하시길
            //go.GetComponent<Totem>().totemDataset.buff.amount = Mathf.Floor(Mathf.Pow(go.GetComponent<Totem>().totemDataset.buff.amount, 1.0f + 0.01f * (SpawnManger.stage - 1))); // 임시 수식;
            //tempTotemDataset.buff.amount += ((spawnManger.stage-1) / 100f); // 임시 수식;
            var orgAmount = tempTotemDataset.buff.orgAmount + spawnManger.stage/2 * 0.009f;
            tempTotemDataset.buff.amount = orgAmount; // 임시 수식;
        }
        else if (name == "buff_defendup")
        {
            go = totem.CreateTotemOrNull("방어력토템1");
            go.transform.position = position;
            go.gameObject.SetActive(false);
            tempTotemDataset = go.GetComponent<Totem>().totemDataset;
            var orgAmount = tempTotemDataset.buff.orgAmount;
            tempTotemDataset.buff.amount = orgAmount + spawnManger.stage * 4; // 임시 수식;
        }
        SetTotemInfo();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragCheck)
        {
            totemInfo.gameObject.SetActive(false);
        }
    }
}
