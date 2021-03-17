using System;

[Serializable]
public class WebExceptionJsonData { 
    public long ResponseCode;
    public string Msg;

    public WebExceptionJsonData() { }

    public WebExceptionJsonData(long responseCode, string msg) {
        ResponseCode = responseCode;
        Msg = msg;
    }
}