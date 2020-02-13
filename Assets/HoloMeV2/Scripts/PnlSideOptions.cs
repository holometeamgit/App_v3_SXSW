using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PnlSideOptions : MonoBehaviour
{
    [SerializeField]
    RectTransform rTHistoryCodesContent;

    [SerializeField]
    GameObject btnHistoryCodePrefab;

    [SerializeField]
    BlurController blurController;

    [SerializeField]
    PnlVideoCode pnlVideoCode;

    List<GameObject> codeButtonPool = new List<GameObject>();

    public void OnEnable()
    {
        blurController.AddBlur();
        CreateHistoryCodes();
    }

    #region TestFunctions
    public void AddCode()
    {
        var randomNum = "" + Random.Range(0, 10) + Random.Range(0, 10) + Random.Range(0, 10) + Random.Range(0, 10);
        PrefsCodeHistory.UpdatePrefsString(randomNum);
        print("Random code generated " + randomNum);
        CreateHistoryCodes();
    }

    public void RemoveFirstCode()
    {
        string[] savedCodes = PrefsCodeHistory.GetSavedCodes();
        if (savedCodes.Length >= 1)
        {
            PrefsCodeHistory.RemoveCode(savedCodes[0]);
        }
        CreateHistoryCodes();
    }

    public void DeleteCodes()
    {
        PlayerPrefs.DeleteAll();
        CreateHistoryCodes();
    }
    #endregion

    public void CreateHistoryCodes()
    {
        string[] savedCodes = PrefsCodeHistory.GetSavedCodes();

        //print(PlayerPrefs.GetString("PrefCodeMemory", ""));

        if (codeButtonPool.Count < savedCodes.Length)
        {
            for (int i = 0; i < savedCodes.Length; i++)
            {
                if (codeButtonPool.Count < savedCodes.Length)
                {
                    codeButtonPool.Add(Instantiate(btnHistoryCodePrefab, rTHistoryCodesContent, false));
                }
            }
        }

        for (int i = 0; i < codeButtonPool.Count; i++)
        {
            if (i > savedCodes.Length - 1)
            {
                codeButtonPool[i].SetActive(false);
            }
            else
            {
                string code = savedCodes[i];

                codeButtonPool[i].SetActive(true);
                codeButtonPool[i].GetComponentInChildren<TextMeshProUGUI>().text = code;
                codeButtonPool[i].GetComponent<Button>().onClick.RemoveAllListeners();
                codeButtonPool[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    GetComponent<AnimatedTransition>().DoMenuTransition(false);
                    pnlVideoCode.OpenWithCode(code);
                });

                codeButtonPool[i].transform.Find("imgOfflineAvailable").gameObject.SetActive(HelperFunctions.DoesFileExist(code));
            }
        }
    }

}
