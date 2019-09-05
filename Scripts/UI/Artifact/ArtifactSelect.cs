using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSelect : MonoBehaviour
{
    public GameObject artifactSelectForm;

    public bool isTouching = false;
    public bool isSelecting = false;
    public Artifact[] artifact = new Artifact[3];

    public Image[] artifactPanel;
    public Image[] artifactImage;
    public Text[] artifactName;
    public Text[] artifactRarity;
    public Text[] artifactDesc;
    public Text[] artifactOption;

    public Button artifactRefresh;
    public Image artifactRefreshImage;

    private static ArtifactSelect instance;
    public static ArtifactSelect Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ArtifactSelect>();
            return instance;
        }
    }

    public void LoadArtifactSelect(string[] artifactsName)
    {
        isSelecting = true;

        GameManager.Instance.MenuButtomPause();

        var artifactList = ArtifactManager.Instance.artifactTotalList;

        for (int i = 0; i < 3; i++)
        {
            foreach(var artifactInfo in artifactList)
            {
                if(artifactInfo.name.Equals(artifactsName[i]))
                {
                    artifact[i] = artifactInfo;
                    break;
                }
            }

            artifactPanel[i].color = Database.Instance.colors[(int)artifact[i].artifactRarity];
            artifactImage[i].sprite = artifact[i].sprite;
            artifactName[i].text = artifact[i].name;
            artifactRarity[i].text = artifact[i].artifactRarity.ToString();
            artifactDesc[i].text = artifact[i].description;
            artifactOption[i].text = Artifact.ArtifactTypeToString(artifact[i].type);
        }

        artifactSelectForm.SetActive(true);
    }

    public void ShowArtifactSelect()
    {
        artifactRefresh.enabled = true;
        artifactRefreshImage.color = new Color(255/ 255f, 131/ 255f, 0, 255/ 255f);

        if (!isSelecting)
        {
            isSelecting = true;
            isTouching = false;
            GameManager.Instance.MenuButtomPause();
        }

        artifact = ArtifactManager.Instance.GetRandom3Artifact();
        for (int i=0;i<3;i++)
        {
            artifactPanel[i].color = Database.Instance.colors[(int)artifact[i].artifactRarity];
            artifactImage[i].sprite = artifact[i].sprite;
            artifactName[i].text = artifact[i].name;
            artifactRarity[i].text = artifact[i].artifactRarity.ToString();
            artifactDesc[i].text = artifact[i].description;
            artifactOption[i].text = Artifact.ArtifactTypeToString(artifact[i].type);
        }

        artifactSelectForm.SetActive(true);
    }

    public void Select1()
    {
        if (isTouching)
            return;

        isTouching = true;
        ArtifactConfirm.Instance.SetArtifactConfirmForm(artifact[0]);
    }

    public void Select2()
    {
        if (isTouching)
            return;

        isTouching = true;
        ArtifactConfirm.Instance.SetArtifactConfirmForm(artifact[1]);
    }

    public void Select3()
    {
        if (isTouching)
            return;

        isTouching = true;
        ArtifactConfirm.Instance.SetArtifactConfirmForm(artifact[2]);
    }
    
    public void ShowAdMob()
    {
        AdMobManager.Instance.OnBtnViewAdClicked();
    }

    public void Refresh()
    {
        ShowArtifactSelect();

        // 비활성코드
        artifactRefresh.enabled = false;
        artifactRefreshImage.color = Color.gray;
    }

    // 테스트용코드
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F10))
        {
            ShowArtifactSelect();
        }
#endif
    }
}
