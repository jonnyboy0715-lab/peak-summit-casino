# Peak Summit Casino

A PEAK-first BepInEx prototype for a summit door that opens into a time-limited gambling room.

This project is intentionally a source-first prototype rather than a verified, played build. It is designed to be compiled against the PEAK install on this PC and tested in a real co-op lobby before being packaged for Melty.

## What it does

- Adds a summit door marker in the PEAK scene flow.
- Creates a room trigger that can be connected to the real summit geometry once the exact PEAK prefab and scene structure are mapped.
- Tracks the room timer and kicks players out after the configured window.
- Provides a clean hook for the room to be re-enabled only after the level is completed.

## Intended flow

1. The player reaches the summit.
2. The summit door becomes active.
3. All players enter the gambling room together.
4. A timer counts down.
5. The room expires and players are returned to the run.
6. The room unlocks again only after the level is completed and a new PEAK run begins.

## Install layout

Place the compiled DLL in:

`C:\Program Files (x86)\Steam\steamapps\common\PEAK\BepInEx\plugins\PeakSummitCasino.dll`

## Project layout

- `src/PeakSummitCasino/PeakSummitCasino.csproj` - project definition
- `src/PeakSummitCasino/PeakSummitCasinoPlugin.cs` - plugin entry point
- `src/PeakSummitCasino/SummitDoorController.cs` - summit trigger and room timer logic

## Important note

This is not a tested release. It is the source foundation for the first playable build. The next step is to install and test it inside PEAK with a real lobby, then refine the summit door placement and room timing against the actual PEAK scene and player flow.
