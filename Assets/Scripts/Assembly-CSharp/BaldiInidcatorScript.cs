using UnityEngine;

public class BaldiInidcatorScript : MonoBehaviour
{
    public Animator baldiInidcator;

    private void Start()
    {
        baldiInidcator = base.GetComponent<Animator>();
        Index = Settings.GetInt("BaldiIndicator");
    }

    private void OnEnable()
    {
        Settings.OnSettingChanged += OnSettingChanged;
    }

    private void OnDisable()
    {
        Settings.OnSettingChanged -= OnSettingChanged;
    }

    private int Index;

    private void OnSettingChanged(string setting, object val)
    {
        if (setting == "BaldiIndicator")
        {
            Index = (int)val;
        }
    }

    public void TriggerHear()
    {
        if (Index == 3 || Index == 1) return;
        baldiInidcator.SetTrigger("Baldi_Ind");
    }
    public void TriggerConfused()
    {
        if (Index == 3 || Index == 1) return;
        baldiInidcator.SetTrigger("Baldi_ConInd");
    }
    public void TriggerLost()
    {
        if (Index == 3 || Index == 2) return;
        baldiInidcator.SetTrigger("ConInd");
    }
    public void TriggerFound()
    {
        if (Index == 3 || Index == 2) return;
        baldiInidcator.SetTrigger("FoundInd");
    }
}
