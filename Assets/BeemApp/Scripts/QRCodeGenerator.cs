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

/// <summary>
/// QRCode Generator
/// </summary>
public class QRCodeGenerator {

    private Texture2D _storeEncoding;
    private QRCodeAdditionalInformationScriptableObject _QRCodeAdditionalInformationScriptableObject;
    private bool _isLock;

    private const int WIDTH = 256;
    private const int HEIGHT = 256;
    private const string DEFAULR_QRCODE_STRING= "https://www.beem.me/";

    public QRCodeGenerator(QRCodeAdditionalInformationScriptableObject qrCodeAdditionalInformationScriptableObject) {
        _QRCodeAdditionalInformationScriptableObject = qrCodeAdditionalInformationScriptableObject;
        _storeEncoding = new Texture2D(WIDTH, HEIGHT);
        CallBacks.onGetQRCode += GenerateQRCode;
    }

    /// <summary>
    /// Generate QRCode
    /// </summary>
    /// <param name="qrcodeString"></param>
    public void GenerateQRCode(string qrcodeString) {
        if (_isLock)
            return;

        _isLock = true;

        /*       var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
               EncodeStringAsync(qrcodeString).ContinueWith(task => {
                   _isLock = false;
                   CallBacks.onQRCodeCreated?.Invoke(_storeEncoding);
               }, taskScheduler);*/

        EncodeString(qrcodeString);
    }

    private void EncodeString(string qrcodeString) {
        Color32[] convertPixelToTexture = GetColorsQrCode(qrcodeString);
        var result = AddAdditionalInfo(convertPixelToTexture, WIDTH, HEIGHT);
        EncodeTextureToQRCode(result);
    }

   /* private async Task EncodeStringAsync(string qrcodeString) {
        Color32[] convertPixelToTexture = GetColorsQrCode(qrcodeString);
        var result = await AddAdditionalInfo(convertPixelToTexture, WIDTH, HEIGHT);
        EncodeTextureToQRCode(result);
    }*/

    private Color32[] GetColorsQrCode(string qrCodeString) {
        string textWrite = string.IsNullOrEmpty(qrCodeString) ? DEFAULR_QRCODE_STRING : qrCodeString;
        Color32[] convertPixelToTexture = Encode(textWrite, _storeEncoding.width, _storeEncoding.height);

        return convertPixelToTexture;
    }

    private void EncodeTextureToQRCode(Color32[] convertPixelToTexture) {
        _storeEncoding.SetPixels32(convertPixelToTexture);
        _storeEncoding.Apply();
    }

    
    private async Task<Color32[]> AddAdditionalInfoAsync(Color32[] qrcode, int widthQRCodeTexture, int heightQRCodeTexture) {

        qrcode = AddAdditionalInfo(qrcode, widthQRCodeTexture, heightQRCodeTexture);

        return qrcode;
    }

    private Color32[] AddAdditionalInfo(Color32[] qrcode, int widthQRCodeTexture, int heightQRCodeTexture) {


        Color32[] _boardingColors = _QRCodeAdditionalInformationScriptableObject.BoardingSprite.GetPixels32();

        Color32[] _logoColors = _QRCodeAdditionalInformationScriptableObject.LogoSprite.GetPixels32();
        Color32 whiteColor = new Color32(255, 255, 255, 255);

        for (int height = 0; height < heightQRCodeTexture; height++) {
            for (int width = 0; width < widthQRCodeTexture; width++) {
                int id = height * WIDTH + width;
                if (!_boardingColors[id].CompareRGB(whiteColor)) {
                    qrcode[id] = _boardingColors[id];
                }
                if (_logoColors[id].a > 100) {
                    qrcode[id] = _logoColors[id];
                }
            }
        }

        return qrcode;
    }

    private Color32[] Encode(string textForEncoding, int width, int height) {
        BarcodeWriter writter = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = height,
                Width = width,
                Margin = 1,
                PureBarcode = false
            }
        };
        return writter.Write(textForEncoding);
    }

    ~QRCodeGenerator() {
        CallBacks.onGetQRCode -= GenerateQRCode;
    }
}
