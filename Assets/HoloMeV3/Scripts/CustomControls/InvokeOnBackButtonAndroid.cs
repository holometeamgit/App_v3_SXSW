using UnityEngine;
using UnityEngine.UI;

public class InvokeOnBackButtonAndroid : MonoBehaviour
{
#if UNITY_ANDROID

    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (!button)
        {
            Debug.LogError("No button was attached to the gameobject");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (button ?? isActiveAndEnabled)
            {
                //var canvasGroup = GetComponentsInParent<CanvasGroup>();
                //if (canvasGroup.Length >= 1)
                //{
                //    print($"CANVAS GROUP FOUND alpha = {canvasGroup[0].alpha}  interactable = {canvasGroup[0].interactable}");

                //    if (canvasGroup[0].alpha <= .5f || canvasGroup[0].interactable == false)
                //        return;
                //}

                button?.onClick.Invoke();
            }
        }
    }
#endif
}
