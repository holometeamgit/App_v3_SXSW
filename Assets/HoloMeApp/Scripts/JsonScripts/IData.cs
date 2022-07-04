using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Server Json Data
/// </summary>
public interface IData {
    string GetId { get; }
    string GetShareLink { get; }
    string GetUsername { get; }
    string GetStatus { get; }
}
