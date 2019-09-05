using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotemManager : MonoBehaviour
{
    public GameObject totemPrefab;
    [SerializeField]
    public float pastTime;
    public float coolTime;
    public bool isUsable;
    public bool isDragging = false;
    public Text coolTimeText;
    public Image[] cooltimeImage;

    Dictionary<string,TotemDataset> totemDatas = new Dictionary<string, TotemDataset>();
    Dictionary<string, Sprite> Images = new Dictionary<string, Sprite>();

    public List<Totem> totems = new List<Totem>();

    private static TotemManager instance = null;
    public static TotemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TotemManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        pastTime = coolTime;
        // 이미지 로드
        Images.Add("buff_attackup", Resources.Load<Sprite>("Image/Buff/buff_attackup"));
        Images.Add("buff_dotheal", Resources.Load<Sprite>("Image/Buff/buff_dotheal"));
        Images.Add("buff_speeddown", Resources.Load<Sprite>("Image/Buff/buff_speeddown"));
        Images.Add("buff_dotdamage", Resources.Load<Sprite>("Image/Buff/buff_dotdamage"));
        Images.Add("buff_defendup", Resources.Load<Sprite>("Image/Buff/buff_defendup"));

        StartCoroutine(LoadJsonDataCoroutine());
        StartCoroutine(totemCoolTimeCoroutine());
    }

    IEnumerator totemCoolTimeCoroutine()
    {
        while(true)
        {
            pastTime += Time.deltaTime * GameManager.Instance.GameSpeed;
            if (pastTime > coolTime)
            {
                coolTimeText.gameObject.SetActive(false);
                coolTimeText.text = "0초";
                foreach (Image Cool in cooltimeImage)
                {
                    Cool.fillAmount = 0;
                }
                isUsable = true;
            }
            else
            {
                coolTimeText.gameObject.SetActive(true);
                coolTimeText.text = ((int)(coolTime - pastTime)).ToString() + "초";
                foreach (Image Cool in cooltimeImage)
                {
                    Cool.fillAmount = 1;
                }
                isUsable = false;
            }

            yield return null;
        }
    }

    public void PlacementSuccess()
    {
        // 시작시간을 조절해서 쿨다운을 적용
        pastTime = ArtifactManager.Instance.TotemCoolDown;
    }

    IEnumerator LoadJsonDataCoroutine()
    {
        yield return new WaitUntil(() => JsonManager.Instance.ready);

        LoadTotemDataFromJson(JsonManager.Instance.jsonTotems);
    }

    public void LoadTotemDataFromJson(JsonTotem[] jsonTotems)
    {
        foreach(var jsonTotem in jsonTotems)
        {
            Buff buff = new Buff(
                    (BuffType)System.Enum.Parse(typeof(BuffType), jsonTotem.buff_type),
                    (jsonTotem.buff_positive == 1 ? true : false),
                    jsonTotem.buff_name,
                    jsonTotem.buff_desc,
                    jsonTotem.buff_amount,
                    (jsonTotem.buff_tick_time == 0) ? 0 : jsonTotem.buff_tick_time + 0.1f,
                    jsonTotem.buff_tick_time,
                    jsonTotem.buff_tick_time
                );

            TotemDataset totemDataset = new TotemDataset(
                    jsonTotem.totem_name,
                    Images[jsonTotem.sprite_name],
                    jsonTotem.radius,
                    jsonTotem.duration_time,
                    0,
                    (jsonTotem.affect_myunit == 1 ? true : false),
                    (jsonTotem.affect_enemy == 1 ? true : false),
                    buff
                );

            totemDatas.Add(totemDataset.totemName, totemDataset);
        }
    }

    public GameObject CreateTotemOrNull(string totemName)
    {
        if (!totemDatas.ContainsKey(totemName))
            return null;

        GameObject go = MasterObjectPool.Instance.GetFromPoolOrNull("TotemBase",gameObject);
        go.name = totemName;
        go.GetComponent<Totem>().Init(totemDatas[totemName].Clone());
        // Init() 안에 SetActive(true)가 포함돼있다.

        
        return go;
    }
}
