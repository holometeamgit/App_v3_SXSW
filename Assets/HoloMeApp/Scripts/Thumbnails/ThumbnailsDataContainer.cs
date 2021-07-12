using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Beem.SSO;

public class ThumbnailsDataContainer {

    private ThumbnailPriority thumbnailPriority;

    Dictionary<long, StreamJsonData.Data> streamDataDictionary;
    List<StreamJsonData.Data> streamData;

    public ThumbnailsDataContainer(ThumbnailPriority thumbnailPriority) {
        this.thumbnailPriority = thumbnailPriority;
        streamDataDictionary = new Dictionary<long, StreamJsonData.Data>();
        streamData = new List<StreamJsonData.Data>();
        CallBacks.onGetLikeState += GetLikeState;
    }

    public List<StreamJsonData.Data> GetDataList() {
        return streamData;
    }

    public void Clear() {
        streamDataDictionary.Clear();
        streamData.Clear();
    }

    public bool ContainStream(long id) {
        return streamDataDictionary.ContainsKey(id);
    }

    public void AddListStreamJsonData(StreamJsonData newStreamData) {

        foreach (var data in newStreamData.results) {
            AddStreamJsonData(data);
        }

        SortListByStartDate();
        CallBacks.onStreamsContainerUpdated?.Invoke();
    }

    private void AddStreamJsonData(StreamJsonData.Data data) {
        if (ContainStream(data.id)) {
            StreamJsonData.Data prevStreamData = streamDataDictionary[data.id];
            //if (data.id == 796)

            if (!ObjectComparer.Equals(prevStreamData, data)) {
                prevStreamData.Update(data);
                CallBacks.onStreamByIdInContainerUpdated?.Invoke(data.id);
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

    private void GetLikeState(long streamId) {
        if (!ContainStream(streamId))
            return;


        var streamData = streamDataDictionary[streamId];

        CallBacks.onGetLikeStateCallBack?.Invoke(streamId, streamData.is_liked, streamData.count_of_likes);
    }

    ~ThumbnailsDataContainer() {
        CallBacks.onGetLikeState -= GetLikeState;
    }

[Serializable]
    public struct Priority {
        public StreamJsonData.Data.Stage Stage;
        public bool IsPin;
    }
}
