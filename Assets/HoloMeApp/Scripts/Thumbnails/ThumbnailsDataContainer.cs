using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThumbnailsDataContainer {
    public Action<long> OnStreamUpdated;
    public Action<long> OnStreamRemoved;
    public Action OnDataUpdated;

    Dictionary<long, StreamJsonData.Data> streamDataDictionary;
    List<StreamJsonData.Data> streamData;

    private int lastPaginationAddedPosition;
    private int lastAddedPosition = 0; //need for understand that you need to delete the data below the pagination
    private int countAddedStreamData = 0;

    StreamDataEqualityComparer streamDataEqualityComparer;

    public ThumbnailsDataContainer() {
        streamDataDictionary = new Dictionary<long, StreamJsonData.Data>();
        streamData = new List<StreamJsonData.Data>();

        streamDataEqualityComparer = new StreamDataEqualityComparer();
    }

    public List<StreamJsonData.Data> GetDataList() {
        return streamData;
    }

    public void Refresh() {
        countAddedStreamData = 0;
    }

    public void AddListStreamJsonData(StreamJsonData newStreamData) {

        lastPaginationAddedPosition = -1;

        foreach (var data in newStreamData.results) {
            AddStreamJsonData(data);
        }

        if (newStreamData == null || newStreamData.results.Count == 0)
            RemoveTail();

        OnDataUpdated?.Invoke();
    }

    private void AddStreamJsonData(StreamJsonData.Data data) {
        int index = FindAddingPosition(data); 
        Debug.Log("add to index " + index);
        countAddedStreamData++;

        //add or update data
        streamData.Insert(index, data);
        StreamJsonData.Data prevStreamData = null;
        if (streamDataDictionary.ContainsKey(data.id)) {
            prevStreamData = streamDataDictionary[data.id];
        }

        streamDataDictionary[data.id] = data;

        if (!streamDataEqualityComparer.Equals(prevStreamData, data))
            OnStreamUpdated?.Invoke(data.id);

        //remove irrelevant data
        if (lastPaginationAddedPosition < 0)
            lastPaginationAddedPosition = index;
        else {
            RemoveIrrelevantData(index);
        }

        lastAddedPosition = lastPaginationAddedPosition;
    }

    private int FindAddingPosition(StreamJsonData.Data data) {
        Debug.Log(data.id + " " + data.user + " " + data.status + " " + data.StartDate);// + " " + data.EndDate);

        //TODO optimize if necessary using binary search
        for (int i = 0; i < streamData.Count; i++) {
            if (data.StartDate > streamData[i].StartDate)
                return i;
        }

        return streamData.Count;
    }

    private void RemoveIrrelevantData(int to) {
        //if (to - lastPaginationAddedPosition < 1) 
        //    return;
        //else
        if (to - lastPaginationAddedPosition <= 1) {
            lastPaginationAddedPosition = to;
            return;
        }

        List<long> idRemovedStreamData = new List<long>();
        int removeCount = streamData.Count - 1 - lastAddedPosition - 1;

        for (int i = lastPaginationAddedPosition + 1; i < to; i++) {
            Debug.Log("Remove" + streamData[i].StartDate);
            streamDataDictionary.Remove(streamData[i].id);
            idRemovedStreamData.Add(streamData[i].id);
        }

        streamData.RemoveRange(lastPaginationAddedPosition + 1, removeCount);

        lastPaginationAddedPosition++;

        foreach (var id in idRemovedStreamData)
            OnStreamRemoved?.Invoke(id);
    }

    private void RemoveTail() {
        List<long> idRemovedStreamData = new List<long>();
        int removeCount = streamData.Count - countAddedStreamData;
        if (removeCount <= 0)
            return;

        for (int i = countAddedStreamData; i < streamData.Count; i++) {
            Debug.Log("Remove tail" + streamData[i].StartDate);
            streamDataDictionary.Remove(streamData[i].id);
            idRemovedStreamData.Add(streamData[i].id);
        }

        streamData.RemoveRange(countAddedStreamData, removeCount);


        foreach (var id in idRemovedStreamData)
            OnStreamRemoved?.Invoke(id);
    }
}
