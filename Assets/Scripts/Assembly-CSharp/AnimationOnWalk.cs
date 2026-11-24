using UnityEngine;

public class AnimationOnWalk : MonoBehaviour
{
    private Vector3 lastPosition;
    public Animator anim;
    public int CurrentItemSlot;
    private float AnimTime = 0.1f;
    private bool CurrentBoolean;

    private Transform t;

    private Vector3 NextPosition;
    private Quaternion NextRotation = Quaternion.identity;

    public float YAddition = 0f;

    private void Awake()
    {
        t = transform.parent != null ? transform.parent : transform;
        NextPosition = t.position;
        NextRotation = t.rotation;
    }

    private void Update()
    {
        if (AnimTime > 0f)
        {
            AnimTime -= Time.unscaledDeltaTime;
            SetBool(true);
        }
        else
        {
            AnimTime = 0f;
            SetBool(false);
        }
        // Position
        if (t.position != NextPosition)
        {
            t.position = Vector3.Lerp(t.position, NextPosition, 20f * Time.unscaledDeltaTime);
        }
        if (t.rotation != NextRotation)
        {
            t.rotation = Quaternion.Lerp(t.rotation, NextRotation, 20f * Time.unscaledDeltaTime);
        }
    }

    public void UpdatePos(Vector3 pos, Quaternion rot)
    {
        if (YAddition != 0f) pos.y += YAddition;
        NextPosition = pos;
        NextRotation = rot;
    }

    public void Moving()
    {
        AnimTime = 0.1f;
    }

    private void SetBool(bool TheBoolean)
    {
        if (TheBoolean == CurrentBoolean) return;
        CurrentBoolean = TheBoolean;
        anim.SetBool("w", CurrentBoolean);
    }
}