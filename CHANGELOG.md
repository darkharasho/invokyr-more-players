# Changelog

## 0.1.2
- Fix the game crashing on launch: no longer patches `VhsNetworkManager.Awake` (the patch crashed inside IL2CPP). The limit is still raised every half second.

## 0.1.1
- Declare the BepInEx IL2CPP pack as a dependency, so mod managers install BepInEx with this mod.

## 0.1.0
- First, experimental release: raises the network and Steam lobby limits and gives players past the sixth a slot, character and spawn point.
