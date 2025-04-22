using System;
using System.Collections.Generic;
using System.Text;
using static HellzClient.Utilities.GunLib;
using static HellzClient.Menu.Main;
using static HellzClient.Utilities.Variables;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Menu.ButtonHandler;
using static HellzClient.Mods.ModButtons;
using static HellzClient.Mods.Categories.Settings;
using UnityEngine;
using static HellzClient.Mods.Categories.Methods;

namespace HellzClient.Mods.Categories
{
    public class Movement
    {
        public static void Noclip()
        {
            bool enableColliders = rightTrigger > 0.5f;

            foreach (MeshCollider meshCollider in Resources.FindObjectsOfTypeAll<MeshCollider>()) meshCollider.enabled = enableColliders;
        }

        public static void Fly(bool button)
        {
            if (button)
            {
                playerInstance.transform.position += (playerInstance.headCollider.transform.forward * Time.deltaTime) * 25;
                playerInstance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void SpeedBoost(float speed, float jump)
        {
            playerInstance.maxJumpSpeed = speed;
            playerInstance.jumpMultiplier = jump;
            GorillaGameManager.instance.fastJumpLimit = speed;
            GorillaGameManager.instance.fastJumpMultiplier = jump;
        }


        public static void NormalPlats()
        {
            if (leftGrab && LPlat1 == null) Methods.CreatePlatform1(ubershader, leftColor, Methods.Hand.Left);
            else if (!leftGrab && LPlat1 != null) GameObject.Destroy(LPlat1);
            if (rightGrab && RPlat1 == null) Methods.CreatePlatform1(ubershader, rightColor, Methods.Hand.Right);
            else if (!rightGrab && RPlat1 != null) GameObject.Destroy(RPlat1);
        }

        public static void InvisPlats()
        {
            if (leftGrab && LPlat == null) CreatePlatform1(null, leftColor, Methods.Hand.Left);
            else if (!leftGrab && LPlat != null) GameObject.Destroy(LPlat);
            if (rightGrab && RPlat == null) CreatePlatform1(null, rightColor, Methods.Hand.Right);
            else if (!rightGrab && RPlat != null) GameObject.Destroy(RPlat);
        }
    }
}