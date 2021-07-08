using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Beem.UI {

    /// <summary>
    /// Panel Bottom Bar for Prerecorded Video
    /// </summary>
    public class BottomBar : MonoBehaviour {

        private List<IStreamDataView> _streamDataViews;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="streamData">Stream Json data</param>
        public void Init(StreamJsonData.Data streamData) {

            _streamDataViews = GetComponentsInChildren<IStreamDataView>().ToList();

            _streamDataViews.ForEach(x => x.Init(streamData));

            gameObject.SetActive(true);
        }
    }
}
