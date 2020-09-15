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
        Debug.Log("UserAccountDeletedCallBack");
        switchLogOut.Switch();
    }

    private void OnEnable() {
        Debug.Log("sub UserAccountDeletedCallBack");
        userWebManager.OnUserAccountDeleted += UserAccountDeletedCallBack;
    }

    private void OnDisable() {
        Debug.Log("unsub UserAccountDeletedCallBack");
        userWebManager.OnUserAccountDeleted -= UserAccountDeletedCallBack;
    }
}
