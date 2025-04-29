using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static HellzClient.Utilities.Variables;

namespace HellzClient.Mods.Categories
{
    internal class Tagging
    {
        public static void TagAll()
        {
            if (IAmInfected && pollerInstance.rightControllerIndexFloat > 0.5f || IAmInfected && Mouse.current.leftButton.isPressed)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (!RigIsInfected(rig))
                    {
                        taggerInstance.offlineVRRig.enabled = false;
                        taggerInstance.offlineVRRig.transform.position = rig.transform.position;
                        playerInstance.leftControllerTransform.position = rig.transform.position;
                    }
                }
            }
            else taggerInstance.offlineVRRig.enabled = true;
        }

        public static void TagSelf()
        {
            if (!IAmInfected && pollerInstance.rightControllerIndexFloat > 0.5f || !IAmInfected && Mouse.current.leftButton.isPressed)
            {
                foreach (VRRig rig in GorillaParent.instance.vrrigs)
                {
                    if (RigIsInfected(rig))
                    {
                        taggerInstance.offlineVRRig.enabled = false;
                        taggerInstance.offlineVRRig.transform.position = rig.transform.position;
                        playerInstance.leftControllerTransform.position = rig.transform.position;
                    }
                }
            }
            else taggerInstance.offlineVRRig.enabled= true;
        }
    }
}
