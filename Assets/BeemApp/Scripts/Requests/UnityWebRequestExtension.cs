using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace Beem.Utility.Requests {
    public static class UnityWebRequestExtension {
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp) {
            TaskCompletionSource<UnityWebRequest.Result> tsc = new TaskCompletionSource<UnityWebRequest.Result>();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

            if (reqOp.isDone)
                tsc.TrySetResult(reqOp.webRequest.result);

            return tsc.Task.GetAwaiter();
        }
    }
}