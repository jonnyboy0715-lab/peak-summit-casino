# Peak Summit Casino

A PEAK-first BepInEx mod concept that adds a summit door to a time-limited gambling room.

This repository is a starting point for a BepInEx plugin that sits inside PEAK and adds:
- a summit door or trigger at the climb endpoint
- a multiplayer gambling room
- a timer that kicks everyone out after a fixed window
- a full run reset so the room can only re-open after finishing the level

This project is not yet a completed release. It is the foundation for the build/test loop and is designed to be placed in your PEAK install under `BepInEx/plugins` once the PEAK assemblies and mod loader are available in the game folder.

## Intended design

- Host game: PEAK
- Mod type: BepInEx plugin
- Multiplayer: PEAK co-op players enter together
- Gamblers: shared pot and team-based gambling, with a timer instead of endless room access
- Portal flow: summit door -> room -> timeout -> return to PEAK level flow -> complete run -> room unlocks next time

## Install layout

Place the compiled DLL in:

`<PEAK install folder>/BepInEx/plugins/PeakSummitCasino.dll`

If your PEAK install is under a custom Steam library path, use that folder instead.

## Repo structure

- `src/PeakSummitCasino/PeakSummitCasino.csproj` - project file for the plugin
- `src/PeakSummitCasino/SummitGamblingRoomPlugin.cs` - plugin entry point and runtime logic

## Next step

Compile the project against your local PEAK installation and then test in the game with a PEAK co-op lobby. Once the summit door and room flow are proven, package the DLL with a release zip for Melty.
