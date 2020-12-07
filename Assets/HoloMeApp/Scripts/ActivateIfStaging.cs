using UnityEngine;

public class ActivateIfStaging : MonoBehaviour
{
    [SerializeField]
    GameObject objectToActivate;

    private void Awake()
    {
#if DEV
        objectToActivate.SetActive(true);
#else
        objectToActivate.SetActive(false);
#endif
    }

}
