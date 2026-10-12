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
    private GameObject? _playerCharacter;

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
            "Position of the casino door in the summit scene."
        );

        SceneManager.sceneLoaded += OnSceneLoaded;
        _log.LogInfo("PeakSummitCasino loaded. Press F1 to log player coordinates. Waiting for summit scene...");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // F1 debug feature: log player coordinates
        if (Input.GetKeyDown(KeyCode.F1))
        {
            LogPlayerCoordinates();
        }
    }

    private void LogPlayerCoordinates()
    {
        // Try to find the player character if we haven't already
        if (_playerCharacter == null)
        {
            _playerCharacter = FindPlayerCharacter();
        }

        if (_playerCharacter == null)
        {
            _log?.LogWarning("[DEBUG] Player character not found. Make sure you're in the game.");
            Debug.LogWarning("[DEBUG] Player character not found. Make sure you're in the game.");
            return;
        }

        var position = _playerCharacter.transform.position;
        var message = $"[DEBUG] Player Position: X={position.x:F2}, Y={position.y:F2}, Z={position.z:F2}";

        _log?.LogInfo(message);
        Debug.Log(message);
        
        // Also log the scene name and all nearby objects
        var currentScene = SceneManager.GetActiveScene();
        _log?.LogInfo($"[DEBUG] Current Scene: {currentScene.name}");
        Debug.Log($"[DEBUG] Current Scene: {currentScene.name}");
    }

    private GameObject? FindPlayerCharacter()
    {
        // Search for player by common names
        string[] playerNames = { "Player", "player", "Character", "character", "PlayerCharacter", "playerCharacter" };

        foreach (var name in playerNames)
        {
            var found = GameObject.Find(name);
            if (found != null)
            {
                _log?.LogInfo($"[DEBUG] Found player object: {found.name}");
                return found;
            }
        }

        // Search all active game objects with "Player" in the name
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("Player", StringComparison.OrdinalIgnoreCase))
            {
                _log?.LogInfo($"[DEBUG] Found player object: {obj.name}");
                return obj;
            }
        }

        // Search for any object with a CharacterController component (common player pattern)
        var charControllers = FindObjectsOfType<CharacterController>();
        if (charControllers.Length > 0)
        {
            _log?.LogInfo($"[DEBUG] Found player via CharacterController: {charControllers[0].gameObject.name}");
            return charControllers[0].gameObject;
        }

        // Search for any object with a Rigidbody that might be the player
        var rigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (rb.gameObject.name.Contains("Player", StringComparison.OrdinalIgnoreCase))
            {
                _log?.LogInfo($"[DEBUG] Found player via Rigidbody: {rb.gameObject.name}");
                return rb.gameObject;
            }
        }

        _log?.LogWarning("[DEBUG] Could not find player character by any method.");
        return null;
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
        if (door == null)
            return;

        door.SetRoomDuration(_roomDurationSeconds.Value);
        door.SetDebugLogging(_debugLogging.Value);
        _doorsByScene[scene.name] = door;

        _log?.LogInfo($"Summit casino door initialized in scene '{scene.name}' at {_doorPosition.Value}.");
    }

    private static bool ShouldHandleScene(Scene scene)
    {
        var name = scene.name;
        return name.Contains(SummitSceneKeyword, StringComparison.OrdinalIgnoreCase);
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
        doorObj.transform.position = _doorPosition.Value;
        doorObj.transform.rotation = Quaternion.identity;

        var meshFilter = doorObj.AddComponent<MeshFilter>();
        var meshRenderer = doorObj.AddComponent<MeshRenderer>();
        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

        var material = new Material(Shader.Find("Standard"));
        material.color = new Color(0.75f, 0.35f, 0.8f, 1f);
        meshRenderer.material = material;

        doorObj.transform.localScale = new Vector3(3f, 4f, 0.25f);

        var trigger = doorObj.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1f, 1f, 1f);

        var rb = doorObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        var controllerComponent = doorObj.AddComponent<SummitDoorController>();
        controllerComponent.SetDebugLogging(_debugLogging.Value);
        controllerComponent.SetRoomDuration(_roomDurationSeconds.Value);

        _log?.LogInfo($"Created visible summit door in scene '{scene.name}'.");
        return controllerComponent;
    }
}
