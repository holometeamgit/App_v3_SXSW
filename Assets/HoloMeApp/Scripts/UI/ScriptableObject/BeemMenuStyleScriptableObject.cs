﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MenuStyle", menuName = "Data/UI/MenuStyle", order = 111)]
public class BeemMenuStyleScriptableObject : ScriptableObject {
    public List<ColorStyleScriptableObject> ColorStyles;
    public List<SpriteStyleScriptableObject> Sprites;
}