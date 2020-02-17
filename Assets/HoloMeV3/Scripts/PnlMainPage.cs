using UnityEngine;

public class PnlMainPage : MonoBehaviour
{
    [SerializeField]
    PnlFetchingData pnlFetchingData;

    bool hasFetchedData;

    private void OnEnable()
    {
        if (!hasFetchedData)
        {
            pnlFetchingData.gameObject.SetActive(true);
            hasFetchedData = true;
        }
    }

    void Update()
    {

    }
}
