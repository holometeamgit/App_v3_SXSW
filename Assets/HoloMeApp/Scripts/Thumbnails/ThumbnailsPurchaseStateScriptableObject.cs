using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ThumbnailsPurchaseState", menuName = "Data/Thumbnail/ThumbnailsPurchaseState")]
public class ThumbnailsPurchaseStateScriptableObject : ScriptableObject {
    public List<Sprite> ThumbnailIcons;
}
