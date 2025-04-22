using HarmonyLib;
using HellzClient.Mods;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Utilities.GunLib;
using UnityEngine.Animations.Rigging;

namespace HellzClient.Utilities
{
    public class Variables : MonoBehaviourPunCallbacks
    {
        // --- UI Variables ---
        public static GameObject menuObj = null;
        public static GameObject background = null;
        public static GameObject border = null;
        public static GameObject canvasObj = null;
        public static GameObject clickerObj = null;
        public static GameObject disconnectButton = null;
        public static Text title;



        // -- V2 UI VARIABLES -- //
        public static bool V2Menu = false;
        public static GameObject menuObjV2 = null;
        public static GameObject backgroundV2 = null;
        public static GameObject canvasObjV2 = null;
        public static GameObject clickerObjV2 = null;
        public static Text titleV2;

        // --- UI Colors ---
        public static Color32 BackgroundColor = Red;
        public static Color32 BorderColor1 = Black;
        public static Color32 BackToStartButtonColor = Black;
        public static Color32 PageButtonsColor = Black;
        public static Color32 ModEnabledColor = new Color32(13, 13, 13, 255);
        public static Color32 ModDisabledColor = Black;
        public static Color32 ClickerColor = White;
        public static Color32 TitleTextColor = White;
        public static Color32 ModsTextColor = White;
        public static Color32 DisconnectButtonTextColor = White;
        public static Color32 BackToStartTextColor = White;
        public static Color32 PageButtonsTextColor = White;

        // --- Menu and Interaction Settings ---
        public static Category currentPage = Category.Home;
        public static int currentCategoryPage = 0;
        public static int ButtonsPerPage = 9;
        public static bool toggledisconnectButton = true;
        public static bool rightHandedMenu = false;
        public static bool toggleNotifications = true;
        public static bool PCMenuOpen = false;
        public static KeyCode PCMenuKey = KeyCode.LeftAlt;
        public static bool openMenu;
        public static bool menuOpen = false;
        public static bool InMenuCondition;
        public static bool InPcCondition;

        // -- Player Controller Variables --
        public static bool leftPrimary;
        public static bool leftSecondary;
        public static bool rightPrimary;
        public static bool rightSecondary;
        public static bool leftGrab;
        public static bool rightGrab;
        public static float leftTrigger;
        public static float rightTrigger;

        // --- Player and Movement Variables ---
        public static GorillaLocomotion.GTPlayer playerInstance;
        public static GorillaTagger taggerInstance;
        public static ControllerInputPoller pollerInstance;
        public static VRRig vrrig = null;
        public static Material vrrigMaterial = null;

        // --- Camera Variables ---
        public static GameObject thirdPersonCamera;
        public static GameObject shoulderCamera;
        public static GameObject TransformCam = null;
        public static bool didThirdPerson = false;
        public static GameObject cm;

        // --- Physics Variables ---
        public static Rigidbody currentMenuRigidbody = null;
        public static Vector3 previousVelocity = Vector3.zero;
        public const float velocityThreshold = 0.05f;
        public static Shader guiShader = Shader.Find("GUI/Text Shader");
        public static Shader uberShader = Shader.Find("GorillaTag/UberShader");

        // --- Game Mode Variables ---
        public static readonly Dictionary<string, string> gameModePaths = new Dictionary<string, string>
        {
            { "forest", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit" },
            { "city", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front" },
            { "canyons", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon" },
            { "mountains", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain For Computer" },
            { "beach", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach from Forest" },
            { "sky", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds" },
            { "basement", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer" },
            { "metro", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Metropolis from Computer" },
            { "arcade", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City frm Arcade" },
            { "rotating", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Rotating Map" },
            { "bayou", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - BayouComputer2" },
            { "caves", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave" }
        };

        // --- Utility Methods ---
        public static string DetectCurrentMap()
        {
            foreach (var entry in gameModePaths)
            {
                if (GameObject.Find(entry.Value) != null) return entry.Key;
            }
            return null;
        }

        public static string GetPathForGameMode(string gameMode)
        {
            gameModePaths.TryGetValue(gameMode.ToLower(), out string path);
            return path;
        }

        public static void IsModded()
        {
            if (!PhotonNetwork.IsConnected)
            {
                NotificationLib.SendNotification("<color=blue>Room</color> : You are not connected to a room.");
                return;
            }

            string message = GetHTMode().Contains("MODDED") ? "<color=blue>Room</color> : You are in a modded lobby." : "<color=blue>Room</color> : You are not in a modded lobby.";
            NotificationLib.SendNotification(message);
        }

        public static string GetHTMode()
        {
            if (PhotonNetwork.CurrentRoom == null || !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameMode"))
            {
                return "ERROR";
            }
            return PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString();
        }

        public static Photon.Realtime.Player NetPlayerToPhotonPlayer(NetPlayer netPlayer) => netPlayer.GetPlayerRef();

        public static NetPlayer PhotonPlayerToNetPlayer(Photon.Realtime.Player player)
        {
            VRRig rig = GorillaGameManager.instance.FindPlayerVRRig(player); 
            return rig != null ? rig.Creator : null; 
        }

        public static NetPlayer GetNetPlayerFromRig(VRRig vrrig) => vrrig.Creator;

        public static VRRig GetRigFromNetPlayer(NetPlayer netPlayer) => GorillaGameManager.instance.FindPlayerVRRig(netPlayer);

        public static PhotonView GetPhotonViewFromRig(VRRig vrrig)
        {
            return (PhotonView)Traverse.Create(vrrig).Field("photonView").GetValue();
        }

        public static PhotonView GetPhotonViewFromNetPlayer(NetPlayer netPlayer) => GetPhotonViewFromRig(GorillaGameManager.instance.FindPlayerVRRig(netPlayer));

        public static bool GetGamemode(string gamemodeName)
        {
            return GorillaGameManager.instance.GameModeName().ToLower().Contains(gamemodeName.ToLower());
        }

        public static bool IsOtherPlayer(VRRig rig) => rig != null && rig != taggerInstance.offlineVRRig && !rig.isOfflineVRRig && !rig.isMyPlayer;

        public static bool IAmInfected => taggerInstance.offlineVRRig != null && RigIsInfected(taggerInstance.offlineVRRig);

        public static bool RigIsInfected(VRRig vrrig)
        {
            string materialName = vrrig.mainSkin.material.name;
            return materialName.Contains("fected") || materialName.Contains("It");
        }

        public static void IsMasterCheck()
        {
            if (!PhotonNetwork.IsConnected)
            {
                NotificationLib.SendNotification("<color=blue>Room</color> : You are not connected to a room.");
                return;
            }

            NotificationLib.SendNotification(PhotonNetwork.IsMasterClient ? "<color=blue>Room</color> : You are master." : "<color=blue>Room</color> : You are not master.");
        }

        public static bool InLobby() => PhotonNetwork.InLobby;
        public static bool IsMaster() => PhotonNetwork.IsMasterClient;
    }
}
