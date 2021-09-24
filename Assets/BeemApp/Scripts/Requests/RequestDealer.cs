using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Beem.Utility.Requests {
    /// <summary>
    /// Async Request Dealer
    /// </summary>
    public class RequestDealer : IRequestDealer {

        public async void Send(UnityWebRequest unityWebRequest, Action<string> Success = null, Action<string> Fail = null, Action<float> Progress = null) {
            var operation = unityWebRequest.SendWebRequest();

            while (!operation.isDone) {
                Progress?.Invoke(operation.progress);
                await Task.Yield();
            }

            if (unityWebRequest.result == UnityWebRequest.Result.Success) {
                Debug.Log($"Success: {unityWebRequest.downloadHandler.text}");
                Success?.Invoke(unityWebRequest.downloadHandler.text);
            } else {
                Debug.LogError($"Failed: {unityWebRequest.error}");
                Fail?.Invoke(unityWebRequest.error);
            }
        }
    }

}
