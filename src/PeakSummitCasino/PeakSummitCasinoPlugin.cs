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
    private const string SummitSceneKeyword = "summit";

    private ConfigEntry<float> _roomDurationSeconds;
    private ConfigEntry<bool> _debugLogging;
    private ConfigEntry<Vector3> _doorPosition;
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

        _doorPosition = Config.Bind(
            "SummitGamblingRoom",
            "DoorPosition",
            new Vector3(0f, 5f, 10f),
            "Position of the casino door at the summit (X, Y, Z)."
        );

        SceneManager.sceneLoaded += OnSceneLoaded;

        _log.LogInfo("PeakSummitCasino prototype loaded. Waiting for summit scene...");
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
            _log?.LogInfo($"Reused existing door in scene '{scene.name}'.");
            return;
        }

        var door = CreateSummitDoor(scene);
        door.SetRoomDuration(_roomDurationSeconds.Value);
        door.SetDebugLogging(_debugLogging.Value);
        _doorsByScene[scene.name] = door;

        _log?.LogInfo($"Summit casino door initialized in scene '{scene.name}' at position {_doorPosition.Value}.");
    }

    private static bool ShouldHandleScene(Scene scene)
    {
        var name = scene.name;
        // Only handle the summit scene specifically
        return name.Contains(SummitSceneKeyword, StringComparison.OrdinalIgnoreCase);
    }

    private SummitDoorController CreateSummitDoor(Scene scene)
    {
        var existing = GameObject.Find(SummitDoorName);
        if (existing != null)
        {
            var controller = existing.GetComponent<SummitDoorController>();
            if (controller != null)
            {
                _log?.LogWarning("Door already exists; reusing it.");
                return controller;
            }
        }

        // Create the door game object with a visible representation
        var doorObj = new GameObject(SummitDoorName);
        doorObj.transform.position = _doorPosition.Value;
        doorObj.transform.rotation = Quaternion.identity;

        // Add a visible mesh (cube as placeholder for a door)
        var meshFilter = doorObj.AddComponent<MeshFilter>();
        var meshRenderer = doorObj.AddComponent<MeshRenderer>();
        
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        
        var mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.8f, 0.2f, 0.8f, 1f); // Purple-ish color for visibility
        meshRenderer.material = mat;

        // Scale the door to look like a doorway
        doorObj.transform.localScale = new Vector3(3f, 4f, 0.2f);

        // Add collider for trigger detection
        var collider = doorObj.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = Vector3.one; // Size is 1,1,1 since the mesh is scaled

        // Add rigidbody (kinematic so it doesn't fall or move)
        var rb = doorObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Attach the controller
        var controllerComponent = doorObj.AddComponent<SummitDoorController>();
        controllerComponent.SetDebugLogging(_debugLogging.Value);
        controllerComponent.SetRoomDuration(_roomDurationSeconds.Value);

        _log?.LogInfo($"Created summit door at position {_doorPosition.Value} in scene '{scene.name}'.");
        return controllerComponent;
    }
}
