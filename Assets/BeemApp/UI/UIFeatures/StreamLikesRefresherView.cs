using Beem;
using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.UI {

    /// <summary>
    /// Stream likes refresher
    /// </summary>
    public class StreamLikesRefresherView : AbstractStreamRefresherView {

        [Header("Button for Likes")]
        [SerializeField]
        private UIBtnLikes _uIBtnLikes;

        private const int REFRESH_DELAY_FOR_LIKES = 10;

        protected override int delay => REFRESH_DELAY_FOR_LIKES;

        public override void Refresh(string streamID) {
            _uIBtnLikes.SetStreamId(long.Parse(streamID));
        }
    }
}
