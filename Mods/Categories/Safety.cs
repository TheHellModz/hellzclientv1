using HellzClient.Utilities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using static HellzClient.Utilities.Variables;

namespace HellzClient.Mods.Categories
{
    public static class Safety
    {
        public static void Antireportdisconnect()
        {
            var scoreboardLine = GorillaScoreboardTotalUpdater.allScoreboardLines.Find((GorillaPlayerScoreboardLine L) => L.playerVRRig.isLocal);
            foreach (VRRig vrrigs in GorillaParent.instance.vrrigs)
            {
                var Limit = 0.51f;
                var ReportPosition = scoreboardLine.reportButton.gameObject.transform.position;
                var RightDis = Vector3.Distance(vrrigs.rightHandTransform.position, ReportPosition);
                var LeftDis = Vector3.Distance(vrrigs.leftHandTransform.position, ReportPosition);
                if (RightDis <= Limit || LeftDis <= Limit)
                {
                    if (!vrrigs.isLocal)
                    {
                        PhotonNetwork.Disconnect();
                        NotificationLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> <color=white>Someone attempted to report you, you have been disconnected.</color>");

                    }
                }
            }
        }

        public static void DisableFingers()
        {
            pollerInstance.leftControllerGripFloat = 0.0f;
            pollerInstance.leftControllerIndexFloat = 0.0f;
            pollerInstance.rightControllerGripFloat = 0.0f;
            pollerInstance.rightControllerIndexFloat = 0.0f;
        }
    }
}