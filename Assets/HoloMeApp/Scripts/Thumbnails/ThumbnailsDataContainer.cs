using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ThumbnailsDataContainer {
    public Action OnDataUpdated;

    private ThumbnailPriority thumbnailPriority;

    StreamDataEqualityComparer streamDataEqualityComparer;

    Dictionary<long, StreamJsonData.Data> streamDataDictionary;
    List<StreamJsonData.Data> streamData;

    public ThumbnailsDataContainer(ThumbnailPriority thumbnailPriority) {
        this.thumbnailPriority = thumbnailPriority;
        streamDataDictionary = new Dictionary<long, StreamJsonData.Data>();
        streamData = new List<StreamJsonData.Data>();

        streamDataEqualityComparer = new StreamDataEqualityComparer();
    }

    public List<StreamJsonData.Data> GetDataList() {
        return streamData;
    }

    public void Refresh() {
        streamDataDictionary.Clear();
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
        if (streamDataDictionary.ContainsKey(data.id)) {
            StreamJsonData.Data prevStreamData = streamDataDictionary[data.id];

            if (!streamDataEqualityComparer.Equals(prevStreamData, data)) {
                streamData.Remove(prevStreamData);
                streamData.Add(data);
            } 
        } else {
            streamDataDictionary[data.id] = data;
            streamData.Add(data);
        }



    }

    private void SortListByStartDate() {



        streamData.Sort((elem1, elem2)
            => {

                Priority priority1 = new Priority { Stage = elem1.GetStage(), IsPin = elem1.IsPin() };
                Priority priority2 = new Priority { Stage = elem2.GetStage(), IsPin = elem2.IsPin() };

                int stageCompare = thumbnailPriority.Priorities.IndexOf(priority1)
                    .CompareTo(thumbnailPriority.Priorities.IndexOf(priority2));

                if (stageCompare != 0)
                    return stageCompare;

                return elem2.StartDate.CompareTo(elem1.StartDate);
            });


    }

    [Serializable]
    public struct Priority {
        public StreamJsonData.Data.Stage Stage;
        public bool IsPin;
    }
}
