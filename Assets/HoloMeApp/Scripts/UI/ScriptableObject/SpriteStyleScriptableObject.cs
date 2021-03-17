using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteStyleList", menuName = "Data/UI/SpriteStyleList", order = 113)]
public class SpriteStyleScriptableObject : StyleScriptableObject {
    public List<TaggableSprite> Sprites;
}
