using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactConfirm : MonoBehaviour
{
    public GameObject form;
    public Image artifactImage;
    public Text artifactName;

    Artifact artifact;

    private static ArtifactConfirm instance;
    public static ArtifactConfirm Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ArtifactConfirm>();
            return instance;
        }
    }

    public void SetArtifactConfirmForm(Artifact artifact)
    {
        this.artifact = artifact;

        artifactImage.sprite = artifact.sprite;
        artifactName.text = artifact.name;

        form.SetActive(true);
    }

    public void Yes()
    {
        ArtifactManager.Instance.AddArtifact(artifact.type, artifact.name);
        ArtifactReward.Instance.ShowRewardForm(artifact);

        form.SetActive(false);
        ArtifactSelect.Instance.artifactSelectForm.SetActive(false);

        ArtifactSelect.Instance.isSelecting = false;
    }

    public void No()
    {
        ArtifactSelect.Instance.isTouching = false;
        form.SetActive(false);
    }
}
