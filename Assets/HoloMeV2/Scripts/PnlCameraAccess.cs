using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PnlCameraAccess : MonoBehaviour
{
    [SerializeField]
    AnimatedTransition animatedTransition;

    [SerializeField]
    PermissionGranter permissionGranter;

    [SerializeField]
    TextMeshProUGUI txtMessage;

    public UnityEvent OnAccessGranted;

    bool initialStartupOccurred;

    private void OnEnable()
    {
        if (!initialStartupOccurred)
        {
            initialStartupOccurred = true;
            return;
        }

        StartCoroutine(WaitForCameraAccess());

        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    permissionGranter.RequestCameraAccess();
        //}
        //else
        {
            txtMessage.text = "Please enable camera access and restart this app";
        }
    }

    IEnumerator WaitForCameraAccess()
    {
        while (!permissionGranter.HasCameraAccess)
        {
            yield return null;
        }

        //if (Application.platform == RuntimePlatform.Android)
        {
            Application.Quit();
            yield break;
        }

        OnAccessGranted?.Invoke();
        animatedTransition.DoMenuTransition(false);
    }
}
