using System;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPool<T> : MonoBehaviour where T : class
{
    int _count = 0;
    Func<T> _func;
    Queue<T> _pool = new Queue<T>();

    public int Count
    {
        get { return _count; }
        set { _count = value; }
    }

    public ObjectPool(int count, Func<T> func)
    {
        _count = count;
        _func = func;
        Allocation();
    }

    public T Get()
    {
        if (_pool.Count>0)
        {
            return _pool.Dequeue();
        }
        else
        {
            return _func();
        }
    }
    public void Set(T data)
    {
        _pool.Enqueue(data);
    }
    
    void Allocation()
    {
        for (int i = 0; i < _count; i++)
        {
            _pool.Enqueue(_func());
        }
    }
    
    
}
