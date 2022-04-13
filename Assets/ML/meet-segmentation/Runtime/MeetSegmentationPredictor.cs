/* 
*   Meet Segmentation
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatSuite.ML.Vision {

    using System;
    using NatSuite.ML.Features;
    using NatSuite.ML.Internal;
    using NatSuite.ML.Types;

    /// <summary>
    /// Meet segmentation from MediaPipe.
    /// </summary>
    public sealed partial class MeetSegmentationPredictor : IMLPredictor<MeetSegmentationPredictor.SegmentationMap> {

        #region --Client API--
        /// <summary>
        /// Create the meet segmentation predictor.
        /// </summary>
        /// <param name="model">Meet segmentation ML model.</param>
        public MeetSegmentationPredictor (MLModel model) => this.model = model as MLEdgeModel;

        /// <summary>
        /// Segment a person in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Segmentation map.</returns>
        public SegmentationMap Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"Meet segmentation predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            if (!MLImageType.FromType(input.type))
                throw new ArgumentException(@"Meet segmentation predictor expects an an array or image feature", nameof(inputs));
            // Predict
            using var inputFeature = (input as IMLEdgeFeature).Create(model.inputs[0]);
            using var outputFeatures = model.Predict(inputFeature);
            // Marshal // We don't have to do this, but we might as well correct the vertical mirroring
            var matte = new MLArrayFeature<float>(outputFeatures[0]);
            var result = new SegmentationMap(matte.shape[2], matte.shape[1], matte.ToArray());
            // Return
            return result;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;

        void IDisposable.Dispose () { }
        #endregion
    }
}