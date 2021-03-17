using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TaggableSprite : ITaggable<string>
{
    public string Key => tag;
    public Sprite Sprite => sprite;

    [SerializeField]
    string tag;

    [SerializeField]
    Sprite sprite;
}
