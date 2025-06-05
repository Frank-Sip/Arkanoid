using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager
{
    private Canvas dynamicCanvas;
    private Dictionary<string, GameCounter> counters = new Dictionary<string, GameCounter>();

    public void Init(GameObject canvas, GameCounter[] gameCounters)
    {
        dynamicCanvas = canvas.GetComponent<Canvas>();
        dynamicCanvas.sortingOrder = 100;
        
        foreach (var counter in gameCounters)
        {
            if (!string.IsNullOrEmpty(counter.id))
            {
                counters[counter.id] = counter;
                counter.UpdateValue(0);
            }
        }
        dynamicCanvas.gameObject.SetActive(false);
    }

    public void IncrementCounter(string id)
    {
        if (counters.TryGetValue(id, out GameCounter counter))
        {
            counter.Increment();
        }
    }

    public void SetCounterValue(string id, int value)
    {
        if (counters.TryGetValue(id, out GameCounter counter))
        {
            counter.UpdateValue(value);
        }
    }

    public void ResetCounters()
    {
        foreach (var counter in counters.Values)
        {
            counter.Reset();
            counter.UpdateValue(0);
        }
    }
}