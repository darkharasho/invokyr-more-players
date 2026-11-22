# Invokyr More Players

Raises the player limit for games you host from 6 to `MaxPlayers` (12 by default, up to 16).

## What it changes
- The network and Steam lobby limits, so more players can join.
- Players past the sixth get a player slot, a character and a spawn point. The game has 8 characters and 6 spawn points, so from the ninth player on characters repeat, and spawn points are shared.

## Experimental
This is a first version. The game is built around 6 players, and some of it may not make room for more. The portraits and chairs at the game table, and the list of dead players while spectating, are the likeliest places. Please report what breaks, and with how many players.

## Notes
- The host needs the mod. Players joining by invite or room code don't. To see a full lobby (6 or more players) in the lobby browser, the joining player needs it too.
- Set `MaxPlayers` in `BepInEx/config/com.darkharasho.invokyr.moreplayers.cfg` before you create the game.
- Needs BepInEx 6 for IL2CPP (be.755 or newer).
