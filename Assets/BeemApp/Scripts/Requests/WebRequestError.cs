using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WebRequest Error
/// </summary>
public class WebRequestError {

    private long _code = 404;

    public long Code {
        get {
            return _code;
        }
    }

    private string _detail = "Not Found";

    public string Detail {
        get {
            return _detail;
        }
    }

    public WebRequestError(long code, string body) {
        _code = code;
        ErrorBody data = JsonUtility.FromJson<ErrorBody>(body);
        _detail = data.detail;
        HelperFunctions.DevLogError($"Error: Code - {Code}, Detail - {Detail}");
    }

    public WebRequestError() {
        HelperFunctions.DevLogError($"Error: Code - {Code}, Detail - {Detail}");
    }

    public class ErrorBody {
        public string detail;
    }
}
