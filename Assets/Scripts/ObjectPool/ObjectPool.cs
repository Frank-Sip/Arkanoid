using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Stack<T> pool = new Stack<T>();
    private readonly int expandCount;

    public ObjectPool(T prefab, Transform parent, int initialCount = 2, int expandCount = 10)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.expandCount = expandCount;

        for (int i = 0; i < initialCount; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }
    
    private void ExpandPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }

    public T Get()
    {
        if (pool.Count <= 0)
        {
            ExpandPool(expandCount);
        }

        T obj = pool.Pop();
        obj.gameObject.SetActive(true);
        return obj;
    }
    
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }
}
