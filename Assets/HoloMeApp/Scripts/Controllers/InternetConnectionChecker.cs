﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetConnectionChecker : MonoBehaviour {
    private const float DEFAULT_COOLDOWN = 5.0f;
    private const float FAST_COOLDOWN = 1.0f;

    private float currentCooldown;
    [SerializeField]
    private GameObject internetConnactionLostGO;

    private void Start() {
        currentCooldown = DEFAULT_COOLDOWN;
        StartCoroutine(CheckingInternet());
    }

    private void CheckInternetConnection() {
        bool internetIsLost = Application.internetReachability == NetworkReachability.NotReachable;
        internetConnactionLostGO.SetActive(internetIsLost);
        currentCooldown = internetIsLost ? FAST_COOLDOWN : DEFAULT_COOLDOWN;
    }

    IEnumerator CheckingInternet() {
        while (true) {
            CheckInternetConnection();
            yield return new WaitForSeconds(currentCooldown);
        }
    }

    private void OnApplicationPause(bool pause) {
        if(pause) {
            StopAllCoroutines();
        } else {
            StartCoroutine(CheckingInternet());
        }
    }
}
