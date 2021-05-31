using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SnapShotController : MonoBehaviour {

    private Vector2 pivot = Vector2.one * 0.5f;
    private CancellationTokenSource cancelTokenSource;

    public async void CreateSnapShot() {
        cancelTokenSource = new CancellationTokenSource();
        try {
            await LoadAsyncTexture();
            await LoadAsyncSprite(LoadAsyncTexture().Result);
            SnapShotCallBacks.onSnapshotEnded?.Invoke(LoadAsyncSprite(LoadAsyncTexture().Result).Result);
        } finally {
            if (cancelTokenSource != null) {
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
            }
        }
    }

    private Task<Texture2D> LoadAsyncTexture() {
        return Task.Run(() => ScreenCapture.CaptureScreenshotAsTexture(1));
    }

    private Task<Sprite> LoadAsyncSprite(Texture2D texture2D) {
        return Task.Run(() => Sprite.Create(texture2D, new Rect(0, 0, Screen.width, Screen.height), pivot));
    }

    public void Cancel() {
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }
}
