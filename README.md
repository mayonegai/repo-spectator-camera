# REPO Spectator Camera Mod

Free camera spectator mode for R.E.P.O game.

## Why

I wanted to check out level to study how map is generated, it's pretty shit so don't blame me for anything.
You **should** pair this mod with [FullBright](https://thunderstore.io/c/repo/p/Omniscye/FullBright/) mod as the game's postprocessing makes everything veeery dark.

## Features
- Detach camera from player and fly around freely
- Take screenshots from any angle
- Adjustable movement speed

## Installation
1. Make sure you have [BepInExPack](https://github.com/BepInEx/BepInEx/releases) installed (r2modman works)
2. Download `SpectatorCameraMod.dll` from releases
3. Place it in `BepInEx/plugins/` 
    > if using r2modman - go to the settings, Browse profile folder -> drop the dll in the specified folder

## Controls
- **F9** - Toggle spectator mode
- **WASD** - Move camera
- **Mouse** - Look around
- **Space** - Move up
- **Ctrl** - Move down
- **Shift** - Move faster
- **+/-** - Adjust speed

## Building from Source
```bash
git clone https://github.com/mayonegai/repo-spectator-camera
cd repo-spectator-camera
# edit the csproj with proper paths (sorry I'm not a dotnet dev so I couldn't figure it out)
dotnet build -c Release
```

The compiled DLL will be in `bin/Release/netstandard2.1/`

## Notes
- Player is frozen while in spectator mode
- Camera automatically reparents when toggling off
- Works alongside other mods

## License
MIT
