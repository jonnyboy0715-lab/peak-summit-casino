using System.Collections.Generic;
using UnityEngine;

namespace PeakSummitCasino;

public sealed class SummitDoorController : MonoBehaviour
{
    private const float DefaultRoomDuration = 300f;
    private readonly HashSet<GameObject> _playersInside = new();

    private float _roomDurationSeconds = DefaultRoomDuration;
    private float _timer;
    private bool _roomActive;
    private bool _debugLogging;

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
    }

    private void Update()
    {
        if (!_roomActive)
            return;

        _timer -= Time.deltaTime;

        if (_debugLogging)
            Debug.Log($"PeakSummitCasino room timer: {_timer:F1}s remaining");

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

        _playersInside.Add(playerObject);

        if (!_roomActive)
        {
            OpenRoom();
        }

        if (_debugLogging)
            Debug.Log($"Player entered summit room: {playerObject.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other)
            return;

        var playerObject = other.gameObject;
        if (_playersInside.Contains(playerObject))
            _playersInside.Remove(playerObject);

        if (_debugLogging)
            Debug.Log($"Player left summit room: {playerObject.name}");
    }

    private void OpenRoom()
    {
        _roomActive = true;
        _timer = _roomDurationSeconds;
        if (_debugLogging)
            Debug.Log("PeakSummitCasino room opened.");
    }

    private void ResetRoom()
    {
        _roomActive = false;
        _timer = 0f;
        _playersInside.Clear();
        if (_debugLogging)
            Debug.Log("PeakSummitCasino room reset.");
    }

    private void ExpireRoom()
    {
        _roomActive = false;
        _playersInside.Clear();

        if (_debugLogging)
            Debug.Log("PeakSummitCasino room expired. Players are kicked back to the PEAK run loop.");

        // This is the hook for the actual PEAK scene flow after a real summit room is mapped in-game.
        // The final version should return the players to the active run and keep the room locked until
        // the level is completed.
    }
}
