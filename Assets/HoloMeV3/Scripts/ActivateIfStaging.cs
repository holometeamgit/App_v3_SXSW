using UnityEngine;

public class ActivateIfStaging : MonoBehaviour
{
    [SerializeField]
    GameObject objectToActivate;

    private void Awake()
    {
#if STAGING
        objectToActivate.SetActive(true);
#else
        objectToActivate.SetActive(false);
#endif
    }

}
