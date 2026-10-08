# Peak Summit Casino v0.1.0-alpha.1

This is a prerelease prototype for PEAK testing.

## Summary

A BepInEx prototype that adds a summit door marker and time-limited gambling room flow for PEAK.

## Included

- summit door marker creation on PEAK scene load
- room timer state tracking
- player enter/exit detection for the room trigger volume
- debug logging for room creation and expiry
- configuration entries for room duration and debug mode

## Known limitations

- This is not a verified in-game playable build.
- The exact PEAK summit scene geometry and real room hookup are not yet mapped.
- The actual "kick players back to the run" flow is still stubbed and requires integration with the real PEAK level flow.
- Multiplayer flow and room unlock behavior are intentionally left for real lobby testing.

## Install

Place the compiled DLL in:

`C:\Program Files (x86)\Steam\steamapps\common\PEAK\BepInEx\plugins\PeakSummitCasino.dll`

## Build

```bash
dotnet build src/PeakSummitCasino/PeakSummitCasino.csproj
```

## Notes

This prerelease is intended for local source testing inside a real PEAK lobby before polishing the summit integration and gameplay loop.
