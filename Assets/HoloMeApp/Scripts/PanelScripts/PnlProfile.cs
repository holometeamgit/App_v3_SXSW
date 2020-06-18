using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//TODO update the functionality after it appears on the server
public class PnlProfile : MonoBehaviour
{
    public delegate void SaveAccesseDelegate();

    public UnityEvent OnUsernameChoosed;
    private SaveAccesseDelegate saveAccesseDelegate;

    public void SetActionOnSignUp(SaveAccesseDelegate saveAccesseDelegate) {
        this.saveAccesseDelegate = saveAccesseDelegate;
    }

    public void ChooseUserName() {
        saveAccesseDelegate.Invoke();
        OnUsernameChoosed.Invoke();
    }
}
