using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : ScriptableObject
{
    private readonly GameObject prefab;
    private readonly T controller;
    private readonly Transform parent;
    private readonly int expandCount;
    private readonly Func<T> createFunc;

    private readonly Queue<(T Controller, GameObject Instance)> available = new();

    public ObjectPool(GameObject prefab, T controller, Transform parent, int initialCount, int expandCount, Func<T> createFunc = null)
    {
        this.prefab = prefab;
        this.controller = controller;
        this.parent = parent;
        this.expandCount = expandCount;
        this.createFunc = createFunc ?? (() => UnityEngine.Object.Instantiate(controller));

        Expand(initialCount);
    }

    private void Expand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var instance = UnityEngine.Object.Instantiate(prefab, parent);
            instance.SetActive(false);
            var newController = createFunc();
            available.Enqueue((newController, instance));
        }
    }

    public (T Controller, GameObject Instance) Get()
    {
        if (available.Count == 0)
        {
            Expand(expandCount);
        }

        var (controller, instance) = available.Dequeue();
        instance.SetActive(true);
        return (controller, instance);
    }

    public void Return(T controller, GameObject instance)
    {
        if (instance != null)
        {
            instance.transform.localScale = Vector3.one;
            instance.SetActive(false);
            instance.transform.SetParent(parent);
            available.Enqueue((controller, instance));
        }
    }
}