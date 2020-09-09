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
        //userWebManager.DisableUserAccount();
    }

    private void Start()
    {
        userWebManager.OnUserAccountDeleted += UserAccountDeletedCallBack;
    }

    private void UserAccountDeletedCallBack()
    {
        if (!this.isActiveAndEnabled)
            return;

        switchLogOut.Switch();
    }
}
