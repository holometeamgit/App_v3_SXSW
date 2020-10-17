using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThumbnailsDataContainer {
    public Action OnDataUpdated;

    StreamDataEqualityComparer streamDataEqualityComparer;

    Dictionary<long, StreamJsonData.Data> streamDataDictionary;
    List<StreamJsonData.Data> streamData;

    public ThumbnailsDataContainer() {
        streamDataDictionary = new Dictionary<long, StreamJsonData.Data>();
        streamData = new List<StreamJsonData.Data>();

        streamDataEqualityComparer = new StreamDataEqualityComparer();
    }

    public List<StreamJsonData.Data> GetDataList() {
        return streamData;
    }

    public void Refresh() {
        streamData.Clear();
    }

    public void AddListStreamJsonData(StreamJsonData newStreamData) {

        foreach (var data in newStreamData.results) {
            AddStreamJsonData(data);
        }

        SortListByStartDate();
        OnDataUpdated?.Invoke();
    }

    private void AddStreamJsonData(StreamJsonData.Data data) {

        Debug.Log(data.id + " " + data.user + " ");

        if (streamDataDictionary.ContainsKey(data.id)) {
            StreamJsonData.Data prevStreamData = streamDataDictionary[data.id];

            if (!streamDataEqualityComparer.Equals(prevStreamData, data))
                streamData.Remove(prevStreamData);
        }


        streamDataDictionary[data.id] = data;
        streamData.Add(data);
    }

    private void SortListByStartDate() {
        streamData.Sort((emp1, emp2) => emp2.StartDate.CompareTo(emp1.StartDate));
    }
}
