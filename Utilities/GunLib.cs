using UnityEngine;
using UnityEngine.InputSystem;
using static HellzClient.Utilities.Variables;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Menu.Main;
using HellzClient.Menu;
using GorillaLocomotion;

namespace HellzClient.Utilities
{
    public class GunLib
    {
        public static VRRig lockedTargetRig;
        public static VRRig potentialTargetRig;
        public static GameObject gunPointer = null;
        public static bool GunGrip => pollerInstance.rightGrab;
        public static bool GunTrigger => pollerInstance.rightControllerIndexFloat > 0.1f;
        public static RaycastHit raycastHit;


        public static void SetupGunObjectPositions(Vector3 pos)
        {
            if (gunPointer == null)
            {
                gunPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Renderer renderer = gunPointer.GetComponent<Renderer>();
                renderer.material.shader = Shader.Find("GUI/Text Shader");
                Object.Destroy(gunPointer.GetComponent<Rigidbody>());
                Object.Destroy(gunPointer.GetComponent<SphereCollider>());
                gunPointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }

            Color32 pointerColor = GunTrigger ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);

            gunPointer.transform.position = pos;
            MeshRenderer gunRenderer = gunPointer.GetComponent<MeshRenderer>();
            if (gunRenderer != null)
            {
                gunRenderer.material.color = pointerColor;
            }

            SetGunVisibility(true);
        }

        public static void SetupRaycast()
        {
            Ray ray = new Ray(playerInstance.rightControllerTransform.position - playerInstance.rightControllerTransform.up, -playerInstance.rightControllerTransform.up);

            Physics.Raycast(ray, out raycastHit, 100f);

            if (lockedTargetRig != null)
            {
                SetupGunObjectPositions(lockedTargetRig.transform.position);
            }
            else
            {
                SetupGunObjectPositions(raycastHit.point);
            }
        }

        public static void SetGunVisibility(bool isVisible)
        {
            if (gunPointer != null)
            {
                gunPointer.SetActive(isVisible);
            }
        }

        public static void NoLockOnGunTest()
        {
            if (GunGrip)
            {
                SetupRaycast(); // Spawns the Pointer 

                if (GunTrigger)
                {
                    // Code That Executes When Pressing Trigger
                }
                else
                {
                    // Optional (What Happens When Letting Go Of Trigger)
                }
            }
            else
            {
                SetGunVisibility(false); // Hides the Pointer when letting go 
            }
        }

        public static void LockOnGunTest()
        {
            if (GunGrip)
            {
                SetupRaycast(); // Spawns the Pointer

                potentialTargetRig = raycastHit.collider?.GetComponentInParent<VRRig>(); // Finds A VRRig if the raycast hits a collider

                if (GunTrigger)
                {
                    if (lockedTargetRig == null)
                    {
                        lockedTargetRig = potentialTargetRig; // (DONT REMOVE) 
                    }
                    else
                    {
                        // Code That Executes When Pressing Trigger
                    }
                }
                else
                {
                    // Optional (What Happens When Letting Go Of Trigger)
                    lockedTargetRig = null; // (DONT REMOVE) Makes the target null so the target resets when letting go 
                }
            }
            else
            {
                lockedTargetRig = null; // (DONT REMOVE) Makes the target null so the target resets when letting go 
                SetGunVisibility(false); // Hides the Pointer when letting go 
            }
        }
    }
}
