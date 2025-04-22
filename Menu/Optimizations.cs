using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HellzClient.Utilities.Variables;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Menu.Main;
using static HellzClient.Menu.ButtonHandler;

namespace HellzClient.Menu
{
    public class Optimizations
    {
        public static class ButtonPool
        {
            private static List<GameObject> buttonPool = new List<GameObject>();
            private static int currentIndex = 0;

            public static GameObject GetButton()
            {
                if (currentIndex < buttonPool.Count)
                {
                    GameObject button = buttonPool[currentIndex];
                    if (button == null)
                    {
                        button = CreateNewButton();
                        buttonPool[currentIndex] = button;
                    }
                    button.SetActive(true);
                    currentIndex++;
                    return button;
                }
                else
                {
                    GameObject newButton = CreateNewButton();
                    buttonPool.Add(newButton);
                    currentIndex++;
                    return newButton;
                }
            }

            public static void ResetPool()
            {
                currentIndex = 0;
                foreach (GameObject obj in buttonPool)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                        obj.transform.parent = null; 
                    }
                }
            }

            private static GameObject CreateNewButton()
            {
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                button.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                return button;
            }
        }

        public static class TextPool
        {
            private static List<GameObject> textPool = new List<GameObject>();
            private static int currentIndex = 0;

            public static GameObject GetTextObject()
            {
                if (currentIndex < textPool.Count)
                {
                    GameObject textObj = textPool[currentIndex];
                    if (textObj == null)
                    {
                        textObj = CreateNewTextObject();
                        textPool[currentIndex] = textObj;
                    }
                    textObj.SetActive(true);
                    currentIndex++;
                    return textObj;
                }
                else
                {
                    GameObject newTextObj = CreateNewTextObject();
                    textPool.Add(newTextObj);
                    currentIndex++;
                    return newTextObj;
                }
            }

            public static void ResetPool()
            {
                currentIndex = 0;
                foreach (GameObject textObj in textPool)
                {
                    if (textObj != null)
                    {
                        textObj.SetActive(false);
                    }
                }
            }

            private static GameObject CreateNewTextObject()
            {
                GameObject textObj = new GameObject();
                Text text = textObj.AddComponent<Text>();
                text.fontStyle = FontStyle.Normal;
                text.color = White;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;
                return textObj;
            }
        }

        public static class ResourceLoader
        {
            // Fonts
            public static Font ArialFont { get; private set; }

            public static void LoadResources()
            {
                ArialFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }

        public static void DestroyObject<T>(ref T obj, float delay = 0f) where T : UnityEngine.Object
        {
            if (obj != null)
            {
                if (obj is Component component)
                {
                    UnityEngine.Object.Destroy(component.gameObject, delay);
                }
                else
                {
                    UnityEngine.Object.Destroy(obj, delay);
                }

                obj = null; 
            }
        }

        public static void CleanupMenu(float delay = 0f)
        {
            DestroyObject(ref menuObj, delay);
            DestroyObject(ref clickerObj);
            currentMenuRigidbody = null;
        }

        public static void ClearMenuObjects()
        {
            DestroyObject(ref menuObj);
            DestroyObject(ref background);
            DestroyObject(ref canvasObj);
            currentMenuRigidbody = null;
        }


        public static void RefreshMenu()
        {
            ClearMenuObjects(); 
            Draw();           
        }
    }
}
