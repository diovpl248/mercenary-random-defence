using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterObjectPool : MonoBehaviour
{
    // 인스펙터창에서 지정용
    public GameObject[] prefabs;

    // 카테고리 정리용
    Dictionary<string, GameObject> parents = new Dictionary<string, GameObject>();
    // 프리팹 저장용
    Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    // 실제 저장되는 풀
    Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();

    private static MasterObjectPool instance;
    public static MasterObjectPool Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MasterObjectPool>();
            return instance;
        }
    }

    private void Awake()
    {
        foreach (var prefab in prefabs)
        {
            // 빈 게임오브젝트 만들어서 항목별 정리용으로 사용
            var parent = new GameObject();
            parent.transform.parent = transform;
            parent.name = prefab.name;

            parents.Add(prefab.name, parent);

            prefabDict.Add(prefab.name, prefab);
            pool.Add(prefab.name, new List<GameObject>());
        }
    }

    // 빠르고 간편하게 사용
    public GameObject GetFromPoolOrNull(string name)
    {
        if (!pool.ContainsKey(name))
            return null;

        foreach (var obj in pool[name])
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        GameObject newObject = Instantiate(prefabDict[name], transform);
        newObject.transform.SetParent(parents[name].transform);

        newObject.SetActive(false);
        pool[name].Add(newObject);

        return newObject;
    }


    // UI같은경우 캔버스가 있어야 하기때문에 부모대상을 임의로 지정
    public GameObject GetFromPoolOrNull(string name, GameObject parentGameObject)
    {
        if (!pool.ContainsKey(name))
            return null;

        foreach (var obj in pool[name])
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        GameObject newObject = Instantiate(prefabDict[name], transform);
        newObject.transform.SetParent(parentGameObject.transform);

        newObject.SetActive(false);
        pool[name].Add(newObject);

        return newObject;
    }

}