using UnityEngine;

public class VideoJsonData
{
    public float positionOffsetX;
    public float positionOffsetY;
    public float positionOffsetZ;
    public string hyperlink;
    public string videoCode;
    public string logoImage;

    public Vector3 GetOffsetVector()
    {
        return new Vector3(positionOffsetX, positionOffsetY, positionOffsetZ);
    }
}
