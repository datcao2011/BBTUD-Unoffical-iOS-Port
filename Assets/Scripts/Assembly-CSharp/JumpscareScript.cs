using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class JumpscareScript : MonoBehaviour
{
    public Transform baldiTransform;

    public GameObject crazyText;

    public GameObject playerObject;

    public GameObject baldi;

    public Transform player;

    public float openingDistance;

    public GameControllerScript gc;

    public int clicks;

    public GameObject hiddenLink;

    private bool IsOptimized;

    private void Start()
    {
        IsOptimized = Settings.GetBool("Optimize", GameOptimizer.OptimizationDefault);
        if (IsOptimized || MultiplayerManager.isActive)
            enabled = false;
    }

    private void Update()
    {
        if (InteractScript.Intracting && !IsOptimized)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "JumpscareDoor")
                {
                    clicks++;
                    if (clicks == 5)
                    {
                        hiddenLink.SetActive(true);
                        gc.PauseGame();
                    }
                    else if (clicks == 10)
                    {
                        gc.baldiScrpt.SpoopyJumpscare();
                    }
                    else
                    {
                        crazyText.SetActive(true);
                    }
                }
            }
        }
    }
}
