using System;
using System.Collections.Generic;
using System.Text;
using static HellzClient.Utilities.GunLib;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Utilities.Variables;
using static HellzClient.Menu.Main;
using static HellzClient.Mods.Categories.Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using HellzClient.Utilities;
using UnityEngine.XR;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR.Interaction.Toolkit;
using GorillaNetworking;
using HellzClient.Utilities.Patches;
using HellzClient.Menu;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using HarmonyLib;

namespace HellzClient.Mods.Categories
{
    public class Player
    {
        // General toggle and state variables
        private static bool isOn = false;
        private static bool wasButtonPressed = false;

        public static bool isOn2 = false;
        private static bool wasButtonPressed2 = false;

        public static void GhostMonke()
        {
            if (!wasButtonPressed && rightPrimary) isOn = !isOn;
            wasButtonPressed = pollerInstance.rightControllerPrimaryButton;
            if (isOn) taggerInstance.offlineVRRig.enabled = false;
            else taggerInstance.offlineVRRig.enabled = true;
        }

        public static void InvisMonkey()
        {
            if (!wasButtonPressed && rightPrimary) isOn = !isOn;
            wasButtonPressed = pollerInstance.rightControllerPrimaryButton;
            if (isOn)
            {
                taggerInstance.offlineVRRig.enabled = false;
                taggerInstance.offlineVRRig.transform.position = new Vector3(999, 999, 999);
            }
            else taggerInstance.offlineVRRig.enabled = true;

        }

        public static void LongArms(bool setActive, Vector3 armLength)
        {
            if (setActive) playerInstance.transform.localScale = armLength;
            else playerInstance.transform.localScale = Vector3.one;
        }

        public static void tptoarea(Vector3 pos)
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 1)
            {
                foreach (MeshCollider meshCollider in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider.enabled = false;
                playerInstance.transform.position = pos;
            }
            else foreach (MeshCollider meshCollider2 in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider2.enabled = true;
        }
    }
}