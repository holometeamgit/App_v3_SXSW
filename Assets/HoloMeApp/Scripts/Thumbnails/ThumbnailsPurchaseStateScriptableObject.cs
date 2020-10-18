using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThumbnailsPurchaseState", menuName = "Data/Thumbnail/ThumbnailsPurchaseState")]
public class ThumbnailsPurchaseStateScriptableObject : ScriptableObject {
    public List<Sprite> ThumbnailIcons;
}
