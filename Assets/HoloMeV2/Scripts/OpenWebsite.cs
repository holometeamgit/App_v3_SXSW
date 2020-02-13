using UnityEngine;

public class OpenWebsite : MonoBehaviour
{
    public void OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }
}
