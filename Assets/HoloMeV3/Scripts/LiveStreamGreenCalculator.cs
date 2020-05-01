using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveStreamGreenCalculator : MonoBehaviour
{
    [SerializeField]
    Material videoMat;

    [SerializeField]
    [Range(0, 1)]
    float originalChroma = .2f;

    [SerializeField]
    [Range(0, 1)]
    float originalChromaT = 0.05f;

    [SerializeField]
    [Range(0, 255)]
    int redDetectionStength = 0;

    [SerializeField]
    [Range(0, 256)]
    int greenDetectionStength = 75;

    [SerializeField]
    [Range(0, 255)]
    int blueDetectionStength = 0;

    [SerializeField]
    bool regenerate;

    public void StartBackgroundRemoval()
    {
        StartCoroutine(StartPicking(true));
    }

    IEnumerator StartPicking(bool resetShaderValues)
    {
        if (resetShaderValues)
        {
            //originalChroma = videoMat.GetFloat("_DChroma");
            videoMat.SetColor("_KeyColor", Color.white);
            videoMat.SetFloat("_DChroma", 0);
            videoMat.SetFloat("_DChromaT", 1);
        }

        yield return new WaitForSeconds(3);

        //while (!videoPlayer.isPlaying && !videoPlayer.isPrepared)
        //{
        //    yield return new WaitForSeconds(1);
        //}

        //videoPlayer.Pause();

        //yield return new WaitForSeconds(1);

        PickColour();

        yield return new WaitForSeconds(1);
    }

    private void PickColour()
    {

        //Texture2D newText2d = (Texture2D)videoPlayer.texture;

        ////Texture2D newText2d = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.ARGB32, false);
        ////newText2d.ReadPixels(Camera.main.pixelRect, 0, 0, false);

        ////Texture2D newText2d = new Texture2D((int)videoPlayer.width, (int)videoPlayer.height, TextureFormat.ARGB32, false);
        ////newText2d.ReadPixels(new Rect(0, 0, (int)videoPlayer.width, (int)videoPlayer.height), 0, 0);

        //newText2d.Apply();

        Texture texture = videoMat.mainTexture;
        Texture2D newText2d = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        newText2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        newText2d.Apply();
        RenderTexture.active = currentRT;

        videoMat.SetColor("_KeyColor", AverageColorFromTexture(newText2d));
        videoMat.SetFloat("_DChroma", originalChroma);
        videoMat.SetFloat("_DChromaT", originalChromaT);
    }

    Color32 AverageColorFromTexture(Texture2D tex)
    {
        Color32[] texColors = tex.GetPixels32();

        List<Color32> texColorsList = new List<Color32>(texColors);

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        int total2 = 0;

        for (int i = 0; i < total; i++)
        {
            if (texColors[i].r >= redDetectionStength && texColors[i].g >= greenDetectionStength && texColors[i].b >= blueDetectionStength)
            {
                total2++;
                r += texColors[i].r;
                g += texColors[i].g;
                b += texColors[i].b;
            }
        }
        return new Color32((byte)(r / total2), (byte)(g / total2), (byte)(b / total2), 0);
    }

    private void Update()
    {
        if (regenerate)
        {
            StartCoroutine(StartPicking(true));
            regenerate = false;
        }
    }
}


