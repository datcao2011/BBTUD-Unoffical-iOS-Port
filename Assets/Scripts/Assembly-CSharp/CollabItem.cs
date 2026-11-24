using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollabItem : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    private GameObject click;

    [SerializeField]
    private GameObject clickCursor;

    private bool Purchased;

    public int Item;

    public int Cost;

    [SerializeField]
    private MattsStore Store;

    [SerializeField]
    private MeshRenderer Sign;

    [SerializeField]
    private Material SoldOut;

    public TMP_Text CostText;

    public SpriteRenderer sprite;

    private static bool BoughtSomething;
    private static bool HasCalledNotEnough;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        BoughtSomething = false;
        HasCalledNotEnough = false;
    }

    // Update is called once per frame
    private void Update()
    {
        bool isHovering = false;
        if (!Purchased && Vector3.Distance(player.position, base.transform.position) < 10f)
        {
            Ray ray = Camera.main.ScreenPointToRay(ReticleScript.Position());
            RaycastHit raycastHit;
            isHovering = Physics.Raycast(ray, out raycastHit) && (raycastHit.transform == transform);
            if (InteractScript.Intracting & Time.timeScale != 0f)
            {
                if (isHovering)
                {
                    int Amount = Store.Get();
                    if (Cost > Amount)
                    {
                        Store.MattAnim.SetBool("Sad", true);
                        if (!HasCalledNotEnough)
                        {
                            HasCalledNotEnough = true;
                            Store.Matt.Stop();
                            if (CollabItems.Instance.GameController.CM.isActiveAndEnabled)
                            {
                                CollabItems.Instance.GameController.CM.DestroyAllCaptions(false);
                            }
                            Store.Matt.PlayOneShotWC(Store.MattNotEnough);
                        }
                        else if (!Store.Matt.isPlaying)
                        {
                            Store.Matt.PlayOneShotWC(Store.MattNotEnough);
                        }
                    }
                    else
                    {
                        Store.MattAnim.SetBool("Sad", false);
                        Store.Buy(Cost);
                        if (!BoughtSomething)
                        {
                            BoughtSomething = true;
                            Store.Matt.Stop();
                            if (CollabItems.Instance.GameController.CM.isActiveAndEnabled)
                            {
                                CollabItems.Instance.GameController.CM.DestroyAllCaptions(false);
                            }
                            Store.Matt.PlayOneShotWC(Store.MattPurchase);
                        }
                        else if (!Store.Matt.isPlaying)
                        {
                            Store.Matt.PlayOneShotWC(Store.MattPurchase);
                        }
                        CollabItems.Instance.Collect(Item);
                        Purchased = true;
                        Sign.material = SoldOut;
                        sprite.gameObject.SetActive(false);
                    }
                }
            }
        }
        // This was moved outside so it won't glitch out
        if (isHovering)
        {
            if (click == null)
            {
                click = Object.Instantiate(clickCursor);
                Destroy(click, 10f);
            }
        }
        else
        {
            if (click != null)
                Object.Destroy(click);
        }
    }
}
