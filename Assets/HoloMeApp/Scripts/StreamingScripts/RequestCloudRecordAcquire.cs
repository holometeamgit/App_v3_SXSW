using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestCloudRecordAcquire : RestRequest {

    public RequestCloudRecordAcquire() {
        requestString = $"v1/apps/{AgoraController.AppId}/cloud_recording/acquire";
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
