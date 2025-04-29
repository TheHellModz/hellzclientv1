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
using UnityEngine.UIElements;
using BepInEx;

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
            if (!wasButtonPressed && rightPrimary || Mouse.current.leftButton.isPressed) isOn = !isOn;
            wasButtonPressed = pollerInstance.rightControllerPrimaryButton;
            if (isOn) taggerInstance.offlineVRRig.enabled = false;
            else taggerInstance.offlineVRRig.enabled = true;
        }

        public static void FreezeRig()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat < 0.5 || Mouse.current.leftButton.isPressed)
            {
                taggerInstance.offlineVRRig.enabled = false;
                taggerInstance.offlineVRRig.transform.position = taggerInstance.headCollider.transform.position;
                taggerInstance.offlineVRRig.transform.rotation = taggerInstance.headCollider.transform.rotation;
            }
            else taggerInstance.offlineVRRig.enabled = true;
        }

        public static void NameChange(string name)
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.NickName = name;
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.NetworkingClient.LocalPlayer.NickName = name;
        }


        public static void HelicopterMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >0.5 || Mouse.current.rightButton.isPressed)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position += new UnityEngine.Vector3(0f, 0.075f, 0f);
                GorillaTagger.Instance.offlineVRRig.transform.rotation = UnityEngine.Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new UnityEngine.Vector3(0f, 10f, 0f));
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static float lastToggleTime = 0f;
        public static float delay = 0.4f;
        public static bool fakelagcooldown;
        public static void FakeLag()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >0.5 || Mouse.current.rightButton.isPressed)
            {
                float currentTime = Time.time;
                if (currentTime - lastToggleTime >= delay)
                {
                    fakelagcooldown = !fakelagcooldown;
                    lastToggleTime = currentTime;

                    GorillaTagger.Instance.offlineVRRig.enabled = fakelagcooldown;
                }
            }
        }

        public static void StillHeliMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5 || Mouse.current.rightButton.isPressed)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.rotation = UnityEngine.Quaternion.Euler(GorillaTagger.Instance.offlineVRRig.transform.rotation.eulerAngles + new UnityEngine.Vector3(0f, 10f, 0f));
                GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.offlineVRRig.transform.rotation;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * -1f;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + GorillaTagger.Instance.offlineVRRig.transform.right * 1f;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void InvisMonkey()
        {
            if (!wasButtonPressed && rightPrimary || Mouse.current.leftButton.isPressed) isOn = !isOn;
            wasButtonPressed = pollerInstance.rightControllerPrimaryButton;
            if (isOn)
            {
                taggerInstance.offlineVRRig.enabled = false;
                taggerInstance.offlineVRRig.transform.position = new Vector3(999, 999, 999);
            }
            else taggerInstance.offlineVRRig.enabled = true;
        }

        public static void HoldRig()
        {
            if (ControllerInputPoller.instance.rightGrab || Mouse.current.rightButton.isPressed)
            {
                taggerInstance.offlineVRRig.enabled = false;
                taggerInstance.offlineVRRig.transform.position = taggerInstance.rightHandTransform.transform.position;
                taggerInstance.offlineVRRig.transform.rotation = taggerInstance.rightHandTransform.transform.rotation;
            }
            else taggerInstance.offlineVRRig.enabled = true;

            if (ControllerInputPoller.instance.leftGrab || Mouse.current.leftButton.isPressed)
            {
                taggerInstance.offlineVRRig.enabled = false;
                taggerInstance.offlineVRRig.transform.position = taggerInstance.leftHandTransform.transform.position;
                taggerInstance.offlineVRRig.transform.rotation = taggerInstance.leftHandTransform.transform.rotation;
            }
            else taggerInstance.offlineVRRig.enabled = true;
        }

        public static void SpazArms()
        {
            if (pollerInstance.rightControllerIndexFloat > 0.5f || Mouse.current.leftButton.isPressed)
            {
                playerInstance.leftControllerTransform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), 4f));
                playerInstance.rightControllerTransform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), 4f));
                playerInstance.leftControllerTransform.position = taggerInstance.leftHandTransform.position + playerInstance.leftControllerTransform.forward *5f;
                playerInstance.leftControllerTransform.position = taggerInstance.rightHandTransform.position + playerInstance.leftControllerTransform.forward * 5f;
            }
        }

        public static void TinyRig()
        {
            if (pollerInstance.rightControllerIndexFloat > 0.5f || Mouse.current.leftButton.isPressed)
            {
                Traverse.Create(GorillaLocomotion.GTPlayer.Instance).Field("scaleMultipler").SetValue(0.1f);
            }
            else Traverse.Create(GorillaLocomotion.GTPlayer.Instance).Field("scaleMultipler").SetValue(1f);
        }

        public static void LargeRig()
        {
            if (pollerInstance.rightControllerIndexFloat > 0.5f || Mouse.current.leftButton.isPressed)
            {
                Traverse.Create(GorillaLocomotion.GTPlayer.Instance).Field("scaleMultipler").SetValue(5f);
            }
            else Traverse.Create(GorillaLocomotion.GTPlayer.Instance).Field("scaleMultipler").SetValue(1f);
        }

        public static void LongArms(bool setActive, Vector3 armLength)
        {
            if (setActive) playerInstance.transform.localScale = armLength;
            else playerInstance.transform.localScale = Vector3.one;
        }

        public static void tptoarea(Vector3 pos)
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5 || Mouse.current.leftButton.isPressed)
            {
                foreach (MeshCollider meshCollider in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider.enabled = false;
                playerInstance.transform.position = pos;
            }
            else foreach (MeshCollider meshCollider2 in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider2.enabled = true;
        }

        public static void tptostump()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 1 || Mouse.current.leftButton.isPressed)
            {
                foreach (MeshCollider meshCollider in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider.enabled = false;
                playerInstance.transform.position = new Vector3(66.9272f, 12.2415f, -82.822f);
            }
            else foreach (MeshCollider meshCollider2 in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider2.enabled = true;
        }
    }
}