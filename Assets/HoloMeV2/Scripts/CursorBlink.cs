using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CursorBlink : MonoBehaviour
{
    Image imgCursor;

    private void Awake()
    {
        imgCursor = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(BlinkCursor());
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            imgCursor.enabled = !imgCursor.enabled;
            yield return new WaitForSeconds(.53f);
        }
    }
}
