using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactItem : MonoBehaviour
{
    public Image icon;
    public Text name;
    public Text desc;
    public Text stat;

    public void SetInfo(Artifact artifact)
    {
        icon.sprite = artifact.sprite;
        name.text = artifact.name;
        desc.text = artifact.description;
        switch(artifact.type)
        {
            case ArtifactType.DamageUP:
                stat.text = "데미지 " + artifact.amount + "% 증가";
                break;
            case ArtifactType.DefenseUP:
                stat.text = "받는 피해량 " + artifact.amount + "% 감소";
                break;
            case ArtifactType.GoldUP:
                stat.text = "골드 수급량 " + artifact.amount + "% 증가";
                break;
            case ArtifactType.TotemCoolDown:
                stat.text = "토템 재사용 대기시간 " + artifact.amount + "초 감소 (최대 20초감소)";
                break;
            case ArtifactType.CriticalPercentUP:
                stat.text = "크리티컬 확률 " + artifact.amount + "% 증가";
                break;
            case ArtifactType.BloodSucking:
                stat.text = "가한 데미지의 " + artifact.amount + "% 흡혈";
                break;
            case ArtifactType.Combination:
                stat.text = "조합 재료";
                break;
        }
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
