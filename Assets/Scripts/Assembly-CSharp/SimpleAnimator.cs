using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleAnimator : MonoBehaviour
{
    public void _SetTrigger(string str)
    {
        GetComponent<Animator>().SetTrigger(str);
    }
}