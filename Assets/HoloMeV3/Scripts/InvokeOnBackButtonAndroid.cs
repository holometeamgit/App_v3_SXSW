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
            if (button??isActiveAndEnabled)
            {
                button?.onClick.Invoke();
            }
        }
    }
#endif
}
