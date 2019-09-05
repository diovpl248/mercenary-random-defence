using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactReward : MonoBehaviour
{
    public GameObject rewardArtifactForm;

    
    public Image rewardLight;
    public Image rewardBackground;
    public Image rewardArtifactImage;
    public Text rewardArtifactRarity;
    public Text rewardArtifactName;
    public Text rewardArtifactDesc;
    public Text rewardArtifactStat;

    private static ArtifactReward instance;
    public static ArtifactReward Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ArtifactReward>();
            return instance;
        }
    }

    public void ShowRewardForm(Artifact artifact)
    {
        var color = Database.Instance.colors[(int)artifact.artifactRarity];

        rewardBackground.color = color;
        rewardLight.color = color;

        rewardArtifactRarity.text = artifact.artifactRarity.ToString();
        rewardArtifactImage.sprite = artifact.sprite;
        rewardArtifactName.text = artifact.name;
        rewardArtifactDesc.text = artifact.description;
        switch (artifact.type)
        {
            case ArtifactType.DamageUP:
                rewardArtifactStat.text = "데미지 " + artifact.amount + "% 증가";
                break;
            case ArtifactType.DefenseUP:
                rewardArtifactStat.text = "받는 피해량 " + artifact.amount + "% 감소";
                break;
            case ArtifactType.GoldUP:
                rewardArtifactStat.text = "골드 수급량 " + artifact.amount + "% 증가";
                break;
            case ArtifactType.TotemCoolDown:
                rewardArtifactStat.text = "토템 재사용 대기시간 " + artifact.amount + "초 감소 (최대 20초감소)";
                break;
            case ArtifactType.CriticalPercentUP:
                rewardArtifactStat.text = "크리티컬 확률 " + artifact.amount + "% 증가";
                break;
            case ArtifactType.BloodSucking:
                rewardArtifactStat.text = "가한 데미지의 " + artifact.amount + "% 흡혈";
                break;
            case ArtifactType.Combination:
                rewardArtifactStat.text = "조합 재료";
                break;
        }

        rewardArtifactForm.SetActive(true);
    }
}
