using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine;
using Object = UnityEngine.Object;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Utilities.Variables;
using static HellzClient.Menu.Main;
using static HellzClient.Menu.ButtonHandler;
using static HellzClient.Menu.Optimizations;
using HellzClient.Utilities;
using HellzClient.Menu;
using static HellzClient.Mods.Categories.Movement;
using static HellzClient.Utilities.Patches.OtherPatches;
using static HellzClient.Utilities.GunLib;
using System.Linq;
using GorillaTag;

namespace HellzClient.Mods.Categories
{
    public class Settings
    {
        public static void SwitchHands(bool setActive)
        {
            rightHandedMenu = setActive;
        }

        public static void ClearNotifications()
        {
            NotificationLib.ClearAllNotifications();
        }

        public static void ToggleNotifications(bool setActive)
        {
            toggleNotifications = setActive;
        }

        public static void ToggleDisconnectButton(bool setActive)
        {
            toggledisconnectButton = setActive;
        }

        public static void ToggleOutline(bool setActive)
        {
            togglemenuOutline = setActive;
        }

        public  static void ToggleFunMods(bool setActive)
        {
            toggledfunbutton = setActive;
        }

        public static void ToggleSideButtons(bool setActive)
        {
            togglediscButton = setActive;
            toggledpanicButton = setActive;
        }



    }
}
