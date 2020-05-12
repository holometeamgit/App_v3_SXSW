public class RequestChannelList : RestRequest
{
    ChannelResponse channelResponse;
    //protected new string requestString = "v1/channel/";

    public RequestChannelList()
    {
        requestString = "v1/channel/";
    }

    public override void OnSuccess(string result)
    {
        channelResponse = OnResponseReturned<ChannelResponse>(result);
        base.OnSuccess(result);
    }

    public bool DoesChannelExist(string channelName)
    {
        if (channelResponse.success)
        {
            if (channelResponse.data.channels.Length > 0)
            {
                foreach (var channel in channelResponse.data.channels)
                {
                    if (channel.channel_name == channelName)
                    {
                        return true;
                    }
                }
            }
            else
                return false;
        }

        return false;
    }

    [System.Serializable]
    public class ChannelResponse
    {
        public bool success;
        public ChannelDataStore data;
    }

    [System.Serializable]
    public class ChannelDataStore
    {
        public ChannelData[] channels;
        public int total_size;
    }

    [System.Serializable]
    public class ChannelData
    {
        public string channel_name;
        public int user_count;
    }
}
