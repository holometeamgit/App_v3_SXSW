using UnityEngine;

public class PnlWelcome : MonoBehaviour
{
    [SerializeField]
    AnimatedTransition animatedTransition;

    [SerializeField]
    PnlMainPage pnlMainPage;

    const string PrefHasSeenWelcome = nameof(PrefHasSeenWelcome);

    private void OnEnable()
    {
        PlayerPrefs.DeleteAll();
        int HasSeen = PlayerPrefs.GetInt(PrefHasSeenWelcome, 0);
        if (HasSeen == 1)
        {
            ShowNextPanel();
        }
    }

    private void ShowNextPanel()
    {
        pnlMainPage.gameObject.SetActive(true);
        animatedTransition.DoMenuTransition(true);
        gameObject.SetActive(false);
    }

    //Link to close button
    public void SetToSeen()
    {
        PlayerPrefs.SetInt(PrefHasSeenWelcome, 1);
        ShowNextPanel();
    }
}
