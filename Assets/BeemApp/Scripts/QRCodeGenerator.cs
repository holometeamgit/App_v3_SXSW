using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Beem.SSO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// QRCode Generator
/// </summary>
public class QRCodeGenerator {

    private Texture2D _storeEncoding;
    private Texture2D _boardingTexture;

    private const int WIDTH = 256;
    private const int HEIGHT = 256;
    private const string DEFAULR_QRCODE_STRING= "https://www.beem.me/";

    public QRCodeGenerator() {
        _storeEncoding = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGBA32, false);
        CallBacks.onGetQRCode += GenerateQRCode;

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Addressables.LoadAssetAsync<Texture2D>("QRCodeBoard").Task.ContinueWith(task => {
            _boardingTexture = task.Result;
        }, taskScheduler);
    }

    /// <summary>
    /// Generate QRCode
    /// </summary>
    /// <param name="qrcodeString"></param>
    public void GenerateQRCode(string qrcodeString) {
        EncodeString(qrcodeString);
        CallBacks.onQRCodeCreated?.Invoke(_storeEncoding);

    }

    private void EncodeString(string qrcodeString) {
        Color32[] convertPixelToTexture = GetColorsQrCode(qrcodeString);
        var result = AddAdditionalInfo(convertPixelToTexture, WIDTH, HEIGHT);
        EncodeTextureToQRCode(result);
    }

    private Color32[] GetColorsQrCode(string qrCodeString) {
        string textWrite = string.IsNullOrEmpty(qrCodeString) ? DEFAULR_QRCODE_STRING : qrCodeString;
        Color32[] convertPixelToTexture = Encode(textWrite, _storeEncoding.width, _storeEncoding.height);

        return convertPixelToTexture;
    }

    private void EncodeTextureToQRCode(Color32[] convertPixelToTexture) {
        _storeEncoding.SetPixels32(convertPixelToTexture);
        _storeEncoding.Apply();
    }

    private Color32[] AddAdditionalInfo(Color32[] qrcode, int widthQRCodeTexture, int heightQRCodeTexture) {

        Color32[] _boardingColors = _boardingTexture.GetPixels32();

        Color32 whiteColor = new Color32(255, 255, 255, 255);
        Color32[] result = new Color32[widthQRCodeTexture * heightQRCodeTexture];

        for (int height = 0; height < heightQRCodeTexture; height++) {
            for (int width = 0; width < widthQRCodeTexture; width++) {
                int id = height * WIDTH + width;
                result[id] = qrcode[id];
                if (!_boardingColors[id].CompareRGB(whiteColor)) {
                    result[id] = _boardingColors[id];
                    if(_boardingColors[id].a < 255) {
                        result[id] = new Color32(0, 0, 0, _boardingColors[id].a);
                    }
                }
            }
        }

        return result;
    }

    private Color32[] Encode(string textForEncoding, int width, int height) {
        BarcodeWriter writter = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = height,
                Width = width,
                Margin = 2,
                PureBarcode = false
            }
        };
        return writter.Write(textForEncoding);
    }

    ~QRCodeGenerator() {
        HelperFunctions.DevLog("~QRCodeGenerator");
        CallBacks.onGetQRCode -= GenerateQRCode;
    }
}
