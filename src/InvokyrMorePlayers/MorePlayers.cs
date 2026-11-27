using System;
using System.Collections.Generic;
using _200_Dev.Entities;
using _200_Dev.Networking;
using _200_Dev.Utility.Runtime.NetworkManager;
using Entities.Player;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using User;
using EPlayerCharacter = User.UserInstancesManager.EPlayerCharacter;

namespace InvokyrMorePlayers
{
    /// <summary>
    /// Raises the player limit (vanilla 6). The limit lives in several places: Mirror's maxConnections, the game's
    /// <c>VhsNetworkManager.MaxPlayers</c>, the Steam lobby's member limit and the lobby browser's player-count filter.
    /// Players past the sixth then need a player index, a character and a spawn point; the game has 6 indices, 8
    /// characters and 6 spawn points, so the extra players reuse them.
    /// </summary>
    internal static class MorePlayers
    {
        private const int CharacterCount = 8;

        private static int Cap => Math.Max(Slots.VanillaMax, Plugin.MaxPlayers.Value);

        public static void Apply(Harmony harmony)
        {
            Prefix(harmony, typeof(SteamLobby), nameof(SteamLobby.Create), nameof(LobbyCreatePrefix));
            Prefix(harmony, typeof(SteamMatchmaking), nameof(SteamMatchmaking.CreateLobby), nameof(SteamCreateLobbyPrefix));
            Prefix(harmony, typeof(SteamMatchmaking), nameof(SteamMatchmaking.AddRequestLobbyListNumericalFilter), nameof(LobbyFilterPrefix));
            Postfix(harmony, typeof(UserInstancesManager), nameof(UserInstancesManager.SrvRefillAvailableIndices), nameof(RefillPostfix));
            Postfix(harmony, typeof(UserInstancesManager), nameof(UserInstancesManager.SrvGetFirstFreeCharacter), nameof(FirstFreeCharacterPostfix));
            Postfix(harmony, typeof(UserInstancesManager), nameof(UserInstancesManager.SrvGetPlayerCharacterByIndex), nameof(CharacterByIndexPostfix));
            Postfix(harmony, typeof(UserInstance), nameof(UserInstance.SrvChooseCharacter), nameof(ChooseCharacterPostfix));
            Postfix(harmony, typeof(UserInstance), nameof(UserInstance.SrvGetSpawnTransformForIndex), nameof(SpawnPostfix));
        }

        private static void Prefix(Harmony harmony, Type type, string method, string patch) => Patch(harmony, type, method, patch, true);

        private static void Postfix(Harmony harmony, Type type, string method, string patch) => Patch(harmony, type, method, patch, false);

        private static void Patch(Harmony harmony, Type type, string method, string patch, bool prefix)
        {
            try
            {
                var target = AccessTools.Method(type, method);
                if (target == null)
                {
                    Plugin.Log.LogWarning($"{type.Name}.{method} not found; skipped");
                    return;
                }
                var hm = new HarmonyMethod(typeof(MorePlayers), patch);
                harmony.Patch(target, prefix: prefix ? hm : null, postfix: prefix ? null : hm);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Patching {type.Name}.{method} failed: {e.Message}");
            }
        }

        /// <summary>Also called every half second by <see cref="LimitKeeper"/>, in case the game puts the limit back.</summary>
        internal static void RaiseNetworkLimit()
        {
            try
            {
                if (VhsNetworkManager.MaxPlayers < Cap)
                    VhsNetworkManager.MaxPlayers = Cap;
                var manager = Mirror.NetworkManager.singleton;
                if (manager != null && manager.maxConnections < Cap)
                    manager.maxConnections = Cap;
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning($"Raising the network limit failed: {e.Message}");
            }
        }

        private static void LobbyCreatePrefix(ref int __3)
        {
            if (__3 < Cap)
                __3 = Cap;
        }

        private static void SteamCreateLobbyPrefix(ref int __1)
        {
            if (__1 < Cap)
                __1 = Cap;
        }

        private static void LobbyFilterPrefix(string __0, ref int __1) => __1 = Slots.FilterValue(__0, __1, Cap);

        private static void RefillPostfix(UserInstancesManager __instance)
        {
            var list = __instance?._availablePlayerIndices;
            if (list == null)
                return;
            var present = new HashSet<int>();
            for (var i = 0; i < list.Count; i++)
                present.Add(list[i]);
            foreach (var index in Slots.MissingIndices(present, Cap))
                list.Add(index);
        }

        private static void FirstFreeCharacterPostfix(ref EPlayerCharacter __result)
        {
            if (__result == EPlayerCharacter.None)
                __result = LeastUsedCharacter();
        }

        private static void ChooseCharacterPostfix(ref EPlayerCharacter __result)
        {
            if (__result == EPlayerCharacter.None)
                __result = LeastUsedCharacter();
        }

        private static void CharacterByIndexPostfix(UserInstancesManager __instance, int __0, ref EPlayerCharacter __result)
        {
            if (__result == EPlayerCharacter.None && __0 >= CharacterCount)
                __result = __instance.SrvGetPlayerCharacterByIndex(Slots.Wrap(__0, CharacterCount));
        }

        private static void SpawnPostfix(int __0, ref Transform __result)
        {
            if (__result == null && __0 >= Slots.VanillaMax)
                __result = UserInstance.SrvGetSpawnTransformForIndex(Slots.Wrap(__0, Slots.VanillaMax));
        }

        private static EPlayerCharacter LeastUsedCharacter()
        {
            var used = new List<int>();
            var players = new Il2CppSystem.Collections.Generic.List<PlayerEntity>();
            EntityManager.GetAllPlayersNoAlloc(players);
            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                    used.Add((int)players[i].playerCharacter);
            }
            return (EPlayerCharacter)Slots.LeastUsed(used, CharacterCount);
        }
    }

    /// <summary>Keeps the raised network limit in place.</summary>
    public sealed class LimitKeeper : MonoBehaviour
    {
        public LimitKeeper(IntPtr ptr) : base(ptr) { }

        private float _next;

        private void Update()
        {
            if (Time.unscaledTime < _next)
                return;
            _next = Time.unscaledTime + 0.5f;
            MorePlayers.RaiseNetworkLimit();
        }
    }
}
