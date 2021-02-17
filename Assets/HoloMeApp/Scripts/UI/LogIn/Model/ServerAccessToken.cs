using System;

[Serializable]
public class ServerAccessToken {
    public string refresh;
    public string access;

    public ServerAccessToken() { }

    public ServerAccessToken(string refresh, string access) {
        this.refresh = refresh;
        this.access = access;
    }
}
