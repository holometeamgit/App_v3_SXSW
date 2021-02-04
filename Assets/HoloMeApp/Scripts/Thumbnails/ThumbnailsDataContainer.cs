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
        /*streamData.GroupBy(elem => elem.GetStatus())
            .Where(grp => thumbnailPriority.Stages.Contains(grp.Key))
            .OrderBy(grp => thumbnailPriority.Stages.IndexOf(grp.Key))
            .ThenBy(e => e.Key)*/


        streamData.Sort((elem1, elem2)
            => {
                int stageCompare = thumbnailPriority.Stages.IndexOf(elem1.GetStatus())
                    .CompareTo(thumbnailPriority.Stages.IndexOf(elem2.GetStatus()));

                if (stageCompare != 0)
                    return stageCompare;

                return elem2.StartDate.CompareTo(elem1.StartDate);
            });


        //streamData.Sort((emp1, emp2) => emp2.StartDate.CompareTo(emp1.StartDate));
    }
}
