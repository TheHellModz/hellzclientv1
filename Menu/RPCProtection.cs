using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HellzClient.Menu
{
    internal class RPCProtection
    {
        public static bool hasRTF;
        public static bool remove;

        public static void RPCProtectionMethod()
        {
            try
            {
                if (RPCProtection.hasRTF)
                {
                    remove = false;
                    if (PhotonNetwork.InRoom)
                    {
                        while (RPCProtection.hasRTF)
                        {
                            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                            raiseEventOptions.CachingOption = (EventCaching)6;
                            raiseEventOptions.TargetActors = new int[]
                            {
                            PhotonNetwork.LocalPlayer.ActorNumber
                            };
                            PhotonNetwork.NetworkingClient.OpRaiseEvent(200, null, raiseEventOptions, SendOptions.SendReliable);
                        }
                    }
                    else
                    {
                        GorillaNot.instance.rpcErrorMax = int.MaxValue;
                        GorillaNot.instance.rpcCallLimit = int.MaxValue;
                        GorillaNot.instance.logErrorMax = int.MaxValue;
                        PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                        PhotonNetwork.QuickResends = int.MaxValue;
                        PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
                        PhotonNetwork.SendAllOutgoingCommands();
                        GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
                    }
                }
            }
            catch
            {
                GorillaNot.instance.rpcErrorMax = int.MaxValue;
                GorillaNot.instance.rpcCallLimit = int.MaxValue;
                GorillaNot.instance.logErrorMax = int.MaxValue;
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;
                PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
                PhotonNetwork.SendAllOutgoingCommands();
                GorillaNot.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);

                UnityEngine.Debug.Log("Are you in a lobby?");
            }
        }
    }
}
