using BepInEx;
using HarmonyLib;
using static HellzClient.Initialization.PluginInfo;

namespace HellzClient.Initialization
{
    [BepInPlugin(menuGUID, menuName, menuVersion)]
    public class BepInExInitializer : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource LoggerInstance;

        void Awake()
        {
            LoggerInstance = Logger;
            new Harmony(menuGUID).PatchAll();
        }
    }
}
