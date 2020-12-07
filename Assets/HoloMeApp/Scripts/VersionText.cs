using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtVersion;

    void Start()
    {
        txtVersion.text = "V" + Application.version;

#if DEV
        txtVersion.text = "Dev " + txtVersion.text;
#endif
    }
}
