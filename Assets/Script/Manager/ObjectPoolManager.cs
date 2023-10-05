using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject objectToPool;
        public int amountToPool;
        public bool canExpand;
    }
    
    public static ObjectPoolManager instance;
    public List<Pool> itemsToPool;
    public List<GameObject> pooledObjects;
    public GameObject playerFireballParent;
    public GameObject enemyFireballParent;
	void Awake() 
    {
        if(instance==null)
        {
            instance=this;
        }
        else if(instance!=this)
        {
            Destroy(gameObject);
            return;
        }

        pooledObjects = new List<GameObject>();
	}

	// Use this for initialization
    void Start() 
    {
        foreach(Pool item in itemsToPool) 
        {
            for(int i = 0; i < item.amountToPool; i++) 
            {
                GameObject obj = Instantiate(item.objectToPool);
                if(item.objectToPool.CompareTag("Fireball"))
                {
                    obj.transform.SetParent(playerFireballParent.transform);
                }
                else
                {
                    obj.transform.SetParent(enemyFireballParent.transform);
                }
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }
	
    public GameObject GetPooledObject(string tag) 
    {
        for(int i=0;i<pooledObjects.Count;i++) 
        {
            if(!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag) 
            {
                return pooledObjects[i];
            }
        }
        
        foreach(Pool item in itemsToPool) 
        {
            if(item.objectToPool.CompareTag(tag)) 
            {
                if (item.canExpand) 
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    if(item.objectToPool.CompareTag("Fireball"))
                    {
                        obj.transform.SetParent(playerFireballParent.transform);
                    }
                    else
                    {
                        obj.transform.SetParent(enemyFireballParent.transform);
                    }
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
}
