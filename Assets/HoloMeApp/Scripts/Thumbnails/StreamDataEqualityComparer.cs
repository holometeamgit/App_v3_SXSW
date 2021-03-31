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
            x.preview_teaser_s3_url == y.preview_teaser_s3_url &&
            x.stream_s3_url == y.stream_s3_url &&
            x.teaser_s3_url == y.teaser_s3_url &&
            x.user == y.user &&
            x.paid_type == y.paid_type &&
            x.is_bought == y.is_bought &&

            x.product_type.name == y.product_type.name &&
            x.product_type.product_id == y.product_type.product_id &&
            x.product_type.price == y.product_type.price &&

            x.status == y.status &&
            x.is_pin == y.is_pin &&
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
