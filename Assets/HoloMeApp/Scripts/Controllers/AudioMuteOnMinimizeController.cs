using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// When minimize the app sound will be mute
/// </summary>
public class AudioMuteOnMinimizeController : MonoBehaviour
{
    private void OnApplicationFocus(bool focus) {
        AudioListener.volume = focus ? 1 : 0;
    }
}
