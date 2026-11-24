using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonUISelection : UnityEngine.UI.Selectable
{
    public ButtonClickedEvent onClick
    {
        get
        {
            return this.m_onClick;
        }
        set
        {
            this.m_onClick = value;
        }
    }

    private void Update()
    {
        if (IsHighlighted() == true)
        {
            GetComponent<TMP_Text>().fontStyle = FontStyles.Underline;
        }
        else
        {
            GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;
        }
        if (IsPressed())
        {
            m_onClick.Invoke();
        }
    }

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_onClick = new ButtonClickedEvent();


    [System.Serializable]
    public class ButtonClickedEvent : UnityEvent
    {

    }
}