/*
 * contains parameters for request from the server
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ThumbnailsFilter 
{
    private Dictionary<string, string> filterParameters;

    public ThumbnailsFilter() {
        filterParameters = new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetParameters() {
        return filterParameters;
    }

    public string GetParametersString() {
        string parameters = string.Join("&", filterParameters.Select(x => x.Key + "=" + x.Value).ToArray());
        return parameters;
    }

    public void Add(string key, string value) {
        if(!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(key))
            filterParameters[key] = value;
    }

    public void Remove(string key) {
        if (filterParameters.ContainsKey(key))
            filterParameters.Remove(key);
    }

    public bool IsEmpty() {
        return filterParameters.Count == 0;
    }
}
