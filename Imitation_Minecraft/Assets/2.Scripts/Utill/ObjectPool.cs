using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ObjectPool<T> : MonoBehaviour where T : class
{
    int m_count = 0;
    Func<T> m_func;
    Queue<T> pool = new Queue<T>();

    public int Count
    {
        get { return m_count; }
        set { m_count = value; }
    }

    public ObjectPool(int count, Func<T> func)
    {
        m_count = count;
        m_func = func;
        Allocation();
    }

    public T Get()
    {
        if (pool.Count>0)
        {
            return pool.Dequeue();
        }
        else
        {
            return m_func();
        }
    }
    public void Set(T data)
    {
        pool.Enqueue(data);
    }
    
    void Allocation()
    {
        for (int i = 0; i < m_count; i++)
        {
            pool.Enqueue(m_func());
        }
    }
    
    
}
