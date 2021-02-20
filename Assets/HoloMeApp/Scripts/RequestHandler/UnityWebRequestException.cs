using System;

[Serializable]
public class UnityWebRequestException : Exception {
    public long Code { get; }

    public UnityWebRequestException() { }

    public UnityWebRequestException(string message)
        : base(message) { }

    public UnityWebRequestException(string message, Exception inner)
        : base(message, inner) { }

    public UnityWebRequestException(long code, string message)
        : this(message) {
        Code = code;
    }
}
