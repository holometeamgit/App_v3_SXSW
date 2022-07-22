
using agora_gaming_rtc;

public class AgoraSharedVideoConfig {
    public const int FrameRate = (int)FRAME_RATE.FRAME_RATE_FPS_30;
    public const int Bitrate = 2000;
    private const int MAX_HEIGHT = 1280;

    public static void GetResolution(int screenWidth, int screenHeigh, out int width, out int heigh) {
        float ratio = ((float)screenWidth) / screenHeigh;

        heigh = MAX_HEIGHT;
        width = (int)(MAX_HEIGHT * ratio);
    }

    public static void GetResolution(int screenWidth, int screenHeigh, out int width, out int heigh, int maxHeigh) {
        float ratio = ((float)screenWidth) / screenHeigh;

        heigh = maxHeigh;
        width = (int)(maxHeigh * ratio);

        heigh = MakeEven(heigh);
        width = MakeEven(width);
    }

    private static int MakeEven(int value) {
        return value % 2 == 0 ? value : value - 1;
    }
}
