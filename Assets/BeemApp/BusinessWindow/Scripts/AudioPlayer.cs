using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioPlayer
/// </summary>
public class AudioPlayer : MonoBehaviour {
    [SerializeField]
    private AudioSource _audioSource;

    /// <summary>
    /// Play Audio
    /// </summary>
    public void Play() {
        _audioSource.Play();
    }
}
