using Beem.Firebase.DynamicLink;
using Beem.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel Bottom Bar for Prerecorded Video
/// </summary>
public class BottomBar : MonoBehaviour {

    private List<IStreamData> streamDatas;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamID"></param>
    public void Init(StreamJsonData.Data streamData) {

        streamDatas = GetComponentsInChildren<IStreamData>().ToList();

        streamDatas.ForEach(x => x.Init(streamData));

        gameObject.SetActive(true);
    }
}
