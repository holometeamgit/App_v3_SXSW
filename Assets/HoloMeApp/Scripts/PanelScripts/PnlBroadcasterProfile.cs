using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlBroadcasterProfile : MonoBehaviour
{
    [SerializeField] AccountManager accountManager; 
    [SerializeField] AnimatedTransition menuProfileBurger;
    [SerializeField] AnimatedTransition menuUserProfileBurger;
    [Space]
    [SerializeField] GameObject broadcasterProfile;
    [Space]
    [SerializeField] Button menuBtn;
         

    public void ShowMenu() {
        var accauntType = accountManager.GetAccountType();
        menuProfileBurger.DoMenuTransition(accauntType == AccountManager.AccountType.Broadcater);
        menuUserProfileBurger.DoMenuTransition(accauntType == AccountManager.AccountType.Subscriber);
        menuBtn.gameObject.SetActive(false);
    }

    private void OnEnable() {
        //menuProfileBurger.DoMenuTransition(false);
        //menuUserProfileBurger.DoMenuTransition(false);

        menuBtn.gameObject.SetActive(true);
    }

    private void OnDisable() {
        menuBtn.gameObject.SetActive(false);
    }
}
