using Beem;
using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

namespace Beem.UI {

    /// <summary>
    /// Stream likes refresher
    /// </summary>
    public class StreamLikesRefresherView : AbstractStreamRefresherView {

        [Header("Button for Likes")]
        [SerializeField]
        private UIBtnLikes _uIBtnLikes;

        private const int REFRESH_DELAY_FOR_LIKES = 10000;

        protected override int delay => REFRESH_DELAY_FOR_LIKES;

        public override void Refresh(string streamID) {
            long streamIdLong = 0;
            long.TryParse(streamID, out streamIdLong);

            if (streamIdLong == 0)
                return;
            CallBacks.onDownloadStreamById(long.Parse(streamID));
        }
    }
}
