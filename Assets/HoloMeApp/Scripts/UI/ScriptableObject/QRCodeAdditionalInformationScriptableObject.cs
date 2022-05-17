using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// QRCode Additional Information ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "QRCode", menuName = "Data/QRCode", order = 131)]
public class QRCodeAdditionalInformationScriptableObject : ScriptableObject
{
    public Texture2D LogoSprite;
    public Texture2D BoardingSprite;
    public string DefaultLink = "https://www.beem.me/";
}
