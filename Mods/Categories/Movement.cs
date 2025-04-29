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
using HellzClient.Utilities;
using System.Reflection;
using UnityEngine.InputSystem;

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

        public static void GravityPull(Vector3 pos)
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat < 0.5 || Mouse.current.leftButton.isPressed)
            {
                GorillaTagger.Instance.bodyCollider.attachedRigidbody.AddForce(pos * (Time.deltaTime * (22f / Time.deltaTime)), (ForceMode)50);
            }
        }

        public static void SpeedBoost(float speed, float jump)
        {
            playerInstance.maxJumpSpeed = speed;
            playerInstance.jumpMultiplier = jump;
            GorillaGameManager.instance.fastJumpLimit = speed;
            GorillaGameManager.instance.fastJumpMultiplier = jump;
        }

        public static void LowGravity()
        {
            GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (6.66f / Time.deltaTime)), ForceMode.Acceleration);
        }

        public static void ZeroGravity()
        {
            GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (9.81f / Time.deltaTime)), ForceMode.Acceleration);
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

        public static Vector3 wallPoint;
        public static Vector3 wallPoint2;
        public static float wallwalkStrength = -10f;

        public static void WallWalk(bool button1, bool button2)
        {
            if (playerInstance.wasLeftHandColliding || playerInstance.wasRightHandColliding)
            {
                FieldInfo fInfo = typeof(GorillaLocomotion.GTPlayer).GetField("lastHitInfoHand");
                if (fInfo != null)
                {
                    bool hasTouched;
                    bool isTouching;
                    object objValue = fInfo.GetValue(playerInstance);
                    if (objValue is RaycastHit)
                    {
                        raycastHit = (RaycastHit)objValue;
                        hasTouched = true;
                        isTouching = true;
                    }
                    else hasTouched = false && isTouching;
                    if (hasTouched)
                    {
                        wallPoint = raycastHit.point;
                        wallPoint2 = raycastHit.normal;
                    }
                }
                if (wallPoint != Vector3.zero && button1 || button2)
                {
                    playerInstance.bodyCollider.attachedRigidbody.AddForce(wallPoint * wallwalkStrength);
                }
            }
        }

        public static void FastSwim(float speed)
        {
            if (playerInstance.InWater)
            {
                playerInstance.gameObject.GetComponent<Rigidbody>().velocity *= speed;
            }
        }

       
    }
}