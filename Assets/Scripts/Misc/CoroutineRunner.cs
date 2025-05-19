using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Coroutine Run(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public Coroutine RunPooled(IEnumerator routine, Action onComplete = null)
    {
        var task = CoroutinePool.Get();
        task.Routine = routine;
        task.OnComplete = onComplete;
        return StartCoroutine(task.Execute());
    }
}

public class PooledCoroutine
{
    public IEnumerator Routine;
    public Action OnComplete;

    public IEnumerator Execute()
    {
        yield return Routine;
        OnComplete?.Invoke();
        CoroutinePool.ReturnToPool(this);
    }
}

public static class CoroutinePool
{
    private static readonly Stack<PooledCoroutine> pool = new();

    public static PooledCoroutine Get() => pool.Count > 0 ? pool.Pop() : new PooledCoroutine();

    public static void ReturnToPool(PooledCoroutine task)
    {
        task.Routine = null;
        task.OnComplete = null;
        pool.Push(task);
    }
}