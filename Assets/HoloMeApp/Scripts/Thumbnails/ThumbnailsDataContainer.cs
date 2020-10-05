using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThumbnailsDataContainer {
    public Action<long> OnStreamUpdated;
    public Action<long> OnStreamRemoved;

    Dictionary<long, StreamJsonData.Data> streamDataDictionary;
    StreamJsonData streamData;

    private int lastPaginationAddedPosition;
    private int lastAddedPosition = 0; //need for need to understand that you need to delete the data below the pagination
    private int countAddedStreamData = 0;

    StreamDataEqualityComparer streamDataEqualityComparer;

    public ThumbnailsDataContainer() {
        streamDataDictionary = new Dictionary<long, StreamJsonData.Data>();
        streamData = new StreamJsonData();

        streamDataEqualityComparer = new StreamDataEqualityComparer();
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
    }

    private void AddStreamJsonData(StreamJsonData.Data data) {
        int index = FindAddingPosition(data); 
        Debug.Log(index);
        countAddedStreamData++;

        //add or update data
        streamData.results.Insert(index, data);
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
        Debug.Log("data.start_date " + data.user + " " + data.StartDate + " " + data.EndDate);

        //TODO optimize if necessary using binary search
        for (int i = 0; i < streamData.results.Count; i++) {
            if (data.StartDate > streamData.results[i].StartDate)
                return i;
        }

        return streamData.results.Count;
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
        int removeCount = streamData.results.Count - 1 - lastAddedPosition - 1;

        for (int i = lastPaginationAddedPosition + 1; i < to; i++) {
            Debug.Log("Remove" + streamData.results[i].StartDate);
            streamDataDictionary.Remove(streamData.results[i].id);
            idRemovedStreamData.Add(streamData.results[i].id);
        }

        streamData.results.RemoveRange(lastPaginationAddedPosition + 1, removeCount);

        lastPaginationAddedPosition++;

        foreach (var id in idRemovedStreamData)
            OnStreamRemoved?.Invoke(id);
    }

    private void RemoveTail() {
        List<long> idRemovedStreamData = new List<long>();
        int removeCount = streamData.results.Count - countAddedStreamData;
        if (removeCount <= 0)
            return;

        for (int i = countAddedStreamData; i < streamData.results.Count; i++) {
            Debug.Log("Remove tail" + streamData.results[i].StartDate);
            streamDataDictionary.Remove(streamData.results[i].id);
            idRemovedStreamData.Add(streamData.results[i].id);
        }

        streamData.results.RemoveRange(countAddedStreamData, removeCount);


        foreach (var id in idRemovedStreamData)
            OnStreamRemoved?.Invoke(id);
    }
}
