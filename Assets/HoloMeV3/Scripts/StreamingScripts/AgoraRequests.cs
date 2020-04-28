using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AgoraRequests : MonoBehaviour
{
    string baseURL = "https://api.agora.io/dev?_ga=2.248837760.647778257.1587586617-241556992.1585304514";
    //string baseURL = "https://api.agora.io/dev?_ga=2.39590436.647778257.1587586617-241556992.1585304514";
    //string baseURL = "https://api.agora.io/dev/v1?_ga=2.210040338.647778257.1587586617-241556992.1585304514";
    //string baseURL = "https://api.agora.io/dev/";

    //below are accessed from top right click on username then restful api
    string customerID = "cca351a8559f4da688319b23865d5e78";
    string certificate = "52dd04cdb705400b97a5d9dec0a4cb51";
    string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    private void Awake()
    {
        //StartCoroutine(GetChannelList());
        //StartCoroutine(GetRequest("https://api.agora.io/dev/v1?_ga=2.9181618.647778257.1587586617-241556992.1585304514/projects/"));
        StartCoroutine(GetRequest("https://api.agora.io/dev/v1/channel/" + appId));
    }

    //IEnumerator GetChannelList()
    //{
    //    string finalURL = "https://api.agora.io/dev/v1/projects/";
    //    //string finalURL = baseURL + "/projects/";
    //    //string finalURL = baseURL + "/channel/de596f86fdde42e8a7f7a39b15ad3c82";
    //    //string finalURL = baseURL + "/v1/channel/de596f86fdde42e8a7f7a39b15ad3c82";
    //    //string finalURL = baseURL + "/v1/channel/{de596f86fdde42e8a7f7a39b15ad3c82}";

    //    UnityWebRequest channelRequest = UnityWebRequest.Get(finalURL);
    //    string authorizationDetails = customerID + ":" + certificate;
    //    string encodedAuthDetails = Base64Encode(authorizationDetails);
    //    channelRequest.SetRequestHeader("Authorization", encodedAuthDetails);

    //    yield return channelRequest.SendWebRequest();

    //    if (channelRequest.isNetworkError || channelRequest.isHttpError)
    //    {
    //        Debug.LogError(channelRequest.error);
    //        yield break;
    //    }

    //    Debug.Log("JSON RECEIVED " + channelRequest.downloadHandler.text);
    //}

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
    IEnumerator GetRequest(string uri)
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
            }
        }
    }

}
