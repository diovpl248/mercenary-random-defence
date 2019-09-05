using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactScrollView : MonoBehaviour
{
    public GameObject content;
    //public GameObject artifactPrefab;

    public Text textDamagePercent;
    public Text textTakeDamagePercent;
    public Text textBloodSuckingPercent;

    void ShowList()
    {
        List<Artifact> artifactList = ArtifactManager.Instance.artifactList;

        foreach (var artifact in artifactList)
        {
            GameObject go = MasterObjectPool.Instance.GetFromPoolOrNull("ArtifactItem", content);
            go.GetComponent<ArtifactItem>().SetInfo(artifact);
            go.transform.localScale = new Vector3(1, 1, 1);
            Color color = Database.Instance.colors[(int)artifact.artifactRarity];
            color.a = 0.2f;
            go.transform.GetChild(0).GetComponent<Image>().color = color;
            go.SetActive(true);
        }
    }

    private void OnEnable()
    {
        float damagePercent = ArtifactManager.Instance.DamagePercent - 100f;
        float takeDamagePercent = ArtifactManager.Instance.TakeDamagePercent - 100f;
        float bloodSickingPercent = ArtifactManager.Instance.BloodSuckingPercent;

        textDamagePercent.text = "공격력: +" + (int)damagePercent + "%";
        textTakeDamagePercent.text = "받는피해: " + (int)takeDamagePercent + "%";
        textBloodSuckingPercent.text = "흡혈: +" + (int)bloodSickingPercent + "%";

        ShowList();
    }

}
