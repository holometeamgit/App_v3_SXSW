/* 
*   Meet Segmentation
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatSuite.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;
    using NatSuite.ML;
    using NatSuite.ML.Features;
    using NatSuite.ML.Vision;

    public sealed class MeetSegmentationSample : MonoBehaviour {

        [Header(@"NatML")]
        public string accessKey;

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;
        [SerializeField]
        private Material mask;

        WebCamTexture webCamTexture;
        RenderTexture segmentationImage;
        Material currentMask;

        MLModel model;
        MeetSegmentationPredictor predictor;
        Color32[] pixelBuffer;

        async void Start() {
            Debug.Log("Fetching model data from NatML...");
            // Fetch model data from NatML
            var modelData = await MLModelData.FromHub("@natsuite/meet-segmentation", accessKey);
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the predictor
            predictor = new MeetSegmentationPredictor(model);
            // Start the webcam
            webCamTexture = new WebCamTexture(1280, 720, 30);
            webCamTexture.Play();
            // Create and display the destination segmentation image
            while (webCamTexture.width == 16 || webCamTexture.height == 16)
                await Task.Yield();
            segmentationImage = new RenderTexture(webCamTexture.width, webCamTexture.height, 0);
            currentMask = new Material(mask);
            currentMask.SetTexture("_MaskTex", segmentationImage);
            currentMask.SetTexture("_MainTex", webCamTexture);
            rawImage.material = currentMask;

            aspectFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
        }

        void Update() {
            // Check that the segmentation image has been created
            if (!segmentationImage)
                return;
            // Check that the camera frame updated
            if (!webCamTexture.didUpdateThisFrame)
                return;
            // Create input feature
            pixelBuffer = webCamTexture.GetPixels32(pixelBuffer);
            var inputFeature = new MLImageFeature(pixelBuffer, webCamTexture.width, webCamTexture.height);
            // Predict
            var segmentationMap = predictor.Predict(inputFeature);
            segmentationMap.Render(segmentationImage);
        }

        void OnDisable() {
            // Dispose the model
            model?.Dispose();
        }
    }
}