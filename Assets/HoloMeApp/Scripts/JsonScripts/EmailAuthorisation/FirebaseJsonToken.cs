using System;

[Serializable]
public class FirebaseJsonToken 
{
    public string firebase_token;

    public FirebaseJsonToken() { }

    public FirebaseJsonToken(string firebase_token) {
        this.firebase_token = firebase_token;
    }
}
