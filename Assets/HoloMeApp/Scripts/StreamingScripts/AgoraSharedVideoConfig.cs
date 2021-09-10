
using agora_gaming_rtc;

public class AgoraSharedVideoConfig
{
    public const int FrameRate = (int)FRAME_RATE.FRAME_RATE_FPS_30;
    public const int Bitrate = 2000;
    private const int maxHeigh = 1280;

    public static void  GetResolution(int screenWidth, int screenHeigh, out int width, out int heigh) {
        float ratio = ((float)screenWidth) / screenHeigh;

        heigh = maxHeigh;
        width = (int)(maxHeigh * ratio);
    }
}
