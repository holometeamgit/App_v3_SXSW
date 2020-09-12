using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorStyleList", menuName = "Data/UI/ColorStyleList", order = 112)]
public class ColorStyleScriptableObject : StyleScriptableObject {
    public List<TaggableColor> Colors;
}
