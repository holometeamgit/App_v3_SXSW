﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlWelcomeV3 : MonoBehaviour
{
    [SerializeField]
    Switcher switcher;

    const string PrefHasSeenWelcome = nameof(PrefHasSeenWelcome);

    public void CheckIfSeen() {
        //PlayerPrefs.DeleteAll();
        int HasSeen = PlayerPrefs.GetInt(PrefHasSeenWelcome, 0);
        if (HasSeen == 1) {
            gameObject.GetComponent<AnimatedTransition>().enabled = false;
            gameObject.SetActive(true);
            Invoke(nameof(ShowNextPanel), .5f);
        } else {
            gameObject.SetActive(true);
        }
    }

    private void ShowNextPanel() {
        switcher.Switch();
        gameObject.SetActive(false);
    }

    //Link to close button
    public void SetToSeen() {
        PlayerPrefs.SetInt(PrefHasSeenWelcome, 1);
        ShowNextPanel();
    }
}
