using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadersManager : MonoBehaviour
{
    [HideInInspector]
    private MeshRenderer[] MeshRenderers;
    public Shader StandardShader;
    public Shader DiffuseShader;

    private bool ShadersCompleted;

    public bool IsSchool;
    public WindowScript Window;

    List<Material> Materials = new List<Material>();

    [SerializeField]
    private Material[] Skipped;

    // Start is called before the first frame update
    private void Start()
    {
        // Since this saves, we have to disable it on the editor
#if !UNITY_EDITOR
        Settings.OnSettingChanged += OnValChange;
        OnValChange("1.4Shaders", Settings.GetBool("1.4Shaders"));
#endif
    }

    private void OnDisable()
    {
        Settings.OnSettingChanged -= OnValChange;
    }

    private void OnValChange(string name, object val)
    {
        if (name == "1.4Shaders")
        {
            bool Value = (bool)val;
            if (Value && (MeshRenderers == null || MeshRenderers.Length <= 0))
            {
                MeshRenderers = (MeshRenderer[])Resources.FindObjectsOfTypeAll(typeof(MeshRenderer));
            }
            if (MeshRenderers != null && ShadersCompleted != Value)
            {
                ShadersCompleted = Value;
                Materials.AddRange(Skipped);
                foreach (MeshRenderer renderer in MeshRenderers)
                {
                    try
                    {
                        if (!Materials.Contains(renderer.sharedMaterial) && !renderer.GetComponent<TMPro.TMP_Text>() && !renderer.GetComponent<UnityEngine.UI.Text>() && renderer.sharedMaterial.name != "Sprites_NewDefault")
                        {
                            renderer.sharedMaterial.shader = Value ? DiffuseShader : StandardShader;
                            Materials.Add(renderer.sharedMaterial);
                        }
                    }
#if DEBUG
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
#else
                    catch
                    {
                        
                    }
#endif
                }
                Materials.Clear();
                #region Doors
                List<string> UsedMaterials = new List<string>();
                foreach (DoorScript script in Resources.FindObjectsOfTypeAll<DoorScript>())
                {
                    if (!UsedMaterials.ToList().Contains(script.open.name))
                    {
                        UsedMaterials.Add(script.open.name);
                        script.open.shader = Value ? DiffuseShader : StandardShader;
                        if (script.locked != null)
                        {
                            script.locked.shader = Value ? DiffuseShader : StandardShader;
                        }
                    }
                }
                foreach (SwingingDoorScript script in Resources.FindObjectsOfTypeAll<SwingingDoorScript>())
                {
                    if (!UsedMaterials.ToList().Contains(script.open.name))
                    {
                        UsedMaterials.Add(script.open.name);
                        script.open.shader = Value ? DiffuseShader : StandardShader;
                        if (script.locked != null)
                        {
                            script.locked.shader = Value ? DiffuseShader : StandardShader;
                        }
                    }
                }
                if (IsSchool && Window)
                {
                    Window.DarkBroken.shader = Value ? DiffuseShader : StandardShader;
                    Window.Broken.shader = Value ? DiffuseShader : StandardShader;
                }
                #endregion
            }
        }
    }
}
