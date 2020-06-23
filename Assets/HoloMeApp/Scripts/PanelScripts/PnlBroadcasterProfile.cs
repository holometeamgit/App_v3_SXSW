using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlBroadcasterProfile : MonoBehaviour
{
    [SerializeField] bool _isUser; //TODO update after add user type definition
    [SerializeField] GameObject _menuProfileBurger;
    [SerializeField] GameObject _menuUserProfileBurger;
    [Space]
    [SerializeField] GameObject _broadcasterProfile;
    [SerializeField] GameObject _broadcasterUserProfile;
    [Space]
    [SerializeField] Button _menuBtn;
         

    public void ShowMenu() {
        _menuProfileBurger.SetActive(!_isUser);
        _menuUserProfileBurger.SetActive(_isUser);
        _menuBtn.gameObject.SetActive(false);
    }

    private void OnEnable() {
        _broadcasterUserProfile.SetActive(!_isUser);
        _broadcasterProfile.SetActive(_isUser);
        _menuBtn.gameObject.SetActive(true);
    }

    private void OnDisable() {
        _menuBtn.gameObject.SetActive(false);
    }
}
