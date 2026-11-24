using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour
{
    public void Load(string thescene)
    {
        SceneManager.LoadScene(thescene);
    }

    public void AddLoad(string thescene)
    {
        if (SceneManager.sceneCount < 2)
        {
            SceneManager.LoadSceneAsync(thescene, LoadSceneMode.Additive);
        }
    }

    public void AsyncLoad(string theasyncscene)
    {
        SceneManager.LoadSceneAsync(theasyncscene);
    }
}
