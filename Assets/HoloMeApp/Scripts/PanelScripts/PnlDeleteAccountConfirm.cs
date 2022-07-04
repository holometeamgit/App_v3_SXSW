using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PnlDeleteAccountConfirm : MonoBehaviour {

    [SerializeField] Switcher switchLogOut;

    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    public void DeleteAccount() {
        //TODO: update it later 
        _userWebManager.DeleteUserAccount();
    }

    private void UserAccountDeletedCallBack() {
        switchLogOut.Switch();
    }

    private void OnEnable() {
        _userWebManager.OnUserAccountDeleted += UserAccountDeletedCallBack;
    }

    private void OnDisable() {
        _userWebManager.OnUserAccountDeleted -= UserAccountDeletedCallBack;
    }
}
