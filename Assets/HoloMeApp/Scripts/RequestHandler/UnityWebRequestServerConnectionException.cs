using System;

[Serializable]
public class UnityWebRequestServerConnectionException : Exception {
    public long Code { get; }

    public UnityWebRequestServerConnectionException() { }

    public UnityWebRequestServerConnectionException(string message)
        : base(message) { }

    public UnityWebRequestServerConnectionException(string message, Exception inner)
        : base(message, inner) { }

    public UnityWebRequestServerConnectionException(long code, string message)
        : this(message) {
        Code = code;
    }
}
