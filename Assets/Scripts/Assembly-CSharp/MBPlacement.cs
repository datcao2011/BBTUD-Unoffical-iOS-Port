using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MBPlacement : MonoBehaviour
{

    private void Awake()
    {
        RectTransform[] Children = GetChildren(transform);
        foreach (RectTransform Child in Children)
        {
            if (Child.name == "Touch" || Child.name == "Pause" || Child.name.StartsWith("MobileItemSlot")) continue;
            if (Child.name == "One Player Controls" || Child.name == "Two Player Controls")
            {
                RectTransform[] B_Children = GetChildren(Child);
                foreach (RectTransform B_Child in B_Children)
                {
                    DefaultPositions.Add(B_Child.name, new Placement(B_Child.anchoredPosition, B_Child.localScale));
                }
                continue;
            }
            DefaultPositions.Add(Child.name, new Placement(Child.anchoredPosition, Child.localScale));
        }
    }

    private Dictionary<string, Placement> DefaultPositions = new Dictionary<string, Placement>();

    public class Placement
    {
        public Placement(Vector2 Pos, Vector2 Siz)
        {
            Position = Pos;
            Size = Siz;
        }
        public Vector2 Position;
        public Vector2 Size;
    }

    private RectTransform[] GetChildren(Transform o)
    {
        RectTransform[] Result = new RectTransform[o.childCount];
        for (int i = 0; i < Result.Length; i++)
        {
            Result[i] = o.GetChild(i).GetComponent<RectTransform>();
        }
        return Result;
    }

    public Placement GetSizeAndPosition(string ObjectName)
    {
        if (!Settings.HasKey("MobileC_" + ObjectName)) return GetDefault(ObjectName);
        try
        {
            string Value = Settings.GetString("MobileC_" + ObjectName);
            string[] Value2 = Value.Split('|');
            string[] Pos = Value2[0].Split(',');
            float[] Converted = new float[2] { float.Parse(Pos[0], NumberStyles.Float, CultureInfo.InvariantCulture), float.Parse(Pos[1], NumberStyles.Float, CultureInfo.InvariantCulture) };
            string[] Scal = Value2[1].Split(',');
            float[] Converted2 = new float[2] { float.Parse(Scal[0], NumberStyles.Float, CultureInfo.InvariantCulture), float.Parse(Scal[1], NumberStyles.Float, CultureInfo.InvariantCulture) };
            return new Placement(new Vector2(Converted[0], Converted[1]), new Vector2(Converted2[0], Converted2[1]));
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Unable to get position for " + ObjectName + " returning default");
            return GetDefault(ObjectName);
        }
    }

    public Placement GetDefault(string ObjectName)
    {
        if (!DefaultPositions.ContainsKey(ObjectName))
        {
            return new Placement(Vector2.zero, Vector2.zero);
        }
        return DefaultPositions[ObjectName];
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        RefreshPlacement();
    }

    public void RefreshPlacement()
    {
        RectTransform[] Children = GetChildren(transform);
        foreach (RectTransform Child in Children)
        {
            if (Child.name == "Touch" || Child.name == "Pause" || Child.name.StartsWith("MobileItemSlot")) continue;
            if (Child.name == "One Player Controls" || Child.name == "Two Player Controls")
            {
                RectTransform[] B_Children = GetChildren(Child);
                foreach (RectTransform B_Child in B_Children)
                {
                    Placement placement = GetSizeAndPosition(B_Child.name);
                    B_Child.anchoredPosition = placement.Position;
                    B_Child.localScale = placement.Size;
                }
                continue;
            }
            Placement placement2 = GetSizeAndPosition(Child.name);
            Child.anchoredPosition = placement2.Position;
            Child.localScale = placement2.Size;
        }
    }

    public void DefaultPlacement()
    {
        RectTransform[] Children = GetChildren(transform);
        foreach (RectTransform Child in Children)
        {
            if (Child.name == "Touch" || Child.name == "Pause" || Child.name.StartsWith("MobileItemSlot")) continue;
            if (Child.name == "One Player Controls" || Child.name == "Two Player Controls")
            {
                RectTransform[] B_Children = GetChildren(Child);
                foreach (RectTransform B_Child in B_Children)
                {
                    Placement placement = GetDefault(B_Child.name);
                    B_Child.anchoredPosition = placement.Position;
                    B_Child.localScale = placement.Size;
                    Settings.DeleteKey("MobileC_" + B_Child.name);
                }
                continue;
            }
            Placement placement2 = GetDefault(Child.name);
            Child.anchoredPosition = placement2.Position;
            Child.localScale = placement2.Size;
             Settings.DeleteKey("MobileC_" + Child.name);
        }
    }
}
