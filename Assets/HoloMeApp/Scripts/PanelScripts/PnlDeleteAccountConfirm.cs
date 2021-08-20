using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlDeleteAccountConfirm : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] Switcher switchLogOut;

    public void DeleteAccount()
    {
        //TODO: update it later 
        userWebManager.DeleteUserAccount();
    }

    private void UserAccountDeletedCallBack()
    {
        switchLogOut.Switch();
    }

    private void OnEnable() {
        userWebManager.OnUserAccountDeleted += UserAccountDeletedCallBack;
    }

    private void OnDisable() {
        userWebManager.OnUserAccountDeleted -= UserAccountDeletedCallBack;
    }
}
