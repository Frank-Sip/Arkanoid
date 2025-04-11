using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public static event Action OnReset;

    public static void ResetGame()
    {
        OnReset?.Invoke();
    }
}
