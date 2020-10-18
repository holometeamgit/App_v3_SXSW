using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamDataEqualityComparer : IEqualityComparer<StreamJsonData.Data>
{
    public bool Equals(StreamJsonData.Data x, StreamJsonData.Data y) {
        if (x == null && y == null)
            return true;
        else if (x == null || y == null)
            return false;

        return x.id == y.id &&
            x.preview_s3_url == y.preview_s3_url &&
            x.stream_s3_url == y.stream_s3_url &&
            x.user == y.user &&
            x.status == y.status &&
            x.agora_sid == y.agora_sid &&
            x.agora_channel == y.agora_channel &&
            x.file_name_prefix == y.file_name_prefix &&
            x.start_date == y.start_date &&
            x.end_date == y.end_date &&
            x.duration == y.duration &&
            x.preview_s3_key == y.preview_s3_key &&
            x.stream_s3_key == y.stream_s3_key &&
            x.title == y.title &&
            x.description == y.description;
    }

    public int GetHashCode(StreamJsonData.Data obj) {
        return (obj.user + obj.StartDate + obj.status + obj.preview_s3_url).GetHashCode();
    }
}
