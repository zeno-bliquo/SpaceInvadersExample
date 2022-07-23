using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // https://forum.unity.com/threads/tip-invoke-any-function-with-delay-also-with-parameters.978273/

public static class Utility
{
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }
 
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}
