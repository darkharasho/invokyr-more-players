using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace InvokyrMorePlayers
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "com.darkharasho.invokyr.moreplayers";
        public const string PluginName = "InvokyrMorePlayers";
        public const string PluginVersion = "0.1.2";

        internal static new ManualLogSource Log;
        internal static ConfigEntry<int> MaxPlayers;

        public override void Load()
        {
            Log = base.Log;
            MaxPlayers = Config.Bind("General", "MaxPlayers", 12, new ConfigDescription(
                "Most players in a game you host (vanilla 6). Takes effect for the next game you create.",
                new AcceptableValueRange<int>(6, 16), new ConfigurationManagerAttributes { DispName = "Max players", Order = 0 }));

            MorePlayers.Apply(new Harmony(PluginGuid));
            MorePlayers.RaiseNetworkLimit();
            AddComponent<LimitKeeper>();
            Log.LogInfo($"{PluginName} {PluginVersion} loaded; up to {MaxPlayers.Value} players");
        }
    }
}
