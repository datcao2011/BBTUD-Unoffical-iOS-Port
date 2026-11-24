using Multiplayer;
using UnityEngine;

public class ElevatorNPCScript : MonoBehaviour
{
    public bool up;

    private bool on;

    private Transform NPC;

    public ElevatorScript script;

    private GameObject[] waitingLine = new GameObject[100];

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    private bool done;

    bool nearby;

    private void Update()
    {
        if (script.unavailable)
        {
            if (num > 0)
            {
                RemoveWaitingPerson();
            }
            else if (on)
            {
                on = false;
                NPC = null;
            }
        }
        if (on && script.up == up & !script.elevating & !done)
        {
            done = true;
            if (MultiplayerManager.isActive)
            {
                if (MultiplayerManager.instance.isBaldi)
                {
                    if ((base.transform.position - NPC.position).magnitude < 25 && NPC.GetComponent<NPC>().wantsElevate) // Make sure to see if they still want to use the elevator (Bug fix)
                    {
                        ClientSend.SendElevatorEventFromPlayer(script.up, false);
                    }
                }
                if (IsControllingBaldiAI())
                {
                    if ((base.transform.position - NPC.position).magnitude < 25 && NPC.GetComponent<NPC>().wantsElevate) // Make sure to see if they still want to use the elevator (Bug fix)
                    {
                        NPC.GetComponent<NPC>().wantsElevate = false;
                        NPC.GetComponent<NPC>().usingElevate = true;
                        ClientSend.SendElevatorEventAI(script.up, false);
                        script.NPCOn(NPC.gameObject);
                    }
                }
            }
            else if ((base.transform.position - NPC.position).magnitude < 25 && NPC.GetComponent<NPC>().wantsElevate) // Make sure to see if they still want to use the elevator (Bug fix)
            {
                NPC.GetComponent<NPC>().wantsElevate = false;
                NPC.GetComponent<NPC>().usingElevate = true;
                script.NPCOn(NPC.gameObject);
            }

            if (waitingLine[0] == null & num <= 0)
            {
                on = false;
                done = false;
                NPC = null;
            }
            else
            {
                NPC = (waitingLine[0] == null) ? null : waitingLine[0].transform;
                RemoveWaitingPerson();
                if (NPC != null && waitingLine[0] == NPC.gameObject)
                {
                    waitingLine[0] = null;
                }

                while (NPC == null && num > 0)
                {
                    RemoveWaitingPerson();
                    NPC = waitingLine[0].transform;
                }

                if (NPC == null)
                {
                    on = false;
                }

                if (!MultiplayerManager.isActive)
                {
                    if (!script.up)
                    {
                        if (up)
                            script.ElevatorGo();
                    }
                    else
                    {
                        if (!up)
                            script.ElevatorGo();
                    }
                }
            }
        }
        if (script.elevating)
        {
            done = false;
        }
        if (MultiplayerManager.isActive && MultiplayerManager.instance.isBaldi)
        {
            if ((transform.position - MultiplayerManager.instance.gc.baldiTransform.position).magnitude <= 25 && MultiplayerManager.instance.gc.baldiScrpt.floor == (up ? 2 : 1))
            {
                MultiplayerManager.instance.gc.baldiScrpt.wantsElevate = true;
                nearby = true;
            }
            else if (nearby)
            {
                nearby = false;
                if (isBaldiInWaitingLine())
                {
                    MultiplayerManager.instance.gc.baldiScrpt.wantsElevate = false;
                }
            }
        }
    }

    private bool isBaldiInWaitingLine()
    {
        return MultiplayerManager.instance.gc.baldiScrpt.wantsElevate;
    }

    private void RemoveWaitingPerson()
    {
        for (int i = 1; i < num; i++)
        {
            waitingLine[i - 1] = waitingLine[i];
        }
        num--;
    }

    int num = 0;

    private bool IsControllingBaldiAI()
    {
        return MultiplayerManager.isActive && MultiplayerManager.instance.isAIBaldi && MultiplayerManager.instance.isMainPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPC")
        {
            if (other.GetComponent<NPC>().wantsElevate)
            {
                if (script.unavailable || (MultiplayerManager.isActive && other.name != "Baldi"))
                {
                    other.GetComponent<NPC>().wantsElevate = false;
                    other.GetComponent<NPC>().SetTarget(new Vector3(-41, 0, 304));
                    return;
                }
                if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || IsControllingBaldiAI()) && on)
                {
                    on = false;
                }
                if (!on)
                {
                    on = true;
                    NPC = other.transform;
                    if (!script.up)
                    {
                        if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || IsControllingBaldiAI()))
                        {
                            if (up && !script.elevating)
                            {
                                ClientSend.SendElevatorEventFromPlayer(script.up, true);
                            }
                        }
                        else if (!MultiplayerManager.isActive)
                        {
                            if (up)
                                script.ElevatorGo();
                        }
                    }
                    else
                    {
                        if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || IsControllingBaldiAI()))
                        {
                            if (!up && !script.elevating)
                            {
                                ClientSend.SendElevatorEventFromPlayer(script.up, true);
                            }
                        }
                        else if (!MultiplayerManager.isActive)
                        {
                            if (!up)
                                script.ElevatorGo();
                        }
                    }
                }
                else
                {
                    waitingLine[num] = other.gameObject;
                    num++;
                }
            }
        }
        if (other.tag == "Player" & !script.unavailable)
        {
            if (MultiplayerManager.isActive && !MultiplayerManager.instance.isBaldi)
            {
                if (!script.up)
                {
                    if (up && !script.elevating)
                    {
                        ClientSend.SendElevatorEventFromPlayer(script.up, true);
                    }
                }
                else
                {
                    if (!up && !script.elevating)
                    {
                        ClientSend.SendElevatorEventFromPlayer(script.up, true);
                    }
                }
            }
            else if (!MultiplayerManager.isActive)
            {
                if (!script.up)
                {
                    if (up)
                        script.ElevatorGo();
                }
                else
                {
                    if (!up)
                        script.ElevatorGo();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            return;
        }
        if (MultiplayerManager.isActive && (MultiplayerManager.instance.isBaldi || IsControllingBaldiAI()))
        {
            if (other.tag == "NPC" && other.GetComponent<NPC>().wantsElevate)
            {
                other.GetComponent<NPC>().wantsElevate = false;
                if (other.name == "Baldi")
                {
                    on = false;
                }
            }
            return;
        }
        if (other.tag == "NPC" && !other.GetComponent<NPC>().usingElevate && (waitingLine[0] != null || NPC != null))
        {
            if (NPC == other.gameObject)
            {
                if (waitingLine[0] == null)
                {
                    NPC = null;
                    on = false;
                    done = false;
                }
                else
                {
                    NPC = waitingLine[0].transform;
                    RemoveWaitingPerson();
                    if (waitingLine[0] == NPC.gameObject)
                    {
                        waitingLine[0] = null;
                    }

                    while (NPC == null)
                    {
                        RemoveWaitingPerson();
                        NPC = waitingLine[0].transform;
                    }

                    if (NPC == other.gameObject) // Same one
                    {
                        NPC = null;
                        on = false;
                        done = false;
                    }
                }
                return;
            }
            for (int i = 0; i < num; i++)
            {
                if (waitingLine[i] == other.gameObject)
                {
                    waitingLine[i] = null;
                    num--;
                }
            }
        }
    }
}