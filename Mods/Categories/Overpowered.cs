using System;
using System.Collections.Generic;
using System.Text;
using HellzClient.Menu;
using Photon.Pun;
using UnityEngine.InputSystem;

namespace HellzClient.Mods.Categories
{
    internal class Overpowered
    {
        

        public static void BreakAudio()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >0.5 || Mouse.current.leftButton.isPressed)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("PlayHandTap", (RpcTarget)1, new object[]
                {
                    111,
                    false,
                    999999f
                });
            }
            RPCProtection.RPCProtectionMethod();
        }



    }
}
