using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System;

public static class StaticCoroutine
{
    private class CoroutineHolder : MonoBehaviour { }
    
    private static CoroutineHolder _runner;
    private static CoroutineHolder runner
    {
        get
        {
            if (_runner == null)
            {
                _runner = new GameObject("Static Corotuine Runner").AddComponent<CoroutineHolder>();
            }
            return _runner;
        }
    }

    public static void StopAllCoroutines()
    {
        runner.StopAllCoroutines();
    }

    public static Coroutine StartCoroutine(IEnumerator corotuine)
    {
        return runner.StartCoroutine(corotuine);
    }
}