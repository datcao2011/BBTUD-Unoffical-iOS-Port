using UnityEngine;

public class NullRotation : MonoCache
{
    public NullRotationManager Manager;

    private Vector3 DRotation;
    private Vector3 DScale;

    private bool IsNormal = true;

    private void Start()
    {
        DRotation = transform.localEulerAngles;
        DScale = transform.localScale;
        IsNormal = true;
    }

    // Disable this script when the GameObject moves out of the camera's view
    private void OnBecameInvisible()
    {
        enabled = false;
    }

    // Enable this script when the GameObject moves into the camera's view
    private void OnBecameVisible()
    {
        enabled = true;
    }

    public override void onTick()
    {
        // if (mesh.isVisible)
        // {
        if (Manager.Intensity > 0f)
        {
            IsNormal = false;
            Vector3 Rotation = DRotation;
            Vector3 Scale = DScale;
            Rotation /= Manager.Result + 1f;
            Scale /= Manager.Result + 1f;

            transform.localEulerAngles = Rotation;
            transform.localScale = Scale;
        }
        else if (!IsNormal)
        {
            IsNormal = true;
            transform.localEulerAngles = DRotation;
            transform.localScale = DScale;
        }
        // }
    }
}