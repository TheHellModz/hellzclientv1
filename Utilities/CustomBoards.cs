using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Mods.Categories.Settings;

namespace HellzClient.Utilities
{
    public class CustomBoards : MonoBehaviour
    {
        public static CustomBoards Instance { get; private set; }

        private TextMeshPro cocText;
        private TextMeshPro codeOfConductText;
        private GameObject treeRoom;
        private MeshRenderer BoardColor;
        private readonly Color titleTextColor = Color.white;
        private readonly Color descriptionTextColor = Color.white;
        private readonly Color boardColors = Color.black;
        private const string GitHubMessageUrl = "https://api.github.com/repos/mtAnimus/CxH-Client-Board-Text/contents/README.md";
        private readonly StringBuilder dynamicMessage = new StringBuilder("Loading message...");
        private Coroutine fetchMessageCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            cocText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/")?.GetComponent<TextMeshPro>();
            if (cocText == null) Debug.LogWarning("[CustomBoards] 'COC Text' not found.");

            codeOfConductText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/")?.GetComponent<TextMeshPro>();
            if (codeOfConductText == null) Debug.LogWarning("[CustomBoards] 'CodeOfConduct' not found.");



            Debug.Log("[CustomBoards] Initialized");

            if (fetchMessageCoroutine == null)
            {
                fetchMessageCoroutine = StartCoroutine(FetchMessageFromGitHub());
            }
            InitializeCustomBoards();
        }

        private void InitializeCustomBoards()
        {
            if (codeOfConductText != null)
            {
                codeOfConductText.text = $"HCO.ORG <color=green>({Initialization.PluginInfo.menuVersion})</color>";
                codeOfConductText.color = titleTextColor;
                codeOfConductText.richText = true;
            }

            if (cocText != null)
            {
                cocText.color = descriptionTextColor;
                cocText.richText = true;
            }
        }

        private IEnumerator FetchMessageFromGitHub()
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(GitHubMessageUrl);
            webRequest.SetRequestHeader("Accept", "application/vnd.github.v3.raw");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                dynamicMessage.Clear();
                dynamicMessage.Append(webRequest.downloadHandler.text.Trim());
                Debug.Log("[CustomBoards] Message fetched successfully.");
            }
            else
            {
                dynamicMessage.Clear();
                dynamicMessage.Append("Default message: Failed to fetch online content.");
                Debug.LogError($"[CustomBoards] Failed to fetch message from GitHub: {webRequest.error}");
            }

            if (cocText != null)
            {
                cocText.text = $"THANK YOU FOR CHOOSING <color=white>HCO!</color>\n\n<color=red>IM NOT RESPONSIBLE FOR ANY ACTIONS TAKEN AGAINST YOUR ACCOUNT.</color>\n\n{dynamicMessage}\n\n<color=#00FFFF>FOUNDED AND DEVELOPED BY: CASH & Hellz</color>\n<color=yellow>BIG THANKS LUCAS AND TIMEZONE FOR HELPING</color>\n<color=purple>MENU DISCORD: DISCORD.GG/CCONTOP</color>";
                cocText.richText = true;
            }
        }
    }
}