using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarApplier : MonoBehaviour
{
    public MeshRenderer[] Arms;
    public MeshRenderer Torso;
    public MeshRenderer[] Legs;
    public MeshRenderer[] Body;
    public MeshRenderer[] Hair;

    public GameObject[] Accessories;

    private void Start()
    {
        OptimizeRenderers();
    }

    private void OptimizeRenderers()
    {
        Shader cheap = Shader.Find("Legacy Shaders/Diffuse");
        if (cheap == null) cheap = Shader.Find("Mobile/Diffuse");
        if (cheap == null) return;
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;
            Material m = r.material;
            if (m != null && m.shader != null && m.shader.name == "Standard")
                m.shader = cheap;
        }
    }

    public void Set(string arms, string torso, string legs, string body, string hair)
    {
        Color current;
        if (ColorUtility.TryParseHtmlString(arms, out current))
        {
            SetColor(current, Arms);
        }
        if (ColorUtility.TryParseHtmlString(torso, out current))
        {
            SetColor(current, Torso);
        }
        if (ColorUtility.TryParseHtmlString(legs, out current))
        {
            SetColor(current, Legs);
        }
        if (ColorUtility.TryParseHtmlString(body, out current))
        {
            SetColor(current, Body);
        }
        if (ColorUtility.TryParseHtmlString(hair, out current))
        {
            SetColor(current, Hair);
        }
    }

    public string[] Get()
    {
        Color[] Colors = Get_Colors();
        string[] Result = new string[Colors.Length];
        for (int i = 0; i < Colors.Length; i++)
        {
            Result[i] = '#' + ColorUtility.ToHtmlStringRGB(Colors[i]);
        }
        return Result;
    }

    public string[] GetFull()
    {
        Color[] Colors = Get_Colors();
        string[] Result = new string[Colors.Length + 1];
        for (int i = 0; i < Colors.Length; i++)
        {
            Result[i] = '#' + ColorUtility.ToHtmlStringRGB(Colors[i]);
        }
        Result[Colors.Length] = GetAccessory().ToString();
        return Result;
    }

    public int GetAccessory()
    {
        int Result = 0;
        for (int i = 0; i < Accessories.Length; i++)
        {
            if (Accessories[i].activeSelf) Result = i + 1;
        }
        return Result;
    }

    public Color[] Get_Colors()
    {
        return new Color[5] { Arms[0].material.color, Torso.material.color, Legs[0].material.color, Body[0].material.color, Hair[0].material.color };
    }

    public void SetColor(Color color, MeshRenderer renderer) => SetColor(color, new MeshRenderer[1] { renderer });

    public void SetColor(Color color, MeshRenderer[] renderers)
    {
        color.a = 1f; // Make sure it's visible
        foreach (MeshRenderer BodyPart in renderers)
        {
            BodyPart.material.color = color;
        }
    }

    public void SetAccessory(int i)
    {
        foreach (GameObject renderer in Accessories)
        {
            renderer.SetActive(false);
        }
        if (i > 0)
        {
            Accessories[i - 1].SetActive(true);
        }
    }
}
