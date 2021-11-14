using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPool : MonoBehaviour
{
    [SerializeField]
    private GameObject wall;

    [SerializeField]
    private Transform parent;

    List<GameObject> wallPool = new List<GameObject>();

    public GameObject GetWall()
    {
        for (int i = 0; i < wallPool.Count; i++)
        {
            if (!wallPool[i].activeSelf)
            {
                return wallPool[i];
            }
        }

        GameObject go = Instantiate(wall, Vector3.zero, Quaternion.identity);

        go.SetActive(false);

        if (parent != null)
        {
            go.transform.parent = parent;
        }

        wallPool.Add(go);

        return go;
    }

    public void CleanPool()
    {
        for (int i = 0; i < wallPool.Count; i++)
        {
            wallPool[i].SetActive(false);
        }
    }
}
