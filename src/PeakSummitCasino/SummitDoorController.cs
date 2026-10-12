using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PeakSummitCasino;

public sealed class SummitDoorController : MonoBehaviour
{
    private const float DefaultRoomDuration = 300f;
    private const string TestRoomSceneName = "CasinoTestRoom";

    private readonly HashSet<GameObject> _playersInside = new();

    private float _roomDurationSeconds = DefaultRoomDuration;
    private float _timer;
    private bool _roomActive;
    private bool _debugLogging;
    private Scene _testRoomScene;
    private bool _testRoomLoaded;

    public void SetRoomDuration(float seconds)
    {
        _roomDurationSeconds = Mathf.Max(1f, seconds);
    }

    public void SetDebugLogging(bool enabled)
    {
        _debugLogging = enabled;
    }

    private void Start()
    {
        ResetRoom();
        if (_debugLogging)
            Debug.Log("SummitDoorController initialized and ready for player interaction.");
    }

    private void Update()
    {
        if (!_roomActive)
            return;

        _timer -= Time.deltaTime;

        if (_debugLogging)
            Debug.Log($"[CasinoRoom] Timer: {_timer:F1}s / {_roomDurationSeconds:F1}s");

        if (_timer <= 0f)
        {
            ExpireRoom();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other || !other.attachedRigidbody)
            return;

        var playerObject = other.gameObject;
        if (!playerObject.name.Contains("Player", System.StringComparison.OrdinalIgnoreCase))
            return;

        // Ignore if player is already inside
        if (_playersInside.Contains(playerObject))
            return;

        _playersInside.Add(playerObject);

        if (_debugLogging)
            Debug.Log($"[CasinoRoom] Player entered door trigger: {playerObject.name}");

        // Open the room when first player enters
        if (!_roomActive)
        {
            OpenRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other)
            return;

        var playerObject = other.gameObject;
        if (_playersInside.Contains(playerObject))
        {
            _playersInside.Remove(playerObject);

            if (_debugLogging)
                Debug.Log($"[CasinoRoom] Player left door trigger: {playerObject.name}");
        }
    }

    private void OpenRoom()
    {
        if (_debugLogging)
            Debug.Log("[CasinoRoom] Opening casino room...");

        _roomActive = true;
        _timer = _roomDurationSeconds;

        // Load the test casino room (additive load so summit stays loaded)
        if (!_testRoomLoaded)
        {
            LoadCasinoTestRoom();
        }

        if (_debugLogging)
            Debug.Log("[CasinoRoom] Room is now active with " + _playersInside.Count + " player(s).");
    }

    private void LoadCasinoTestRoom()
    {
        // Try to load the casino test room additively
        // If it doesn't exist as a real scene, create a simple test environment
        if (SceneManager.GetSceneByName(TestRoomSceneName).IsValid())
        {
            _testRoomLoaded = true;
            if (_debugLogging)
                Debug.Log("[CasinoRoom] Test room scene already loaded.");
            return;
        }

        try
        {
            // Attempt to load the scene additively
            SceneManager.LoadScene(TestRoomSceneName, LoadSceneMode.Additive);
            _testRoomLoaded = true;
            if (_debugLogging)
                Debug.Log("[CasinoRoom] Loaded test casino room scene: " + TestRoomSceneName);
        }
        catch (System.Exception ex)
        {
            // If the scene doesn't exist, create a minimal test environment
            if (_debugLogging)
                Debug.LogWarning($"[CasinoRoom] Could not load scene '{TestRoomSceneName}': {ex.Message}. Creating fallback test room...");

            CreateFallbackTestRoom();
            _testRoomLoaded = true;
        }
    }

    private void CreateFallbackTestRoom()
    {
        // Create a simple test room GameObject structure
        var testRoom = new GameObject("CasinoTestRoomRoot");
        testRoom.transform.position = new Vector3(0f, 0f, 50f); // Offset from summit

        // Add a floor
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "TestRoomFloor";
        floor.transform.parent = testRoom.transform;
        floor.transform.localPosition = Vector3.zero;
        floor.transform.localScale = new Vector3(5f, 1f, 5f);
        
        var floorCollider = floor.GetComponent<BoxCollider>();
        if (floorCollider != null) floorCollider.enabled = true;
        if (floor.GetComponent<Rigidbody>()) 
            DestroyImmediate(floor.GetComponent<Rigidbody>());

        // Add some walls (simple cubes)
        for (int i = 0; i < 4; i++)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "TestRoomWall_" + i;
            wall.transform.parent = testRoom.transform;
            wall.transform.localScale = new Vector3(5f, 3f, 0.2f);

            switch (i)
            {
                case 0: wall.transform.localPosition = new Vector3(0f, 1.5f, 5f); break;      // Front
                case 1: wall.transform.localPosition = new Vector3(0f, 1.5f, -5f); break;     // Back
                case 2: wall.transform.localPosition = new Vector3(5f, 1.5f, 0f); break;      // Right
                case 3: wall.transform.localPosition = new Vector3(-5f, 1.5f, 0f); break;     // Left
            }

            var wallCollider = wall.GetComponent<BoxCollider>();
            if (wallCollider != null) wallCollider.enabled = true;
            if (wall.GetComponent<Rigidbody>())
                DestroyImmediate(wall.GetComponent<Rigidbody>());

            // Make walls semi-transparent so we can see inside
            var renderer = wall.GetComponent<MeshRenderer>();
            if (renderer)
            {
                var mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                renderer.material = mat;
            }
        }

        // Add a simple info text billboard
        var billboard = new GameObject("CasinoTestRoomInfo");
        billboard.transform.parent = testRoom.transform;
        billboard.transform.localPosition = new Vector3(0f, 2f, 0f);

        if (_debugLogging)
            Debug.Log("[CasinoRoom] Fallback test room created successfully.");
    }

    private void ResetRoom()
    {
        _roomActive = false;
        _timer = 0f;
        _playersInside.Clear();

        if (_testRoomLoaded && _testRoomScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(_testRoomScene);
            _testRoomLoaded = false;
        }

        if (_debugLogging)
            Debug.Log("[CasinoRoom] Room reset.");
    }

    private void ExpireRoom()
    {
        if (_debugLogging)
            Debug.Log("[CasinoRoom] Room time expired. Closing casino room and returning players to summit...");

        _roomActive = false;

        // Unload test room
        if (_testRoomLoaded)
        {
            var testRoomRoot = GameObject.Find("CasinoTestRoomRoot");
            if (testRoomRoot != null)
            {
                DestroyImmediate(testRoomRoot);
            }
            _testRoomLoaded = false;
        }

        _playersInside.Clear();

        if (_debugLogging)
            Debug.Log("[CasinoRoom] Players returned to PEAK summit. Room is available again.");
    }
}
