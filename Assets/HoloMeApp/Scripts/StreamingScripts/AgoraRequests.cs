using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AgoraRequests : MonoBehaviour
{
    const string baseURL = "https://api.agora.io/dev/";
    const string customerID = "cca351a8559f4da688319b23865d5e78";
    const string certificate = "52dd04cdb705400b97a5d9dec0a4cb51";
    const string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    IEnumerator GetRequest(string uri, Action<string> OnSuccess, Action OnFailed)
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
                HelperFunctions.DevLog(pages[page] + ": Error: " + webRequest.error);
                OnFailed?.Invoke();
            }
            else
            {
                HelperFunctions.DevLog(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text + webRequest.responseCode);
                OnSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    public void RequestChannels(RestRequest restRequest)
    {
        print(restRequest.RequestString);
        StartCoroutine(GetRequest(baseURL + restRequest.RequestString + appId, restRequest.OnSuccess, restRequest.OnFailed));
    }

    //void RequestChannels()
    //{
    //    StartCoroutine(GetRequest(baseURL + "v1/channel/" + appId, OnChannelsReturned));
    //}

    //void OnChannelsReturned(string jsonText)
    //{
    //    var response = JsonParser.CreateFromJSON<ChannelResponse>(jsonText);
    //}


}
