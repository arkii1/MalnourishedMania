using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalnourishedMania
{
    [CreateAssetMenu(menuName ="Managers/Object Pooler", fileName = "Object Pooler")]
    public class ObjectPooler : ScriptableObject
    {
        public List<Pool> pools = new List<Pool>();
        Dictionary<string, int> obj_dict = new Dictionary<string, int>();
        GameObject poolParent;


        public void Init()
        {
            if (poolParent)
                Destroy(poolParent);

            poolParent = new GameObject();
            poolParent.name = "Object Pool";
            obj_dict.Clear();

            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].budget < 1)
                    pools[i].budget = 1;

                pools[i].cur = 0;
                pools[i].createdObjects.Clear();

                if (obj_dict.ContainsKey(pools[i].prefab.name))
                {
                    Debug.Log("Object Pooler containts duplicates of entry with id " + pools[i].prefab.name + "! This is not allowed!");
                    continue;
                }
                else
                {
                    obj_dict.Add(pools[i].prefab.name, i);
                }
            }
        }

        public GameObject RequestObject(string id)
        {
            if (!poolParent)
                Init();


            GameObject retVal = null;
            int index = 0;
            if (obj_dict.TryGetValue(id, out index))
            {
                Pool p = pools[index];
                if (p.createdObjects.Count - 1 < p.budget)
                {
                    retVal = Instantiate(p.prefab);
                    retVal.transform.parent = poolParent.transform;
                    p.createdObjects.Add(retVal);
                }
                else
                {
                    p.cur = (p.cur < p.createdObjects.Count - 1) ? p.cur += 1 : 0;
                    retVal = p.createdObjects[p.cur];
                    retVal.SetActive(false);
                    retVal.SetActive(true);
                }
            }

            return retVal;
        }
    }
}

[System.Serializable]
public class Pool
{
    public GameObject prefab;
    public int budget = 15;
    [HideInInspector]
    public int cur; //which current indexed object to replace
    [HideInInspector]
    public List<GameObject> createdObjects = new List<GameObject>();
}
