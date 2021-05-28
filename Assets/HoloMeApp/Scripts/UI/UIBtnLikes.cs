using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;
using TMPro;

public class UIBtnLikes : MonoBehaviour
{
    [SerializeField] Image imageLike;
    [SerializeField] Image imageUnlike;
    [SerializeField] TMP_Text likesCount;

    private bool isLike;
    private int count;
    private long streamId = -1;

    public void SetState(bool isLike, int count, long streamId) {
        this.isLike = isLike;
        this.count = count;
        UpdateBtnState();
    }

    public void ClickLike() {
        Handheld.Vibrate();
        isLike = !isLike;
        count = isLike ? ++count : --count;
        UpdateBtnState();
        if(isLike) {
            CallBacks.onSetLike?.Invoke(streamId);
        } else {
            CallBacks.onSetUnlike?.Invoke(streamId);
        }
    }

    private void UpdateBtnState() {
        imageLike.enabled = isLike;
        imageUnlike.enabled = !isLike;
        likesCount.text = count.ToString();
    }

}
