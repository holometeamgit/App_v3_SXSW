using System;

public class LoadingKey {
    DateTime dateTime;
    object obj;

    public LoadingKey(object obj) {
        this.obj = obj;
        dateTime = DateTime.Now;
    }

    public static bool operator ==(LoadingKey lk1, LoadingKey lk2) {
        return lk1.dateTime == lk2.dateTime && lk1.obj == lk2.obj;
    }

    public static bool operator !=(LoadingKey lk1, LoadingKey lk2) {
        return lk1.dateTime != lk2.dateTime && lk1.obj != lk2.obj;
    }
}
