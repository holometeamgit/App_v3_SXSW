using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AsyncWebRequest : MonoBehaviour {


    private const string API_KEY = "6c9ddff22a920c8e97a8b2449a9b366b";
    private const string SERVER = "https://build-api.cloud.unity3d.com/api/v1";
    private const string ORG_ID = "10170749378847";
    private const string PROJECT_ID = "ba3333ea-4845-4d5b-a4c1-896277975428";
    private const string BUILD_TARGET_ID = "firebaseapkdev-test-manifest";

    [SerializeField]
    private string url = $"{SERVER}/orgs/{ORG_ID}/projects/{PROJECT_ID}/buildtargets/{BUILD_TARGET_ID}/envvars";

    [SerializeField]
    private string release_notes = "Dev build";


    [ContextMenu("Test Get")]
    public async void TestGet() {

        using var www = UnityWebRequest.Get(url);

        www.SetRequestHeader("Content-Type", "application/json");

        www.SetRequestHeader("Authorization", "Basic " + API_KEY);

        var operation = www.SendWebRequest();

        while (!operation.isDone) {
            await Task.Yield();
        }

        if (www.result == UnityWebRequest.Result.Success) {
            Debug.Log($"Success: {www.downloadHandler.text}");
            FirebaseEnviromentVariables firebaseEnviromentVariables = JsonUtility.FromJson<FirebaseEnviromentVariables>(www.downloadHandler.text);
            firebaseEnviromentVariables.FIREBASE_RELEASE_NOTES = release_notes;
            string body = JsonUtility.ToJson(firebaseEnviromentVariables);
            TestPut(body);
        } else {
            Debug.Log($"Failed: {www.error}");
        }
    }

    private void ChangeReleaseNotes() {

    }

    public class FirebaseEnviromentVariables {
        public string BEEM_BUILD_TYPE;
        public string FIREBASE_GROUPS;
        public string FIREBASE_APP;
        public string FIREBASE_TOKEN;
        public string FIREBASE_RELEASE_NOTES;
        public string BEEM_VERSION;
        public string BEEM_BUILD;
    }

    public async void TestPut(string body) {

        byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

        using var www = UnityWebRequest.Put(url, bodyRaw);

        www.SetRequestHeader("Content-Type", "application/json");

        www.SetRequestHeader("Authorization", "Basic " + API_KEY);

        var operation = www.SendWebRequest();

        while (!operation.isDone) {
            await Task.Yield();
        }

        if (www.result == UnityWebRequest.Result.Success) {
            Debug.Log($"Success: {www.downloadHandler.text}");

        } else {
            Debug.Log($"Failed: {www.error}");
        }
    }
}
