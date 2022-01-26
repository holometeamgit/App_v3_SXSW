using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// Use this script to animate a horizontal scroll list of buttons. 
/// Disable the scroll rect in the editor to prevent manual scrolling
/// Disable mask to allow buttons to still be interactable
/// Viewport should be the side of a single button as pressed buttons will be centralised to it.
/// </summary>
public class ScrollRectSnapButtonHorz : MonoBehaviour {
    [SerializeField]
    ScrollRect scrollRect;

    [SerializeField]
    Button[] buttons;

    [SerializeField]
    float transitionSpeed = 0.25f;

    [SerializeField]
    Color selectedButtonColor;

    [SerializeField]
    float scaleUpFactor = 1.5f;

    float segmentSectionSize;

    Color previousTextColor;
    int previousIndex = -1;
    bool originalColorAssigned;

    private void Awake() {

        for (int i = 0; i < buttons.Length; i++) {
            int j = i;
            buttons[i].onClick.AddListener(() => RegisterIndex(j));
        }

        RectTransform scrollSnapRect = GetComponent<RectTransform>();

        scrollRect.horizontalNormalizedPosition = 0;
        segmentSectionSize = 1f / (buttons.Length - 1); //Viewport should be the same size/width as each button element

        RegisterIndex(0);
    }

    /// <summary>
    /// This should be called by the button's UnityEvent to trigger the animation
    /// </summary>
    /// <param name="index">Index of the button being pressed</param>
    public void RegisterIndex(int index) {

        if (index < 0 && index >= buttons.Length) {
            HelperFunctions.DevLogError("Button index was out of range");
            return;
        }

        print("RegisterIndex Called " + index);
        print("segment  restul = " + segmentSectionSize * index);

        //Kill tweens before calling again

        DOTween.To(() => scrollRect.horizontalNormalizedPosition, x => scrollRect.horizontalNormalizedPosition = x, segmentSectionSize * index, transitionSpeed);

        ResetLastIndexVariables();

        TextMeshProUGUI textMesh = buttons[index].GetComponentInChildren<TextMeshProUGUI>();
        if (!originalColorAssigned) {
            previousTextColor = textMesh.color;
            originalColorAssigned = true;
        }
        previousIndex = index;
        textMesh.DOColor(selectedButtonColor, transitionSpeed);

        buttons[index].transform.DOScale(new Vector2(scaleUpFactor, scaleUpFactor), transitionSpeed);
    }

    private void ResetLastIndexVariables() {
        if (previousIndex != -1 && previousIndex < buttons.Length) {
            buttons[previousIndex].GetComponentInChildren<TextMeshProUGUI>().DOColor(previousTextColor, transitionSpeed);
            buttons[previousIndex].transform.DOScale(Vector2.one, transitionSpeed);
        }
    }

}
