## Project Context

A BepInEx 6 (BepInEx.Unity.IL2CPP, be.755) mod for Invokyr (Unity 6000.3, IL2CPP, Mirror, Steam lobbies): raises the player limit (vanilla 6) for games you host. Experimental. Game code is in `vhs.dll`. Packaged Thunderstore-style under `darkharasho`, released on GitHub. Build setup is shared with `invokyr-admin-menu` (see its CLAUDE.md for refs, packaging and the flat zip layout HML needs). References also `com.rlabrecque.steamworks.net` and `EnhancedFramework.Core` (UserInstancesManager is an EnhancedSingleton).

## What's patched (`MorePlayers.cs`)
- Limits: `VhsNetworkManager.MaxPlayers` (static) and Mirror `NetworkManager.singleton.maxConnections`, set at load, in a `VhsNetworkManager.Awake` postfix and every 0.5 s (`LimitKeeper`), in case the game resets them. `SteamLobby.Create` arg `maxMembers` (index 3) and `SteamMatchmaking.CreateLobby` arg `cMaxMembers`. `SteamMatchmaking.AddRequestLobbyListNumericalFilter`: a player-count key with value 6 becomes the cap (client side, lobby browser).
- Players 7+: `UserInstancesManager.SrvRefillAvailableIndices` postfix adds indices up to the cap to `_availablePlayerIndices`; `SrvGetFirstFreeCharacter` / `UserInstance.SrvChooseCharacter` fall back to the least-used character when the game returns None; `SrvGetPlayerCharacterByIndex` and `UserInstance.SrvGetSpawnTransformForIndex` wrap high indices.
- Not handled yet (see the research in the first session): `PortraitManager._portraits` (6 networked portraits in the scene), `GameTable._chairs`, `ShopRoom.playersSpawnPoints`, `SpectatorUI._deadPlayer*` arrays, `PauseMenu.playerInfoUIs`, `EntityManager.IsCharacterInUse` during character swaps. Extra networked portraits/chairs would need matching asset ids on every client (and a version handshake via `VHS_Authenticator`).
- Pure logic in `Slots.cs`, tested. The decompile has no method bodies, so every behaviour assumption must be checked with 7+ players.

## Building
`scripts/package.sh` → `dist/darkharasho-InvokyrMorePlayers-<version>.zip`; `hml install nexus-invokyr <zip> --profile Dev`. Tests: `dotnet test tests/InvokyrMorePlayers.Tests`. Run builds with `nice -n 19 taskset -c 0-3`. Release: bump `Plugin.cs`, csproj, manifest, CHANGELOG; push tag `v<version>`.
