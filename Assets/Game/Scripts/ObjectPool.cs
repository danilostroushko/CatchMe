using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<TObject>
{
    private readonly Stack<TObject> pool;
    private readonly int capacity;
    private readonly Func<TObject> createFunc;
    private readonly Action<TObject> putAction;
    private readonly Action<TObject> clearAction;

    public int Count => pool.Count;

    public ObjectPool(int capacity, Func<TObject> createFunc, Action<TObject> putAction = null, Action<TObject> clearAction = null)
    {
        pool = new Stack<TObject>(capacity);

        this.createFunc = createFunc;
        this.putAction = putAction;
        this.clearAction = clearAction;
        this.capacity = capacity;
    }

    public TObject Get()
    {
        if (createFunc == null)
        {
            throw new ArgumentNullException("createFunc for pool object can't be null");
        }

        return pool.Count > 0 ? pool.Pop() : createFunc();
    }

    public void Put(TObject obj)
    {
        putAction?.Invoke(obj);

        if (pool.Count == capacity)
        {
            return;
        }

        pool.Push(obj);
    }

    public void Clear()
    {
        if (clearAction == null)
        {
            pool.Clear();
            return;
        }

        while (pool.Count > 0)
        {
            TObject obj = pool.Pop();
            clearAction(obj);
        }
    }
}