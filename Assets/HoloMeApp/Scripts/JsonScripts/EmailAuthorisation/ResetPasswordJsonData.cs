using System;

[Serializable]
public class ResetPasswordJsonData
{
    public string new_password1;
    public string new_password2;
    public string uid;
    public string token;


    public ResetPasswordJsonData() { }

    public ResetPasswordJsonData(string new_password1, string new_password2, string uid, string token) {
        this.uid = uid;
        this.token = token;
        this.new_password1 = new_password1;
        this.new_password2 = new_password2;
    }
}
