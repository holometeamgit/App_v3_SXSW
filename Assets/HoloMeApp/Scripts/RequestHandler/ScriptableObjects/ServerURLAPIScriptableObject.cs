using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerURLAPI", menuName = "Data/API/ServerURLAPI", order = 100)]
public class ServerURLAPIScriptableObject : ScriptableObject
{
    public string ServerURLAuthAPI = "https://devholo.me/api-auth";
    public string ServerURLMediaAPI = "https://devholo.me/api-media";
}
