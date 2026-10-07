using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PeakSummitCasino;

[BepInPlugin(ModId, ModName, ModVersion)]
public sealed class PeakSummitCasinoPlugin : BaseUnityPlugin
{
    public const string ModId = "jonnyboy0715-lab.peak-summit-casino";
    public const string ModName = "PeakSummitCasino";
    public const string ModVersion = "0.1.0";

    private const string SummitDoorName = "PeakSummitCasino_SummitDoor";

    private ConfigEntry<float> _roomDurationSeconds;
    private ConfigEntry<bool> _debugLogging;
    private static ManualLogSource? _log;

    private readonly Dictionary<string, SummitDoorController> _doorsByScene = new();

    private void Awake()
    {
        _log = Logger;

        _roomDurationSeconds = Config.Bind(
            "SummitGamblingRoom",
            "RoomDurationSeconds",
            300f,
            "Time in seconds before the room kicks players back out to continue the PEAK run."
        );

        _debugLogging = Config.Bind(
            "SummitGamblingRoom",
            "DebugLogging",
            true,
            "Print extra debug information while the mod is being tested."
        );

        SceneManager.sceneLoaded += OnSceneLoaded;

        _log.LogInfo("PeakSummitCasino prototype loaded. This is source-only and unverified until tested inside PEAK.");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!ShouldHandleScene(scene))
            return;

        if (_doorsByScene.TryGetValue(scene.name, out var existingDoor))
        {
            existingDoor.SetRoomDuration(_roomDurationSeconds.Value);
            existingDoor.SetDebugLogging(_debugLogging.Value);
            return;
        }

        var door = CreateSummitDoor(scene);
        door.SetRoomDuration(_roomDurationSeconds.Value);
        door.SetDebugLogging(_debugLogging.Value);
        _doorsByScene[scene.name] = door;
    }

    private static bool ShouldHandleScene(Scene scene)
    {
        var name = scene.name;
        return name.Contains("Peak", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Mountain", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Level", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Map", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Game", StringComparison.OrdinalIgnoreCase);
    }

    private SummitDoorController CreateSummitDoor(Scene scene)
    {
        var existing = GameObject.Find(SummitDoorName);
        if (existing != null)
        {
            var controller = existing.GetComponent<SummitDoorController>();
            if (controller != null)
                return controller;
        }

        var doorObj = new GameObject(SummitDoorName);
        doorObj.transform.position = Vector3.zero;

        var collider = doorObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(3f, 4f, 1f);

        var rb = doorObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var controllerComponent = doorObj.AddComponent<SummitDoorController>();
        controllerComponent.SetDebugLogging(_debugLogging.Value);
        controllerComponent.SetRoomDuration(_roomDurationSeconds.Value);

        _log?.LogInfo($"Created summit door marker for scene '{scene.name}'.");
        return controllerComponent;
    }
}
