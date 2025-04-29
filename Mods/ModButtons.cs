using static HellzClient.Utilities.GunLib;
using static HellzClient.Utilities.Variables;
using static HellzClient.Utilities.ColorLib;
using static HellzClient.Mods.Categories.Movement;
using static HellzClient.Mods.Categories.Player;
using static HellzClient.Mods.Categories.Room;
using static HellzClient.Mods.Categories.Fun;
using static HellzClient.Mods.Categories.Settings;
using static HellzClient.Mods.Categories.Overpowered;
using static HellzClient.Mods.Categories.Safety;
using static HellzClient.Mods.Categories.Tagging;
using static HellzClient.Mods.Categories.Visual;
using static HellzClient.Menu.ButtonHandler;
using static HellzClient.Menu.Main;
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
        Safety,
        Overpowered,
        Visual,
        Admin,
        Fun,
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
            new Button("Safety", Category.Home, false, false, ()=>ChangePage(Category.Safety)),
            new Button("Visual", Category.Home, false, false, ()=>ChangePage(Category.Visual)),
            #endregion

            #region Settings
            new Button("Switch Hands", Category.Settings, true, false, ()=>SwitchHands(true), ()=>SwitchHands(false)),
            new Button("Toggle Fun Mods Button", Category.Settings, true, true, ()=>ToggleFunMods(true), ()=>ToggleFunMods(false)),
            new Button("Toggle Side Buttons", Category.Settings, true, true, ()=>ToggleSideButtons(true), ()=>ToggleSideButtons(false)),
            new Button("Disconnect Button", Category.Settings, true, true, ()=>ToggleDisconnectButton(true), ()=>ToggleDisconnectButton(false)),
           // themeChangerButton = new Button($"Theme : {ThemeDescription}", Category.Settings, false, false, ()=>CycleTheme()),
            new Button("Toggle Notifications", Category.Settings, true, false, ()=>ToggleNotifications(true), ()=>ToggleNotifications(false)),
            new Button("Toggle Outline", Category.Settings, true, true, ()=>ToggleOutline(true), ()=>ToggleOutline(false)),
            new Button("Clear Notifications", Category.Settings, true, false, ()=>ClearNotifications()),
            new Button("Disable All Mods", Category.Settings, false, false, ()=>DisableAllMods()),
            #endregion

            #region Room
            new Button("Quit Game", Category.Room, false, false, ()=>QuitGTAG()),
            new Button("Disconnect", Category.Room, false, false, ()=>Disconnect()),
            new Button("Join Random", Category.Room, false, false, ()=>JoinRandomPublic()),
            new Button("Get All Ids [RT]", Category.Room, false, false, ()=>GetIds()),
            new Button("Report All [RT]", Category.Room, false, false, ()=>ReportAll()),
            new Button("Mute All [RT]", Category.Room, false, false, ()=>MuteAll()),
            #endregion

            #region Player
            new Button("Ghost Monke (A)", Category.Player, true, false, ()=>GhostMonke()),
            new Button("Invisible Monke (A)", Category.Player, true, false, ()=>InvisMonkey()),
            new Button("Long Arms", Category.Player, true, false, ()=>LongArms(true, new Vector3(1.3f, 1.3f, 1.3f)), ()=>LongArms(false, Vector3.zero)),
            new Button("TP --> Stump", Category.Player, true, false, ()=>tptostump()),
            new Button("Tag all", Category.Player, true, false, ()=>TagAll()),
            new Button("Tag Self", Category.Player, true, false, ()=>TagSelf()),
            new Button("Freeze Rig [RT]", Category.Player, true, false, ()=>FreezeRig()),
            new Button("Grab Rig [LG+RG]", Category.Player, true, false, ()=>HoldRig()),
            new Button("Spaz arms [RT]", Category.Player, true, false, ()=>SpazArms()),
            new Button("Tiny Rig [RT]", Category.Player, true, false, ()=>TinyRig()),
            new Button("Large Rig [RT]", Category.Player, true, false, ()=>LargeRig()),
            new Button("Change Name [HCOONTOP]", Category.Player, true, false, ()=>NameChange("HCOONTOP")),
            new Button("Helecopter Monkey [RT]", Category.Player, true, false, ()=>HelicopterMonkey()),
            new Button("BayBlade Monkey [RT]", Category.Player, true, false, ()=>StillHeliMonkey()),
            new Button("Fake Lag [RT]", Category.Player, true, false, ()=>FakeLag()),
            #endregion

            #region Movement
            new Button("Flight (A)", Category.Movement, true, false, ()=>Fly(rightPrimary)),
            new Button("Speed Boost", Category.Movement, true, false, ()=>SpeedBoost(10, 2)),
            new Button("Noclip (RT)", Category.Movement, true, false, ()=>Noclip()),
            new Button("Plats", Category.Movement, true, false, ()=>NormalPlats()),
            new Button("Invis Plats", Category.Movement, true, false, ()=>InvisPlats()),
            new Button("Wall Walk [RG+LG]", Category.Movement, true, false, ()=>WallWalk(ControllerInputPoller.instance.rightGrab, ControllerInputPoller.instance.leftGrab)),
            new Button("Fast Swim", Category.Movement, true, false, ()=>FastSwim(1.05f), ()=>FastSwim(1f)),
            new Button("Gravity Pull Up [RT]", Category.Movement, true, false, ()=>GravityPull(Vector3.up)),
            new Button("Gravity Pull Down [RT]", Category.Movement, true, false, ()=>GravityPull(Vector3.down)),
            new Button("Low Gravity", Category.Movement, true, false, ()=>LowGravity()),
            new Button("No Gravity", Category.Movement, true, false, ()=>ZeroGravity()),
            #endregion

            #region Safety
            new Button("Anti-Report", Category.Safety, true, false, ()=>Antireportdisconnect()),
            new Button("Panic [QUIT]", Category.Safety, true, false, ()=>Application.Quit()),
            new Button("No Fingers", Category.Safety, true, false, ()=>DisableFingers()),
            new Button("Disable all mods", Category.Safety, true, false, ()=>DisableAllMods()),
            #endregion
            
            #region OP  
            new Button("Break Audio", Category.Overpowered, true, false, ()=> BreakAudio()),
            #endregion

            #region Visual
            new Button("Line ESP", Category.Visual, true, false, ()=> LineESP()),
            new Button("Wireframe ESP", Category.Visual, true, false, ()=> WireFrameESP()),
            new Button("Infection Chams", Category.Visual, true, false, ()=> InfectionChams(), ()=> RemoveChams()),
            new Button("Box ESP", Category.Visual, true, false, ()=> CashVisuals.BoxESP(true), ()=> CashVisuals.BoxESP(false)),
            new Button("Skeleton ESP", Category.Visual, true, false, ()=> CashVisuals.SkeletonESP(true), ()=> CashVisuals.SkeletonESP(false)),
            #endregion


            #region Fun
            new Button("Spawn Obby", Category.Fun, true, false, ()=>startObby(), ()=>destroyObby()),

            #endregion 
        };
    }
}