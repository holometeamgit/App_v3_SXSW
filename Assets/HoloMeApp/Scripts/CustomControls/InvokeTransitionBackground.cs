using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class InvokeTransitionBackground : MonoBehaviour
{
    [SerializeField]
    Button buttonToInvoke;

    private void Awake()
    {
        if (buttonToInvoke != null && buttonToInvoke.onClick != null)
        {
            var button = gameObject.GetComponent<Button>();
            if(button == null)
            {
                Debug.LogError("Button wasn't on gameobject");
                return;
            }
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(buttonToInvoke.onClick.Invoke);
        }
    }
}
