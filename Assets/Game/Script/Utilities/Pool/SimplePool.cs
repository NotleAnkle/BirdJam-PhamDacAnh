using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimplePool
{
    static int DEFAULT_AMOUNT = 10;

    static Dictionary<GameUnit, Pool> poolObjects = new Dictionary<GameUnit, Pool>();

    static Dictionary<GameUnit, Pool> poolParents = new Dictionary<GameUnit, Pool>();

    public static void Preload(GameUnit prefab, int amount, Transform parent)
    {
        if (!poolObjects.ContainsKey(prefab))
        {
            poolObjects.Add(prefab, new Pool(prefab, amount, parent));
        }
    }

    public static GameUnit Spawn(GameUnit prefab, Vector3 position, Quaternion rotation)
    {
        GameUnit obj = null;

        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            poolObjects.Add(prefab, new Pool(prefab, DEFAULT_AMOUNT, null));
        }

        obj = poolObjects[prefab].Spawn(position, rotation);

        return obj;
    }

    public static GameUnit Spawn3D(GameUnit prefab, Vector3 position, Quaternion rotation)
    {
        GameUnit obj = null;

        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            poolObjects.Add(prefab, new Pool(prefab, DEFAULT_AMOUNT, null));
        }

        obj = poolObjects[prefab].Spawn3D(position, rotation);

        return obj;
    }

    public static T Spawn<T>(GameUnit prefab, Vector3 position, Quaternion rotation) where T : GameUnit
    {
        GameUnit obj = null;

        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            poolObjects.Add(prefab, new Pool(prefab, DEFAULT_AMOUNT, null));
        }

        obj = poolObjects[prefab].Spawn(position, rotation);

        return obj as T;
    }

    public static T Spawn3D<T>(GameUnit prefab, Vector3 position, Quaternion rotation) where T : GameUnit
    {
        GameUnit obj = null;

        if (!poolObjects.ContainsKey(prefab) || poolObjects[prefab] == null)
        {
            poolObjects.Add(prefab, new Pool(prefab, DEFAULT_AMOUNT, null));
        }

        obj = poolObjects[prefab].Spawn3D(position, rotation);

        return obj as T;
    }

    public static void Despawn(GameUnit GameUnit)
    {
        if (poolParents.ContainsKey(GameUnit))
        {
            poolParents[GameUnit].Despawn(GameUnit);
        }
        else
        {
            GameObject.Destroy(GameUnit);
        }
    }

    public static void CollectAll()
    {
        foreach (var item in poolObjects)
        {
            item.Value.Collect();
        }
    }

    public static void ReleaseAll()
    {
        foreach (var item in poolObjects)
        {
            item.Value.Release();
        }

        poolObjects.Clear();
    }

    public class Pool
    {
        Queue<GameUnit> pools = new Queue<GameUnit>();
        List<GameUnit> activeObjs = new List<GameUnit>();
        Transform parent;
        GameUnit prefab;

        public Pool(GameUnit prefab, int amount, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < amount; i++)
            {
                GameUnit obj = GameObject.Instantiate(prefab, parent);
                poolParents.Add(obj, this);
                pools.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        public GameUnit Spawn(Vector3 position, Quaternion rotation)
        {
            GameUnit obj = null;

            if (pools.Count == 0)
            {
                obj = GameObject.Instantiate(prefab, parent);
                poolParents.Add(obj, this);
            }
            else
            {
                obj = pools.Dequeue();
            }

            obj.rectTf.anchoredPosition = position;
            obj.gameObject.SetActive(true);

            activeObjs.Add(obj);
            return obj;
        }

        public GameUnit Spawn3D(Vector3 position, Quaternion rotation)
        {
            GameUnit obj = null;

            if (pools.Count == 0)
            {
                obj = GameObject.Instantiate(prefab, parent);
                poolParents.Add(obj, this);
            }
            else
            {
                obj = pools.Dequeue();
            }

            obj.tf.position = position;
            obj.gameObject.SetActive(true);

            activeObjs.Add(obj);
            return obj;
        }

        public void Despawn(GameUnit GameUnit)
        {
            GameUnit.gameObject.SetActive(false);
            activeObjs.Remove(GameUnit);
            pools.Enqueue(GameUnit);
        }

        public void Collect()
        {
            while (activeObjs.Count > 0)
            {
                Despawn(activeObjs[0]);
            }
        }

        public void Release()
        {
            Collect();

            while (pools.Count > 0)
            {
                GameUnit obj = pools.Dequeue();
                GameObject.Destroy(obj);
            }
        }
    }
}

public class GameUnit : MonoBehaviour
{
    public Transform tf;
    public RectTransform rectTf;

    public Transform Transform
    {
        get
        {
            if (this.tf == null)
            {
                this.tf = transform;
            }

            return tf;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}