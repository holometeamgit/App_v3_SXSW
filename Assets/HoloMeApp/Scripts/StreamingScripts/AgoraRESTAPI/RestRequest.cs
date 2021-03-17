using System;

public abstract class RestRequest
{
    public Action OnSuccessAction;
    public Action OnFailedAction;

    protected string requestString;
    public string RequestString { get { return requestString; } }

    public virtual void OnSuccess(string result)
    {
        OnSuccessAction?.Invoke();
    }

    public virtual void OnFailed()
    {
        OnFailedAction?.Invoke();
    }

    protected T OnResponseReturned<T>(string jsonText)
    {
        HelperFunctions.DevLog("JSON Returned = " + jsonText);
        var returnType =  JsonParser.CreateFromJSON<T>(jsonText);

        if (returnType == null)
            HelperFunctions.DevLogError("ReturnType couldn't be parsed");

        return returnType;
    }

}
