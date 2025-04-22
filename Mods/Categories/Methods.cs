using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.XR;
using static HellzClient.Utilities.Variables;

namespace HellzClient.Mods.Categories
{
    internal static class Methods
    {

        // -- PLATFORM SETUP STUFF -- //

        internal enum Hand { Left, Right }

        internal static readonly Vector3 defplat = new Vector3(0.28f, 0.015f, 0.28f);
        internal static readonly Shader ubershader = Shader.Find("GorillaTag/UberShader");

        internal static readonly Color leftColor = Color.blue;
        internal static readonly Color rightColor = Color.red;

        internal static GameObject LPlat = null;
        internal static GameObject RPlat = null;

        private static void SetupPlatform(ref GameObject plat, Transform handTransform, Shader render, Color plcolor)
        {
            plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.transform.localScale = defplat;
            plat.transform.position = handTransform.position;
            plat.transform.rotation = handTransform.rotation;

            var renderer = plat.GetComponent<MeshRenderer>();
            renderer.material.shader = render;
            renderer.material.color = plcolor;
        }

        internal enum Hand1 { Left, Right }

        internal static readonly Vector3 defplat1 = new Vector3(0.28f, 0.015f, 0.28f);
        internal static readonly Shader ubershader1 = Shader.Find("GorillaTag/UberShader");

        internal static readonly Color leftColor1 = Color.blue;
        internal static readonly Color rightColor1 = Color.red;

        internal static GameObject LPlat1 = null;
        internal static GameObject RPlat1 = null;

        private static void SetupPlatform1(ref GameObject plat, Transform handTransform, Shader render, Color plcolor)
        {
            plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.transform.localScale = defplat;
            plat.transform.position = handTransform.position;
            plat.transform.rotation = handTransform.rotation;

            var renderer = plat.GetComponent<MeshRenderer>();
            renderer.material.shader = render;
            renderer.material.color = plcolor;
        }
        public static void CreatePlatform1(Shader render, Color plcolor, Hand hand1)
        {
            if (render == null)
            {
                Debug.LogWarning("CreatePlatform failed: Shader is null.");
                return;
            }

            switch (hand1)
            {
                case Hand.Left when LPlat == null:
                    SetupPlatform(ref LPlat1, taggerInstance.leftHandTransform, render, plcolor);
                    break;

                case Hand.Right when RPlat == null:
                    SetupPlatform(ref RPlat1, taggerInstance.rightHandTransform, render, plcolor);
                    break;
            }
        }
    }
}