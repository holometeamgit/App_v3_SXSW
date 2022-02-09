using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BroadcasterData
/// </summary>
public class BroadcasterData {

    public BroadcasterData(bool isRoom) {
        _isRoom = isRoom;
    }

    private bool _isRoom;

    public bool IsRoom {
        get {
            return _isRoom;
        }
    }
}
