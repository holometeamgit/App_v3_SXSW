using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Server Json Data
/// </summary>
public interface IData {
    string Id { get; }
    string ShareLink { get; }
    string Username { get; }
    string Status { get; }
}
