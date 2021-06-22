using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/*
public class CloudBuildHelper : MonoBehaviour {
    private static readonly string _versionNumber;
    private static readonly string _buildNumber;
    static void BuildScript() {
        _versionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");
        if (string.IsNullOrEmpty(_versionNumber))
            _versionNumber = "1.0.0.0";

        _buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");
        if (string.IsNullOrEmpty(_buildNumber))
            _buildNumber = "1";

        PlayerSettings.bundleVersion = _versionNumber;
        PlayerSettings.build
            PlayerSettings.shortBundleVersion = _versionNumber;
    }
}*/