using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamData {
    /// <summary>
    /// InitStreamData
    /// </summary>
    /// <param name="streamData"></param>
    void Init(StreamJsonData.Data streamData);
}
