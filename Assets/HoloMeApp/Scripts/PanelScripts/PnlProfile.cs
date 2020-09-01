using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//TODO update the functionality after it appears on the server
public class PnlProfile : MonoBehaviour
{
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] GameObject InputDataArea;
    [SerializeField] InputFieldController userName;

    public UnityEvent OnUsernameChoosed;

    public void ChooseUsername() {
        if (!string.IsNullOrEmpty(userName.text)) {
            userWebManager.SetUsername(userName.name);

            OnUsernameChoosed.Invoke();
        }
    }

    private void UserInfoLoadedCallBack() {
        userWebManager.UserInfoLoaded.RemoveListener(UserInfoLoadedCallBack);

        InputDataArea.SetActive(true);
    }

    private void OnEnable() {
        string userName = userWebManager.GetUsername();
        if(userName != null) {
            OnUsernameChoosed.Invoke();
        }

        userWebManager.UserInfoLoaded.RemoveListener(UserInfoLoadedCallBack);
        userWebManager.UserInfoLoaded.AddListener(UserInfoLoadedCallBack);
        userWebManager.LoadUserInfo();
    }



    private void OnDisable() {
        InputDataArea.SetActive(false);
    }
}
