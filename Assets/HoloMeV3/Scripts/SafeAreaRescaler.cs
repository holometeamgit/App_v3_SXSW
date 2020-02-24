using UnityEngine;

public class SafeAreaRescaler : MonoBehaviour
{
    void Start()
    {
        Rect area = Screen.safeArea;
        RectTransform rect = GetComponent<RectTransform>();
        //rect.anchorMin = new Vector2(area.xMin, area.yMin);
        //rect.anchorMax = new Vector2(area.xMax, area.yMax);

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(area.width, area.height);
    }
}
