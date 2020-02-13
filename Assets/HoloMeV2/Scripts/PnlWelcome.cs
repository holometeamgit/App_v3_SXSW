using UnityEngine;

public class PnlWelcome : MonoBehaviour
{
    [SerializeField]
    AnimatedTransition animatedTransition;

    [SerializeField]
    PnlVideoCode pnlVideoCode;

    const string PrefHasSeenWelcome = nameof(PrefHasSeenWelcome);

    private void OnEnable()
    {
        int HasSeen = PlayerPrefs.GetInt(PrefHasSeenWelcome, 0);
        if (HasSeen == 1)
        {
            animatedTransition.enabled = false;
            pnlVideoCode.Open();
            gameObject.SetActive(false);
        }
    }

    //Link to close button
    public void SetToSeen()
    {
        PlayerPrefs.SetInt(PrefHasSeenWelcome, 1);
    }
}
