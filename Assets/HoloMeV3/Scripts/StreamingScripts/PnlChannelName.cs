﻿using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class PnlChannelName : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputChannelName;

    [SerializeField]
    IncorrectInputAnimationToggle incorrectInputAnimationToggle;

    [SerializeField]
    UnityEvent OnChannelNamePassed;

    public static string ChannelName { get; private set; }

    public void OnReadyPressed()
    {
        //Any verification and validation should go here
        if (string.IsNullOrWhiteSpace(inputChannelName.text))
        {
            incorrectInputAnimationToggle.StartIncorrectAnimation();
        }
        else
        {
            ChannelName = inputChannelName.text;
            OnChannelNamePassed?.Invoke();
        }
    }

}
