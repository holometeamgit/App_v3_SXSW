using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TaggableColor : ITaggable<string> {

    public string Key => tag;
    public Color Color => color;

    [SerializeField]
    string tag;

    [SerializeField]
    [ColorUsage(true)]
    Color color;
}

