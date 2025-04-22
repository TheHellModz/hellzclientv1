using static HellzClient.Utilities.GunLib;
using static HellzClient.Utilities.Variables;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Mods.Categories.Movement;
using static HellzClient.Mods.Categories.Player;
using static HellzClient.Mods.Categories.Room;
using static HellzClient.Mods.Categories.Settings;
using static HellzClient.Menu.ButtonHandler;
using UnityEngine;
using PlayFab.MultiplayerModels;
using Hellz_Client.Mods;
using PlayFab.SharedModels;
using Photon.Realtime;
using Fusion;
using System.Net.Sockets;
using Steamworks;
using PlayFab.ClientModels;
using KID.Model;
using GorillaNetworking;
using PlayFab.AuthenticationModels;
using Photon.Pun;
using HellzClient.Mods.Categories;

namespace HellzClient.Mods
{
    public enum Category
    {
        // Starting Page
        Home,

        // Mod Categories
        Settings,
        Room,
        Player,
        Movement,
        Overpowered,
        AntiBan,
        Test1,
        Test2,
        Test3,
        Test4,
    }
    public class ModButtons
    {
        public static Button[] buttons =
        {
            #region Starting Page
            new Button("Settings", Category.Home, false, false, ()=>ChangePage(Category.Settings)),
            new Button("Room", Category.Home, false, false, ()=>ChangePage(Category.Room)),
            new Button("Player", Category.Home, false, false, ()=>ChangePage(Category.Player)),
            new Button("Movement", Category.Home, false, false, ()=>ChangePage(Category.Movement)),
            new Button("OP", Category.Home, false, false, ()=>ChangePage(Category.AntiBan)),
            new Button("Test1", Category.Home, false, false, ()=>ChangePage(Category.Test1)),
            new Button("Test2", Category.Home, false, false, ()=>ChangePage(Category.Test2)),
            new Button("Test3", Category.Home, false, false, ()=>ChangePage(Category.Test3)),
            new Button("Test4", Category.Home, false, false, ()=>ChangePage(Category.Test4)),
            #endregion

            

            #region Settings
            new Button("Switch Hands", Category.Settings, true, false, ()=>SwitchHands(true), ()=>SwitchHands(false)),
            new Button("Disconnect Button", Category.Settings, true, true, ()=>ToggleDisconnectButton(false), ()=>ToggleDisconnectButton(true)),
           // themeChangerButton = new Button($"Theme : {ThemeDescription}", Category.Settings, false, false, ()=>CycleTheme()),
            new Button("Toggle Notifications", Category.Settings, true, true, ()=>ToggleNotifications(true), ()=>ToggleNotifications(false)),
           // new Button("Toggle Outline", Category.Settings, true, true, ()=>ToggleMenuOutline(true), ()=>ToggleMenuOutline(false)),
            new Button("Clear Notifications", Category.Settings, false, false, ()=>ClearNotifications()),
            new Button("Disable All Mods", Category.Settings, false, false, ()=>DisableAllMods()),

            #endregion

            #region Room
            new Button("Quit Game", Category.Room, false, false, ()=>QuitGTAG()),
            new Button("Disconnect", Category.Room, false, false, ()=>Disconnect()),
            new Button("Join Random", Category.Room, false, false, ()=>JoinRandomPublic()),
            #endregion

            #region Player
            new Button("Ghost Monke (A)", Category.Player, true, false, ()=>GhostMonke()),
            new Button("Invisible Monke (A)", Category.Player, true, false, ()=>InvisMonkey()),
            new Button("Long Arms", Category.Player, true, false, ()=>LongArms(true, new Vector3(1.3f, 1.3f, 1.3f)), ()=>LongArms(false, Vector3.zero)),
            //new Button("TP --> Stump", Category.Player, true, false, ()=>InvisibleMonke()),
            #endregion

            #region Movement
            new Button("Flight (A)", Category.Movement, true, false, ()=>Fly(rightPrimary)),
            new Button("Speed Boost", Category.Movement, true, false, ()=>SpeedBoost(10, 2)),
            new Button("Noclip (RT)", Category.Movement, true, false, ()=>Noclip()),
            new Button("Plats", Category.Movement, true, false, ()=>NormalPlats()),
            new Button("Invis Plats", Category.Movement, true, false, ()=>InvisPlats()),
            #endregion
        };
    }
}