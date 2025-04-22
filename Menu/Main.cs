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
using System.IO;
using g3;
using Valve.VR;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using UnityEngine.ProBuilder.MeshOperations;
using GorillaNetworking;
using System.Net;
using System.Threading;
using HellzClient.Mods.Categories;
using GorillaExtensions;
using TMPro;
using System.Reflection;
using static Fusion.Sockets.NetBitBuffer;

namespace HellzClient.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            try
            {
                if (playerInstance == null || taggerInstance == null)
                {
                    UnityEngine.Debug.LogError("Player instance or GorillaTagger is null. Skipping menu updates.");
                    return;
                }

                foreach (ButtonHandler.Button bt in ModButtons.buttons)
                {
                    try
                    {
                        if (bt.Enabled && bt.onEnable != null)
                        {
                            bt.onEnable.Invoke();
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error invoking button action: {bt.buttonText}. Exception: {ex}");
                    }
                }

                if (NotificationLib.Instance != null)
                {
                    try
                    {
                        NotificationLib.Instance.UpdateNotifications();
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error updating notifications. Exception: {ex}");
                    }
                }

                if (UnityInput.Current.GetKeyDown(PCMenuKey))
                {
                    PCMenuOpen = !PCMenuOpen;
                }

                HandleMenuInteraction();
            }
            catch (NullReferenceException ex)
            {
                UnityEngine.Debug.LogError($"NullReferenceException: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Unexpected error: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        public void Awake()
        {
            ResourceLoader.LoadResources();
            taggerInstance = GorillaTagger.Instance;
            playerInstance = GorillaLocomotion.GTPlayer.Instance;
            pollerInstance = ControllerInputPoller.instance;
            thirdPersonCamera = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            cm = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1");
        }

        public static void HandleMenuInteraction()
        {
            try
            {
                if (PCMenuOpen && !InMenuCondition && !pollerInstance.leftControllerPrimaryButton && !pollerInstance.rightControllerPrimaryButton && !menuOpen)
                {
                    InPcCondition = true;
                    cm?.SetActive(false);

                    if (menuObj == null)
                    {
                        Draw();
                        AddButtonClicker(thirdPersonCamera?.transform);
                    }
                    else
                    {
                        AddButtonClicker(thirdPersonCamera?.transform);

                        if (thirdPersonCamera != null)
                        {
                            PositionMenuForKeyboard();

                            try
                            {
                                if (Mouse.current.leftButton.isPressed)
                                {
                                    Ray ray = thirdPersonCamera.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
                                    if (Physics.Raycast(ray, out RaycastHit hit))
                                    {
                                        BtnCollider btnCollider = hit.collider?.GetComponent<BtnCollider>();
                                        if (btnCollider != null && clickerObj != null)
                                        {
                                            btnCollider.OnTriggerEnter(clickerObj.GetComponent<Collider>());
                                        }
                                    }
                                }
                                else if (clickerObj != null)
                                {
                                    Optimizations.DestroyObject(ref clickerObj);
                                }
                            }
                            catch (Exception ex)
                            {
                                UnityEngine.Debug.LogError($"Error handling mouse click. Exception: {ex}");
                            }
                        }
                    }
                }
                else if (menuObj != null && InPcCondition)
                {
                    InPcCondition = false;
                    CleanupMenu(0);
                    cm?.SetActive(true);
                }

                openMenu = rightHandedMenu ? pollerInstance.rightControllerSecondaryButton : pollerInstance.leftControllerSecondaryButton;

                if (openMenu && !InPcCondition)
                {
                    InMenuCondition = true;
                    if (menuObj == null)
                    {
                        Draw();
                        AddRigidbodyToMenu();
                        AddButtonClicker(rightHandedMenu ? playerInstance.leftControllerTransform : playerInstance.rightControllerTransform);
                    }
                    else
                    {
                        PositionMenuForHand();
                    }
                }
                else if (menuObj != null && InMenuCondition)
                {
                    InMenuCondition = false;
                    AddRigidbodyToMenu();

                    Vector3 currentVelocity = rightHandedMenu ? playerInstance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0f, false) : playerInstance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0f, false);
                    if (Vector3.Distance(currentVelocity, previousVelocity) > velocityThreshold)
                    {
                        currentMenuRigidbody.velocity = currentVelocity;
                        previousVelocity = currentVelocity;
                    }

                    CleanupMenu(1);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error handling menu interaction. Exception: {ex}");
            }
        }

        public static void Draw()
        {
            if (menuObj != null)
            {
                ClearMenuObjects();
                return;
            }

            CreateMenuObject();
            CreateBackground();
            CreateMenuCanvasAndTitle();
            AddDisconnectButton();
            AddReturnButton();
            AddPageButton(">");
            AddPageButton("<");


            ButtonPool.ResetPool();
            var PageToDraw = GetButtonInfoByPage(currentPage).Skip(currentCategoryPage * ButtonsPerPage).Take(ButtonsPerPage).ToArray();
            for (int i = 0; i < PageToDraw.Length; i++)
            {
                AddModButtons(i * 0.1f, PageToDraw[i]);
            }
        }

      

        public static void CreateMenuObject()
        {
            // Menu Object
            menuObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menuObj.GetComponent<Rigidbody>());
            Destroy(menuObj.GetComponent<BoxCollider>());
            Destroy(menuObj.GetComponent<Renderer>());
            menuObj.name = "menu";
            menuObj.transform.localScale = new Vector3(0.1f, 0.2f, 0.3f);
        }

        public static void CreateBackground()
        {
            // Background
            background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(background.GetComponent<Rigidbody>());
            Destroy(background.GetComponent<BoxCollider>());
            background.GetComponent<MeshRenderer>().material.color = BlackTransparent;
            background.transform.parent = menuObj.transform;
            background.transform.rotation = Quaternion.identity;
            background.transform.localScale = new Vector3(0.01f, 4f, 1f);
            background.name = "menucolor";
            background.transform.position = new Vector3(0.05f, 0, 0.025f);
            CreateBorder(background, RedTransparent);
        }

   
        public static void AddDisconnectButton()
        {
            if (toggledisconnectButton)
            {
                // Disconnect Button
                disconnectButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(disconnectButton.GetComponent<Rigidbody>());
                disconnectButton.GetComponent<BoxCollider>().isTrigger = true;
                disconnectButton.transform.parent = menuObj.transform;
                disconnectButton.transform.rotation = Quaternion.identity;
                disconnectButton.transform.localScale = new Vector3(0.005f, 0.8975f, 0.0575f);
                disconnectButton.transform.localPosition = new Vector3(0.5f, 0f, 0.6f);
                disconnectButton.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("DisconnectButton", Category.Home, false, false, null, null);
                disconnectButton.GetComponent<Renderer>().material.color = ThemeArraya[CurrentTheme];
                disconnectButton.GetComponent<MeshRenderer>().material = new Material(guiShader);

                // Disconnect Button Text
                Text discontext = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
                discontext.text = "Disconnect";
                discontext.font = ResourceLoader.ArialFont;
                discontext.fontStyle = FontStyle.Normal;
                discontext.fontSize = 3;
                discontext.color = White;
                discontext.alignment = TextAnchor.MiddleCenter;
                discontext.resizeTextForBestFit = true;
                discontext.resizeTextMinSize = 0;
                RectTransform rectt = discontext.GetComponent<RectTransform>();
                rectt.sizeDelta = new Vector2(0.2f, 0.02f);
                rectt.localPosition = new Vector3(0.051f, 0f, 0.18f);
                rectt.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                CreateBorder(disconnectButton, RedTransparent);
            }
        }

        public static void CreateMenuCanvasAndTitle()
        {
            // Menu Canvas
            canvasObj = new GameObject();
            canvasObj.transform.parent = menuObj.transform;
            canvasObj.name = "canvas";
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScale = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScale.dynamicPixelsPerUnit = 1000;

            // Menu Title
            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            titleObj.transform.localScale = new Vector3(0.875f, 0.875f, 1f);
            title = titleObj.AddComponent<Text>();
            title.font = ResourceLoader.ArialFont;
            title.fontStyle = FontStyle.Normal;
            title.text = $"{menuName} v{menuVersion}";
            title.color = White;
            title.fontSize = 5;
            title.alignment = TextAnchor.UpperCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.up;
            titleTransform.position = new Vector3(0.051f, 0f, 0.135f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            titleTransform.sizeDelta = new Vector2(0.19f, 0.04f);
        }

        public static void CreateBorder(GameObject obj, Color color)
        {
                float outlineWidth = obj.transform.localScale.x - 0.0025f;
                float outlineHeight = obj.transform.localScale.y + 0.0275f;
                float outlineDepth = obj.transform.localScale.z + 0.0275f;
                GameObject outlineobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(outlineobj.GetComponent<Rigidbody>());
                Destroy(outlineobj.GetComponent<BoxCollider>());
                outlineobj.transform.parent = menuObj.transform;
                outlineobj.transform.rotation = obj.transform.rotation;
                outlineobj.transform.localScale = new Vector3(outlineWidth, outlineHeight, outlineDepth);
                outlineobj.transform.position = obj.transform.position;
                outlineobj.GetComponent<Renderer>().material.color = color;  
        }


        public static void AddModButtons(float offset, ButtonHandler.Button button)
        {
            // Mod Buttons
            GameObject ModButton = ButtonPool.GetButton();
            Rigidbody btnRigidbody = ModButton.GetComponent<Rigidbody>();
            if (btnRigidbody != null)
            {
                Destroy(btnRigidbody);
            }
            BoxCollider btnCollider = ModButton.GetComponent<BoxCollider>();
            if (btnCollider != null)
            {
                btnCollider.isTrigger = true;
            }

            ModButton.transform.SetParent(menuObj.transform, false);
            ModButton.transform.rotation = Quaternion.identity;
            ModButton.transform.localScale = new Vector3(0.005f, 0.5f, 0.08f);
            ModButton.transform.localPosition = new Vector3(0.505f, 1.7f, 0.45f - offset);
            BtnCollider btnColScript = ModButton.GetComponent<BtnCollider>() ?? ModButton.AddComponent<BtnCollider>();
            btnColScript.clickedButton = button;
            ModButton.GetComponent<MeshRenderer>().material.color = Black;
            CreateBorder(ModButton, GreyTransparent);

            // Mod Buttons Text
            GameObject titleObj = TextPool.GetTextObject();
            titleObj.transform.SetParent(canvasObj.transform, false);
            titleObj.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
            Text title = titleObj.GetComponent<Text>();
            title.text = button.buttonText;
            title.font = ResourceLoader.ArialFont;
            title.fontStyle = FontStyle.BoldAndItalic;
            title.color = White;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = new Vector3(0.051f, 0.34f, 0.135f - offset / 3.35f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            titleTransform.sizeDelta = new Vector2(0.16f, 0.01725f);

            Renderer btnRenderer = ModButton.GetComponent<Renderer>();
            if (btnRenderer != null)
            {
                if (button.Enabled)
                {
                    btnRenderer.material.color = Black;
                    title.color = Color.green;
                }
                else
                {
                    btnRenderer.material.color = Black;
                }
            }
        }

        public static void AddPageButton(string button)
        {
            // Page Buttons
            GameObject PageButtons = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(PageButtons.GetComponent<Rigidbody>());
            PageButtons.GetComponent<BoxCollider>().isTrigger = true;
            PageButtons.transform.parent = menuObj.transform;
            PageButtons.transform.rotation = Quaternion.identity;
            PageButtons.transform.localScale = new Vector3(0.005f, 0.25f, 0.08f);
            PageButtons.transform.localPosition = new Vector3(0.505f, button.Contains("<") ? 0.285f : -0.285f, -0.31f);
            PageButtons.GetComponent<Renderer>().material.color = Black;
            PageButtons.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button(button, Category.Home, false, false, null, null);

            // Page Buttons Text
            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            titleObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            Text title = titleObj.AddComponent<Text>();
            title.font = ResourceLoader.ArialFont;
            title.color = White;
            title.fontSize = 3;
            title.fontStyle = FontStyle.Normal;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.2f, 0.03f);
            title.text = button.Contains("<") ? "<" : ">";
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            titleTransform.position = new Vector3(0.051f, button.Contains("<") ? 0.059f : -0.059f, -0.0935f);

            CreateBorder(PageButtons, GreyTransparent);
        }

        public static void AddReturnButton()
        {
            // Return Button
            GameObject BackToStartButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(BackToStartButton.GetComponent<Rigidbody>());
            BackToStartButton.GetComponent<BoxCollider>().isTrigger = true;
            BackToStartButton.transform.parent = menuObj.transform;
            BackToStartButton.transform.rotation = Quaternion.identity;
            BackToStartButton.transform.localScale = new Vector3(0.005f, 0.30625f, 0.08f);
            BackToStartButton.transform.localPosition = new Vector3(0.505f, 0f, -0.31f);
            BackToStartButton.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("ReturnButton", Category.Home, false, false, null, null);
            BackToStartButton.GetComponent<Renderer>().material.color = Black;
            CreateBorder(BackToStartButton, GreyTransparent);

            // Return Button Text
            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            titleObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            titleObj.transform.localPosition = new Vector3(0.85f, 0.85f, 0.85f);
            Text title = titleObj.AddComponent<Text>();
            title.font = ResourceLoader.ArialFont;
            title.fontStyle = FontStyle.Normal;
            title.text = "Return";
            title.color = White;
            title.fontSize = 3;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.2f, 0.02f);
            titleTransform.localPosition = new Vector3(0.051f, 0f, -0.0935f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void AddButtonClicker(Transform parentTransform)
        {
            // Button Clicker
            if (clickerObj == null)
            {
                clickerObj = new GameObject("buttonclicker");
                BoxCollider clickerCollider = clickerObj.AddComponent<BoxCollider>();
                if (clickerCollider != null)
                {
                    clickerCollider.isTrigger = true;
                }
                MeshFilter meshFilter = clickerObj.AddComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                }
                Renderer clickerRenderer = clickerObj.AddComponent<MeshRenderer>();
                if (clickerRenderer != null)
                {
                    clickerRenderer.material.color = White;
                    clickerRenderer.material.shader = Shader.Find("GUI/Text Shader");
                }
                if (parentTransform != null)
                {
                    clickerObj.transform.parent = parentTransform;
                    clickerObj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    clickerObj.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                }
            }
        }

        private static void PositionMenuForHand()
        {
            if (rightHandedMenu)
            {
                menuObj.transform.position = playerInstance.rightControllerTransform.position;
                Vector3 rotation = playerInstance.rightControllerTransform.rotation.eulerAngles;
                rotation += new Vector3(0f, 0f, 180f);
                menuObj.transform.rotation = Quaternion.Euler(rotation);
            }
            else
            {
                menuObj.transform.position = playerInstance.leftControllerTransform.position;
                menuObj.transform.rotation = playerInstance.leftControllerTransform.rotation;
            }
        }

        public static void PosForMenuInfront()
        {
            if (V2Menu)
            {
                menuObj.transform.position = playerInstance.headCollider.transform.position + new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                menuObj.transform.position = playerInstance.rightControllerTransform.position;
                Vector3 rotation = playerInstance.rightControllerTransform.rotation.eulerAngles;
                rotation += new Vector3(0f, 0f, 180f);
                menuObj.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        private static void PositionMenuForKeyboard()
        {
            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.transform.position = new Vector3(-65.8373f, 21.6568f, -80.9763f);
                thirdPersonCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                menuObj.transform.SetParent(thirdPersonCamera.transform, true);

                Vector3 headPosition = thirdPersonCamera.transform.position;
                Quaternion headRotation = thirdPersonCamera.transform.rotation;
                float offsetDistance = 0.65f;
                Vector3 offsetPosition = headPosition + headRotation * Vector3.forward * offsetDistance;
                menuObj.transform.position = offsetPosition;

                Vector3 directionToHead = headPosition - menuObj.transform.position;
                menuObj.transform.rotation = Quaternion.LookRotation(directionToHead, Vector3.up);
                menuObj.transform.Rotate(Vector3.up, -90.0f);
                menuObj.transform.Rotate(Vector3.right, -90.0f);
            }
        }

        public static void AddRigidbodyToMenu()
        {
            if (currentMenuRigidbody == null && menuObj != null)
            {
                currentMenuRigidbody = menuObj.GetComponent<Rigidbody>();
                if (currentMenuRigidbody == null)
                {
                    currentMenuRigidbody = menuObj.AddComponent<Rigidbody>();
                }
                currentMenuRigidbody.useGravity = false;
            }
        }
    }
}