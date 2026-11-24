using System;
using UnityEngine;

public class GrapplingHookItem
{
    public Transform Hook;
    public float Speed;

    public Vector3 GetFrom(Vector3 Position)
    {
        return (Hook.position - Position).normalized * Speed;
    }
}