using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PeakSummitCasino;

[BepInPlugin(ModId, ModName, ModVersion)]
public class SummitGamblingRoomPlugin : BaseUnityPlugin
{
    public const string ModId = "jonnyboy0715-lab.peak-summit-casino";
    public const string ModName = "PeakSummitCasino";
    public const string ModVersion = "0.1.0";

    internal static ManualLogSource Log => Logger;

    private readonly Harmony _harmony = new(ModId);

    private ConfigEntry<float> _roomDurationSeconds;
    private ConfigEntry<bool> _debugLogging;

    private bool _roomActive;
    private float _roomTimer;
    private bool _doorSpawned;

    private void Awake()
    {
        _roomDurationSeconds = Config.Bind(
            "SummitGamblingRoom",
            "RoomDurationSeconds",
            300f,
            "How many seconds the gambling room remains open before kicking everyone out."
        );

        _debugLogging = Config.Bind(
            "SummitGamblingRoom",
            "DebugLogging",
            true,
            "Show extra debug logging while the mod is being tested."
        );

        _harmony.PatchAll();

        Log.LogInfo("PeakSummitCasino loaded.");
        Log.LogInfo($"Room duration set to {_roomDurationSeconds.Value} seconds.");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _harmony.UnpatchSelf();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_debugLogging.Value)
            Log.LogInfo($"Loaded scene: {scene.name}");

        // Hook point for PEAK summit scene creation.
        // This is where a real PEAK mod would locate the summit, add the door trigger,
        // and connect it to a room flow tied to the map and co-op session.
        if (scene.name.Contains("Peak") || scene.name.Contains("Main") || scene.name.Contains("Game"))
        {
            if (!_doorSpawned)
            {
                SpawnSummitDoorMarker(scene);
                _doorSpawned = true;
            }
        }
    }

    private void SpawnSummitDoorMarker(Scene scene)
    {
        GameObject marker = new("PeakSummitCasino_DoorMarker");
        marker.transform.position = Vector3.zero;

        // This is intentionally lightweight. The actual PEAK summit geometry and collider setup
        // must be attached to the real scene once the exact summit root is discovered.
        marker.AddComponent<MarkerBehaviour>();

        Log.LogInfo($"Summit door marker spawned for scene '{scene.name}'.");
    }

    private void Update()
    {
        if (!_roomActive)
            return;

        _roomTimer -= Time.deltaTime;

        if (_roomTimer <= 0f)
        {
            KickPlayersOut();
            _roomActive = false;
        }
    }

    private void KickPlayersOut()
    {
        Log.LogInfo("Summit gambling room expired. Players are being kicked back to the level flow.");

        // This is the point where the real PEAK mod would return players to the mountain run and
        // disable the room before they can re-enter it until the run is complete.
    }

    private void EnterRoom()
    {
        _roomActive = true;
        _roomTimer = _roomDurationSeconds.Value;
        Log.LogInfo("Players entered the summit gambling room.");
    }

    private void OnSummitDoorTriggered()
    {
        // The actual door trigger should call this once the players are in the summit area.
        EnterRoom();
    }
}

public class MarkerBehaviour : MonoBehaviour
{
    private void Start()
    {
        // Placeholder behaviour for the summit door marker.
    }
}

[HarmonyPatch]
public static class SummitRoomPatch
{
    [HarmonyPatch(typeof(SceneManager), "LoadScene")]
    [HarmonyPrefix]
    public static void LoadScenePrefix(string sceneName)
    {
        // Hook location reserved for PEAK summit scene manipulation.
    }
}
