using HellzClient.Mods;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static HellzClient.Utilities.Variables;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Menu.Optimizations;
using static HellzClient.Menu.ButtonHandler;
using static HellzClient.Mods.Categories.Room;
using BepInEx;
using UnityEngine.InputSystem;
using HarmonyLib;
using static HellzClient.Initialization.PluginInfo;
using HellzClient.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace HellzClient.Mods.Categories
{
    internal class Visual
    {

        public static void LineESP()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (vrrig != GorillaTagger.Instance.offlineVRRig)
                {
                    GameObject gameObject = new GameObject("Line");
                    LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
                    float h = (Time.frameCount / 180f) % 1f;
                    float h2 = (Time.frameCount / 300f) % 1f;
                    UnityEngine.Color color = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
                    UnityEngine.Color color2 = UnityEngine.Color.HSVToRGB(h2, 1f, 1f);
                    color.a = 0.4f;
                    color2.a = 0.4f;
                    lineRenderer.startColor = color;
                    lineRenderer.endColor = color2;
                    lineRenderer.startWidth = 0.05f;
                    lineRenderer.endWidth = 0.05f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.useWorldSpace = true;
                    lineRenderer.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    lineRenderer.SetPosition(1, vrrig.transform.position);
                    lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
                    UnityEngine.Object.Destroy(gameObject, Time.deltaTime);
                }
            }
        }

        public static void WireFrameESP()
        {
            foreach (VRRig obj in GorillaParent.instance.vrrigs)
            {
                if (!obj.isOfflineVRRig)
                {
                    if (obj.mainSkin.material.name.Contains("fected") || obj.mainSkin.material.name.Contains("It"))
                    {
                        Vector3 scaledHitbox = new Vector3(obj.transform.localScale.x * 0.5f, obj.transform.localScale.y * 0.7f, obj.transform.localScale.z * 0.5f);
                        Quaternion rotationn = obj.transform.rotation;
                        Vector3 localPosnigga = obj.transform.position;
                        {
                            DrawWireframeBox(localPosnigga - new Vector3(0f, 0.075f, 0f), rotationn, scaledHitbox, new Color32(255, 255, 255, 255));
                        }
                    }
                    else
                    {
                        Vector3 scaledHitbox = new Vector3(obj.transform.localScale.x * 0.5f, obj.transform.localScale.y * 0.7f, obj.transform.localScale.z * 0.5f);
                        Quaternion rotationn = obj.transform.rotation;
                        Vector3 localPosnigga = obj.transform.position;
                        {
                            DrawWireframeBox(localPosnigga - new Vector3(0f, 0.075f, 0f), rotationn, scaledHitbox, new Color32(255, 255, 255, 255));
                        }
                    }
                }
            }
        }

        public class CashVisuals
        {
            private static Dictionary<VRRig, List<LineRenderer>> lines = new Dictionary<VRRig, List<LineRenderer>>();

            public static Color ModeColor(VRRig vrrig)
            {
                Color color = Color.white;
                switch (GorillaGameManager.instance.GameModeName())
                {
                    case "CASUAL":
                        color = Color.red;
                        break;
                    case "INFECTION":
                        color = vrrig.mainSkin.material.name.Contains("fected") ? Color.red : Color.green;
                        break;
                    case "GUARDIAN":
                        color = Color.red;
                        break;
                }
                return color;
            }

            public static int[] bones = { 4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7 };

            public static void BoxESP(bool isActive)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (!lines.ContainsKey(vrrig))
                        {
                            List<LineRenderer> linerList = new List<LineRenderer>();

                            for (int i = 0; i < 4; i++)
                            {
                                GameObject lineObj = new GameObject("InfectionTracer");
                                lineObj.transform.SetParent(vrrig.transform, false);
                                LineRenderer liner = lineObj.AddComponent<LineRenderer>();
                                liner.material = new Material(guiShader);
                                liner.startWidth = 0.01f;
                                liner.endWidth = 0.01f;
                                liner.useWorldSpace = true;
                                liner.positionCount = 2;
                                linerList.Add(liner);
                            }
                            lines[vrrig] = linerList;
                        }
                        foreach (var liner in lines[vrrig])
                        {
                            liner.enabled = isActive;
                        }

                        if (isActive)
                        {
                            UpdateTracerPositions(vrrig);
                        }
                    }
                }
            }

            private static void UpdateTracerPositions(VRRig vrrig)
            {
                if (!lines.ContainsKey(vrrig)) return;
                List<LineRenderer> linerList = lines[vrrig];
                Vector3 myRigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
                Vector3 directionToRig = (myRigPosition - vrrig.transform.position).normalized;
                Vector3 offset = Vector3.Cross(directionToRig, Vector3.up) * 0.26f;
                Vector3 bottomLeft = vrrig.transform.position + offset + new Vector3(0, -0.5f, 0);
                Vector3 bottomRight = vrrig.transform.position - offset + new Vector3(0, -0.5f, 0);
                Vector3 topLeft = vrrig.transform.position + offset + new Vector3(0, 0.5f, 0);
                Vector3 topRight = vrrig.transform.position - offset + new Vector3(0, 0.5f, 0);
                linerList[0].SetPosition(0, bottomLeft);
                linerList[0].SetPosition(1, topLeft);
                linerList[1].SetPosition(0, bottomRight);
                linerList[1].SetPosition(1, topRight);
                linerList[2].SetPosition(0, topLeft);
                linerList[2].SetPosition(1, topRight);
                linerList[3].SetPosition(0, bottomLeft);
                linerList[3].SetPosition(1, bottomRight);
                foreach (var liner in linerList)
                {
                    liner.startColor = ModeColor(vrrig);
                    liner.endColor = ModeColor(vrrig);
                }
            }

            public static void SkeletonESP(bool isActive)
            {
                if (isActive)
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig != GorillaTagger.Instance.offlineVRRig)
                        {
                            if (!vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>())
                            {
                                vrrig.head.rigTarget.gameObject.AddComponent<LineRenderer>();
                            }
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().endWidth = 0.025f;
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().startWidth = 0.025f;
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().material.shader = Shader.Find("GUI/Text Shader");
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().material.color = vrrig.mainSkin.material.color;
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().SetPosition(0, vrrig.head.rigTarget.transform.position + new Vector3(0, 0.160f, 0));
                            vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>().SetPosition(1, vrrig.head.rigTarget.transform.position - new Vector3(0, 0.4f, 0));

                            for (int i = 0; i < bones.Length; i += 2)
                            {
                                if (!vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>())
                                {
                                    vrrig.mainSkin.bones[bones[i]].gameObject.AddComponent<LineRenderer>();
                                }
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().endWidth = 0.025f;
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().startWidth = 0.025f;
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().material.shader = Shader.Find("GUI/Text Shader");
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().material.color = ModeColor(vrrig);
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().SetPosition(0, vrrig.mainSkin.bones[bones[i]].position);
                                vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>().SetPosition(1, vrrig.mainSkin.bones[bones[i + 1]].position);
                            }
                        }
                    }
                    for (int i = 0; i < bones.Count(); i += 2)
                    {
                        GameObject.Destroy(vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>());
                        GameObject.Destroy(vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>());
                    }
                }
                else
                {
                    DisableLineRenderers();
                }
            }

            public static void DisableLineRenderers()
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (vrrig != GorillaTagger.Instance.offlineVRRig)
                    {
                        if (vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>())
                        {
                            GameObject.Destroy(vrrig.head.rigTarget.gameObject.GetComponent<LineRenderer>());
                        }
                        for (int i = 0; i < bones.Length; i += 2)
                        {
                            if (vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>())
                            {
                                GameObject.Destroy(vrrig.mainSkin.bones[bones[i]].gameObject.GetComponent<LineRenderer>());
                            }
                        }
                    }
                }
            }

            private static Dictionary<VRRig, LineRenderer> tracers = new Dictionary<VRRig, LineRenderer>();
            public static void InfectionTracers(bool isActive)
            {
                if (isActive)
                {
                    foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                    {
                        if (vrrig != taggerInstance.offlineVRRig)
                        {
                            if (!tracers.ContainsKey(vrrig))
                            {
                                CreateTracer(vrrig);
                            }
                            tracers[vrrig].material.color = ModeColor(vrrig);
                            tracers[vrrig].SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                            tracers[vrrig].SetPosition(1, vrrig.transform.position);
                        }
                    }
                }
                else
                {
                    DisableAllTracers();
                }
            }

            private static void CreateTracer(VRRig vrrig)
            {
                GameObject lineObj = new GameObject("InfectionTracer");
                lineObj.transform.SetParent(vrrig.headMesh.transform, false);
                LineRenderer liner = lineObj.AddComponent<LineRenderer>();
                tracers[vrrig] = liner;
                liner.material = new Material(guiShader);
                liner.startWidth = 0.0075f;
                liner.useWorldSpace = true;
            }

            public static void DisableAllTracers()
            {
                foreach (var kvp in tracers)
                {
                    UnityEngine.Object.Destroy(kvp.Value.gameObject);
                }
                tracers.Clear();
            }
        }




            public static void InfectionChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    if (vrrig.mainSkin.material.name.Contains("fected") || vrrig.mainSkin.material.name.Contains("It"))
                    {
                        vrrig.mainSkin.material.color = new Color32(byte.MaxValue, 0, 0, 40);
                    }
                    if (!vrrig.mainSkin.material.name.Contains("fected"))
                    {
                        vrrig.mainSkin.material.color = new Color32(0, byte.MaxValue, 0, 40);
                    }
                }
            }
        }

        public static void RemoveChams()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig)
                {
                    vrrig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                }
            }
        }

        public static void BugAndBatESP()
        {
            GameObject batHold = GameObject.Find("Cave Bat Holdable");
            LineRenderer lineRenderer = batHold.AddComponent<LineRenderer>();
            Color color1 = Color.blue;
            Color color2 = Color.cyan;
            color1.a = 0.4f;
            color2.a = 0.4f;
            lineRenderer.startColor = color1;
            lineRenderer.endColor = color2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
            lineRenderer.SetPosition(1, batHold.transform.position);
            lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
            UnityEngine.Object.Destroy(batHold, Time.deltaTime);

            GameObject bugHold = GameObject.Find("Floating Bug Holdable");
            LineRenderer lineRenderer2 = bugHold.AddComponent<LineRenderer>();
            lineRenderer.startColor = color1;
            lineRenderer.endColor = color2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
            lineRenderer.SetPosition(1, bugHold.transform.position);
            lineRenderer.material.shader = Shader.Find("GUI/Text Shader");
            UnityEngine.Object.Destroy(bugHold, Time.deltaTime);
        }

      

        public static void DrawWireframeBox(Vector3 center, Quaternion rotation, Vector3 size, UnityEngine.Color color)  //Nugget gave me this its not skidded
            
        {
            Vector3 halfSize = size * 0.5f;
            Vector3[] corners = new Vector3[8]
            {
        center + rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z),
        center + rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
        center + rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z),
        center + rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z)
            };

            Vector3[][] edges = new Vector3[12][]
            {
        new Vector3[] { corners[0], corners[1] },
        new Vector3[] { corners[1], corners[2] },
        new Vector3[] { corners[2], corners[3] },
        new Vector3[] { corners[3], corners[0] },
        new Vector3[] { corners[4], corners[5] },
        new Vector3[] { corners[5], corners[6] },
        new Vector3[] { corners[6], corners[7] },
        new Vector3[] { corners[7], corners[4] },
        new Vector3[] { corners[0], corners[4] },
        new Vector3[] { corners[1], corners[5] },
        new Vector3[] { corners[2], corners[6] },
        new Vector3[] { corners[3], corners[7] }
            };

            foreach (var edge in edges)
            {
                LineRenderer lineUser = new GameObject("Line").AddComponent<LineRenderer>();
                float h = (Time.frameCount / 180f) % 1f;
                float h3 = (Time.frameCount / 300f) % 1f;
                Color color2 = Color.HSVToRGB(h, 1f, 1f);
                Color color3 = Color.HSVToRGB(h3, 1f, 1f);
                color2.a = 0.4f;
                color3.a = 0.4f;
                lineUser.startColor = color2;
                lineUser.endColor = color3;
                lineUser.startWidth = 0.0225f;
                lineUser.endWidth = 0.0225f;
                lineUser.useWorldSpace = true;
                lineUser.positionCount = 2;
                lineUser.SetPositions(edge);
                lineUser.material = new Material(Shader.Find("GUI/Text Shader"));
                lineUser.material.color = color;
                GameObject.Destroy(lineUser.gameObject, Time.deltaTime);
            }
        }



    }
}
