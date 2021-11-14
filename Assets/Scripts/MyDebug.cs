using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MyDebug
{
    [Conditional("ENABLE_LOG")]
    public static void Message(object message)
    {
        Debug.Log(message);
    }
}
