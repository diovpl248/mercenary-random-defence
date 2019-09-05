using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffPool : MonoBehaviour
{
    public GameObject Buff;
    public GameObject buffPanel;
    private List<GameObject> BuffingPool = new List<GameObject>();

    public GameObject GetFromPool()
    {
        for (int i = 0; i < BuffingPool.Count; i++)
        {
            if (!BuffingPool[i].gameObject.activeInHierarchy)
            {
                return BuffingPool[i];
            }
        }
        GameObject temp = Instantiate(Buff, buffPanel.transform);
        BuffingPool.Add(temp);
        return temp;
    }
}
