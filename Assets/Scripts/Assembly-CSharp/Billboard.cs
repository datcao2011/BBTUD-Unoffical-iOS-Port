// Billboard
using System;
using UnityEngine;

// This script is licensed under a Creative Commons Attribution 4.0 International License.
// (https://creativecommons.org/licenses/by/4.0/)
// This means you MUST give me (YuraSuper2048) credit if you are using this.
// If you redistribute this script (modified or not) you should save this message as is. 
public class Billboard : MonoBehaviour
{
    private Transform m_Camera;

    private bool OnOptimized;

    // Change this to control frequency of billboarding
    // Less = more optimized, but will increase billboarding delays
    // More = less optimized, will reduce billboarding delays
    // (This actually is distance to player where update will take 1 second)
    private const float UpdateFrequency =
#if UNITY_ANDROID
        300;
#else
        1000;
#endif

    // Time before last update
    private float time;

    // Checkmark for objects that must not be optimized (eg: fast moving objects)
    private bool doNotOptimize;

    // Time to wait before billboarding
    private float waitTime;

    // Needed to reduce the built-in unity calls
    private new Transform transform;

    private void Start()
    {
        m_Camera = Camera.main.transform;
        OnOptimized = Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault);

        if (OnOptimized) // Getting transform
            transform = base.transform;
        if (MultiplayerManager.isActive && OnOptimized && transform.parent != null && transform.parent.name == "Baldi")
        {
            OnOptimized = false;
        }
        if (name.EndsWith(" Balloon"))
        {
            OnOptimized = false;
        }
        if (name == "Nameplate")
        {
            OnOptimized = false;
        }
    }

    private void LateUpdate()
    {
        if (!OnOptimized)
        {
            base.transform.LookAt(base.transform.position + m_Camera.rotation * Vector3.forward, m_Camera.rotation * Vector3.up);
            base.transform.localEulerAngles = new Vector3(0f, base.transform.localEulerAngles.y, 0f);
        }
    }

    private void OnBecameVisible()
    {
        if (OnOptimized)
        {
            m_Camera = Camera.main.transform;

            transform.rotation = Quaternion.Euler(0f,
                m_Camera.rotation.eulerAngles.y, 0f);
        }
    }

    private void OnWillRenderObject()
    {
        if (OnOptimized)
        {
            // Getting current camera for further use
            m_Camera = Camera.main.transform;

            // Counting time
            time += Time.unscaledDeltaTime;

            // Waiting some time based on distance
            if (time < waitTime && !doNotOptimize) return;
            time = 0;

            try
            {
                // Calcualting time to wait
                if (!doNotOptimize)
                    waitTime = Vector3.Distance(transform.position, m_Camera.position) / UpdateFrequency;

                // Billboarding
                transform.rotation = Quaternion.Euler(0f,
                    m_Camera.rotation.eulerAngles.y, 0f);
            }
            catch
            {
                // Ignore all errors
            }
        }
    }
}