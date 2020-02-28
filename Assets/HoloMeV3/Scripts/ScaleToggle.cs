using UnityEngine;

public class ScaleToggle : MonoBehaviour
{
    public void ScaleUp(float factor)
    {
        transform.localScale = Vector3.one * factor;
    }

    public void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
}
