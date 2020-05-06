using UnityEngine;

public class PnlWelcome : MonoBehaviour
{
    [SerializeField]
    AnimatedTransition animatedTransition;

    [SerializeField]
    PnlMainPage pnlMainPage;

    const string PrefHasSeenWelcome = nameof(PrefHasSeenWelcome);

    public void CheckIfSeen()
    {
        //PlayerPrefs.DeleteAll();
        int HasSeen = PlayerPrefs.GetInt(PrefHasSeenWelcome, 0);
        if (HasSeen == 1)
        {
            gameObject.GetComponent<AnimatedTransition>().enabled = false;
            gameObject.SetActive(true);
            Invoke(nameof(ShowNextPanel), .5f);
        }
        else
        {
            gameObject.SetActive(true);
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
