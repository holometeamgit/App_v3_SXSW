using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class ToggleSlider : MonoBehaviour
{
    [SerializeField]
    Image imgOn;

    [SerializeField]
    Image imgOff;

    [SerializeField]
    UnityEvent OnToggledOff;

    [SerializeField]
    UnityEvent OnToggledOn;

    bool toggleOn;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (toggleOn)
            {
                SetToOff();
            }
            else
            {
                SetToOn();
            }
        });
    }

    private void OnEnable()
    {
        SetToOff();
    }

    void FlipOnOffImages(bool on)
    {
        imgOn.enabled = on;
        imgOff.enabled = !on;
    }

    void SetToOff()
    {
        FlipOnOffImages(false);
        OnToggledOff?.Invoke();
        toggleOn = false;
    }

    void SetToOn()
    {
        FlipOnOffImages(true);
        OnToggledOn?.Invoke();
        toggleOn = true;
    }

}

