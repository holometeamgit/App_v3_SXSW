using UnityEngine;

public class SafeAreaRescaler : MonoBehaviour
{
    void Start()
    {
        Rect area = Screen.safeArea;
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(area.xMin, area.yMin);
        rect.anchorMin = new Vector2(area.xMax, area.yMax);
    }
}
