using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using static HellzClient.Utilities.NotificationLib;
using static HellzClient.Utilities.Variables;
using static HellzClient.Menu.Main;
using HellzClient.Utilities;
using System.Diagnostics;
using Valve.VR;
using Cinemachine;
using HarmonyLib;
using System.Reflection;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using UnityEngine.XR;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace HellzClient.Mods.Categories
{
    public class Room : MonoBehaviourPunCallbacks
    {
        public static string roomCode;

        public static void QuitGTAG()
        {
            Application.Quit();
        }

        public static void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public static void JoinRandomPublic()
        {
            if (PhotonNetwork.InRoom)
            {
                UnityEngine.Debug.LogWarning("<color=blue>Photon</color> : Already connected to a room.");
                NotificationLib.SendNotification("<color=blue>Photon</color> : Already connected to a room.");
                return;
            }

            string currentMap = DetectCurrentMap();
            if (currentMap == null)
            {
                UnityEngine.Debug.LogError("<color=blue>Photon</color> : Unable to detect the current map.");
                NotificationLib.SendNotification("<color=blue>Photon</color> : Unable to detect the current map.");
                return;
            }

            string path = GetPathForGameMode(currentMap);
            if (path == null)
            {
                UnityEngine.Debug.LogError($"<color=blue>Photon</color> : No valid path found for map: {currentMap}.");
                NotificationLib.SendNotification($"<color=blue>Photon</color> : No valid path found for map: {currentMap}.");
                return;
            }

            GorillaNetworkJoinTrigger joinTrigger = GameObject.Find(path)?.GetComponent<GorillaNetworkJoinTrigger>();
            if (joinTrigger == null)
            {
                UnityEngine.Debug.LogError($"<color=blue>Photon</color> : Join trigger not found for path: {path}.");
                NotificationLib.SendNotification($"<color=blue>Photon</color> : Join trigger not found for path: {path}.");
                return;
            }

            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(joinTrigger, JoinType.Solo);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (returnCode == ErrorCode.GameFull)
            {
                UnityEngine.Debug.LogWarning($"OnJoinRoomFailed : Failed to join room '{roomCode}'. Reason: Is Full.");
                NotificationLib.SendNotification($"<color=red>Error</color> : Failed to join room '{roomCode}'. Reason: Is Full.");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"OnJoinRoomFailed: Failed to join room '{roomCode}'. Reason: {message}.");
                NotificationLib.SendNotification($"<color=red>Error</color>: Failed to join room '{roomCode}'. Reason: {message}.");
            }
        }
    }
}