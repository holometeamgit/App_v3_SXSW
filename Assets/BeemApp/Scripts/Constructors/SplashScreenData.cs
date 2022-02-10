using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SplashScreenData
/// </summary>
public class SplashScreenData {

    public SplashScreenData(bool isLastVersion) {
        _isLastVersion = isLastVersion;
    }

    private bool _isLastVersion;

    public bool IsLastVersion {
        get {
            return _isLastVersion;
        }
    }
}
