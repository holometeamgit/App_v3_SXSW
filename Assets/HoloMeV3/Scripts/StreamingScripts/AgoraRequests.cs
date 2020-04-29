using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AgoraRequests : MonoBehaviour
{
    string baseURL = "https://api.agora.io/dev/";

    //below are accessed from top right click on username then restful api
    string customerID = "cca351a8559f4da688319b23865d5e78";
    string certificate = "52dd04cdb705400b97a5d9dec0a4cb51";
    string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    private void Awake()
    {
        RequestChannels();
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    IEnumerator GetRequest(string uri, Action<string> OnSuccess)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            string authorizationDetails = customerID + ":" + certificate;
            string encodedAuthDetails = Base64Encode(authorizationDetails);
            webRequest.SetRequestHeader("Authorization", "Basic " + encodedAuthDetails);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text + webRequest.responseCode);
                OnSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    void RequestChannels()
    {
        StartCoroutine(GetRequest(baseURL + "v1/channel/" + appId, OnChannelsReturned));
    }

    void OnChannelsReturned(string jsonText)
    {
        var response = JsonParser.CreateFromJSON<ChannelResponse>(jsonText);
    }

    [System.Serializable]
    public class ChannelResponse
    {
        public bool success;
        public ChannelDataStore data;
    }

    [System.Serializable]
    public class ChannelDataStore
    {
        public ChannelData[] channels;
        public int total_size;
    }

    [System.Serializable]
    public struct ChannelData
    {
        public string channel_name;
        public int user_count;
    }
}
