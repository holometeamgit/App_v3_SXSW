using Beem;
using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamLikesRefresherView : AbstractStreamRefresherView {

    [SerializeField]
    private UIBtnLikes _uIBtnLikes;

    private const int REFRESH_DELAY_FOR_LIKES = 10;

    protected override int refreshTimer => REFRESH_DELAY_FOR_LIKES;

    public override void Refresh(string streamID) {
        _uIBtnLikes.SetStreamId(long.Parse(streamID));
    }
}
