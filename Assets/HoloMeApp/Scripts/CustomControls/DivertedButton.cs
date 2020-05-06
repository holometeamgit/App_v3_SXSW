using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DivertedButton : MonoBehaviour
{
    [SerializeField]
    Button referenceButton;

    void Awake()
    {
        var button = GetComponent<Button>();
        if (button)
            button.onClick.AddListener(() => referenceButton.onClick?.Invoke());
        else
            Debug.LogError("No button component was found");
    }

}
