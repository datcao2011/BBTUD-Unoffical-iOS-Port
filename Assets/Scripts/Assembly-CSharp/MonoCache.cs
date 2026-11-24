using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCache : MonoBehaviour
{
    public static List<MonoCache> allUpdate = new List<MonoCache>(100001);

    private void OnEnable() => DoCache(false);
    private void OnDisable() => DoCache(true);
    private void OnDestroy() => DoCache(true);


    private void DoCache(bool remove)
    {
        if (remove)
        {
            allUpdate.Remove(this);
        }
        else
        {
            allUpdate.Add(this);
        }
    }

    public void Tick() => onTick();

    public virtual void onTick()
    {

    }
}