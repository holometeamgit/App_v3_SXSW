using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlFilmingGuidelines : MonoBehaviour
{
    [SerializeField]
    PnlChannelName pnlChannelName;

    [SerializeField]
    GameObject btnConfirm;

    private void OnEnable() {
        btnConfirm.SetActive(!pnlChannelName.IsConfirmFilmingGuidelines());
    }
}
