using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlBroadcasterProfile : MonoBehaviour
{
    [SerializeField] AccountManager accountManager; 
    [SerializeField] GameObject menuProfileBurger;
    [SerializeField] GameObject menuUserProfileBurger;
    [Space]
    [SerializeField] GameObject broadcasterProfile;
    [Space]
    [SerializeField] Button menuBtn;
         

    public void ShowMenu() {
        var accauntType = accountManager.GetAccountType();
        menuProfileBurger.SetActive(accauntType == AccountManager.AccountType.Broadcater);
        menuUserProfileBurger.SetActive(accauntType == AccountManager.AccountType.Subscriber);
        menuBtn.gameObject.SetActive(false);
    }

    private void OnEnable() {
        menuProfileBurger.SetActive(false);
        menuUserProfileBurger.SetActive(false);

        menuBtn.gameObject.SetActive(true);
    }

    private void OnDisable() {
        menuBtn.gameObject.SetActive(false);
    }
}
