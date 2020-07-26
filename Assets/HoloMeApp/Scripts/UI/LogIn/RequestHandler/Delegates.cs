using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ResponseDelegate(long code, string body);
public delegate void ResponseTextureDelegate(long code, string body, Texture texture);
public delegate void ErrorTypeDelegate(long code, string body);
