using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using SchoolSaves;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Multiplayer
{
    public static class ClientSend
    {
        public static void WelcomeReceived()
        {
            // using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
            // {
            //    // packet.Write(MultiplayerManager.instance.client.ID);
            //     packet.Write(PlayerPrefs.GetString("AccountSession"));
            //     SendTCPData(packet);
            // }
        }

        public static void PlayerMovement(Vector3 Movement, Vector3 Rotation)
        {
            // using (Packet packet = new Packet((int)ClientPackets.Movement))
            // {
            //     packet.Write(Movement.x);
            //     packet.Write(Movement.y);
            //     packet.Write(Movement.z);
            //     SendUDPData(packet);
            // }
            if (MultiplayerManager.instance.HasMoreThan2Players)
            {
                MultiplayerManager.instance.client.SendAsUDP(3, Photon.Realtime.ReceiverGroup.Others, PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.UserId, Movement.ToFloat(), Rotation.ToFloat());
                return;
            }
            MultiplayerManager.instance.client.SendAsUDP(1, Photon.Realtime.ReceiverGroup.Others, Movement.ToFloat(), Rotation.ToFloat());
        }

        public static void NPCMovement(string name, Vector3 Movement, Vector3 Rotation)
        {
            // using (Packet packet = new Packet((int)ClientPackets.NPCMovement))
            // {
            //     packet.Write(name);
            //     packet.Write(Movement.x);
            //     packet.Write(Movement.y);
            //     packet.Write(Movement.z);
            //     SendUDPData(packet);
            // }
            // #if HAS_EVENT_PLUGGED
            //             if (CountDownSyncing.e001)
            //             {
            //                 return;
            //             }
            // #endif
            MultiplayerManager.instance.client.SendAsUDP(2, Photon.Realtime.ReceiverGroup.Others, name, Movement.ToFloat(), Rotation.ToFloat());
        }

        // public static void BananaMovement(int ID, Vector3 Movement, Vector3 Velocity)
        // {
        //     MultiplayerManager.instance.client.SendAsUDP(7, Photon.Realtime.ReceiverGroup.Others, ID, Movement.ToFloat(), Velocity.ToFloat());
        // }

        public static void NPCRotation(string name, Quaternion Movement)
        {
            // using (Packet packet = new Packet((int)ClientPackets.NPCRotation))
            // {
            //     packet.Write(name);
            //     packet.Write(Movement.x);
            //     packet.Write(Movement.y);
            //     packet.Write(Movement.z);
            //     packet.Write(Movement.w);
            //     SendUDPData(packet);
            // }

        }

        public static void PlaySoundClip(string name, string soundName)
        {
            // using (Packet packet = new Packet((int)ClientPackets.SoundClip))
            // {
            //     packet.Write(name);
            //     packet.Write(soundName);
            //     SendTCPData(packet);
            // }
            MultiplayerManager.instance.client.PUN_Init("PUN_PlayAudio", RpcTarget.Others, name, soundName);
        }

        public static void StartJumpropeEvent()
        {
            MultiplayerManager.instance.client.PUN_Init("PUN_StartJumprope", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public static void EndJumpropeEvent(bool CutRope)
        {
            MultiplayerManager.instance.client.PUN_Init("PUN_EndJumprope", RpcTarget.All, CutRope);
        }

        public static void RandomEvent(int EventID)
        {
            MultiplayerManager.instance.client.PUN_Init("PUN_RandomEvent", RpcTarget.AllViaServer, EventID);
        }

        public static void SendCharacterState(string name, int State)
        {
            MultiplayerManager.instance.client.PUN_Init("StateCharacterSet", RpcTarget.Others, name, State);
        }
        public static void SendElevatorEventFromPlayer(bool Up, bool IsCooldown)
        {
            MultiplayerManager.instance.client.SendElevatorEvent(Up, IsCooldown);
        }

        public static void SendElevatorEventAI(bool Up, bool IsCooldown)
        {
            MultiplayerManager.instance.client.SendElevatorEvent_AI(Up, IsCooldown);
        }

        public static void SendActionToOtherPlayer(string action)
        {
            // using (Packet packet = new Packet((int)ClientPackets.ReceiveAction))
            // {
            //     packet.Write(action);
            //     SendTCPData(packet);
            // }
            MultiplayerManager.instance.client.PUN_Init("PUN_Action", RpcTarget.Others, action, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.NickName);
        }

        public static void DropItem(int ItemId, float[] Position)
        {
            MultiplayerManager.instance.client.PUN_Init("ItemDropped", RpcTarget.Others, ItemId, Position);
        }

        public static void SendAction(string action)
        {
            // using (Packet packet = new Packet((int)ClientPackets.Captured))
            // {
            //     packet.Write(action);
            //     SendTCPData(packet);
            // }
#if UNITY_EDITOR || NETWORK_CONSOLE
            MultiplayerManager.instance.client.PUN_Init("PUN_Action", RpcTarget.All, action, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.NickName);
#endif
        }

        public static void BSODA(float[] Position, float[] Rotation)
        {
            // using (Packet packet = new Packet((int)ClientPackets.Captured))
            // {
            //     packet.Write(action);
            //     SendTCPData(packet);
            // }
            string Pos = Position[0].ToString(CultureInfo.InvariantCulture) + "," + Position[1].ToString(CultureInfo.InvariantCulture) + "," + Position[2].ToString(CultureInfo.InvariantCulture);
            string Rot = Rotation[0].ToString(CultureInfo.InvariantCulture) + "," + Rotation[1].ToString(CultureInfo.InvariantCulture) + "," + Rotation[2].ToString(CultureInfo.InvariantCulture);
            MultiplayerManager.instance.client.PUN_Init("PUN_Action", RpcTarget.AllViaServer, "bsoda:" + Pos + ":" + Rot, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.NickName);
        }

        public static void MessageSend(string message)
        {
            string user = PhotonNetwork.IsConnected ? PhotonNetwork.NickName : "Player";
            if (AccountManager.hasInstance() && !string.IsNullOrEmpty(AccountManager.getSessionId()))
            {
                user = AccountManager.getUsername();
            }
            // using (Packet packet = new Packet((int)ClientPackets.Chat))
            // {
            //     packet.Write(user);
            //     packet.Write(message);
            //     SendTCPData(packet);
            // }
            MultiplayerManager.instance.client.PUN_Init("PUN_Message", RpcTarget.AllViaServer, user, message);
        }


        public static void SetAlarm(Vector3 Position)
        {
            float[] Pos = Position.ToFloat();
            List<int> Actors = new List<int>();
            Player[] OtherPlayers = PhotonNetwork.PlayerListOthers;
            foreach (Player p in OtherPlayers)
            {
                if (MultiplayerManager.instance.client.CurrentActor_Baldi != p.ActorNumber)
                    Actors.Add(p.ActorNumber);
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, TargetActors = Actors.ToArray() }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(8, Pos, raiseEventOptions, SendOptions.SendReliable);
        }
        public static void Tick(int Seconds, Vector3 Position)
        {
            object[] content = new object[2];
            content[0] = Seconds;
            content[1] = Position.ToFloat();
            List<int> Actors = new List<int>();
            Player[] OtherPlayers = PhotonNetwork.PlayerListOthers;
            foreach (Player p in OtherPlayers)
            {
                if (MultiplayerManager.instance.client.CurrentActor_Baldi != p.ActorNumber)
                    Actors.Add(p.ActorNumber);
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others, TargetActors = Actors.ToArray() }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(9, content, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}