using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySuccessWebRequestData<T> {
    public long Code;
    public T Data;

    public UnitySuccessWebRequestData() { }

    public UnitySuccessWebRequestData(long code, T data) {
        Code = code;
        Data = data;
    }

}
