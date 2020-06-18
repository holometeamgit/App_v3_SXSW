using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ChildrenTargetGraphicList))]
public class ChildrenTargetGraphicButton : Button {

    private ChildrenTargetGraphicList GraphicList;
    protected Graphic[] Graphics {
        get {
            if (GraphicList == null)
                GraphicList = GetComponent<ChildrenTargetGraphicList>();
            if (GraphicList.m_graphics == null) 
                return targetGraphic.transform.GetComponentsInChildren<Graphic>();
            return GraphicList.m_graphics;
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant) {
        Color color;
        switch (state) {
        case SelectionState.Normal:
            color = colors.normalColor;
            break;
        case SelectionState.Highlighted:
            color = colors.highlightedColor;
            break;
        case SelectionState.Pressed:
            color = colors.pressedColor;
            break;
        case SelectionState.Disabled:
            color = colors.disabledColor;
            break;
        case SelectionState.Selected:
            color = colors.selectedColor;
            break;
        default:
            color = Color.black;
            break;
        }
        if (gameObject.activeInHierarchy) {
            switch (transition) {
            case Selectable.Transition.ColorTint:
                ColorTween(color * colors.colorMultiplier, instant);
                break;
            default:
                throw new NotSupportedException();
            }
        }
    }


    private void ColorTween(Color targetColor, bool instant) {
        if (targetGraphic == null) {
            return;
        }

        foreach (Graphic g in Graphics) {
            g.CrossFadeColor(targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
        }
    }
}