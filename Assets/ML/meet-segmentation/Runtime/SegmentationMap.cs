/* 
*   Meet Segmentation
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatSuite.ML.Vision {

    using System;
    using UnityEngine;

    public sealed partial class MeetSegmentationPredictor {

        /// <summary>
        /// Segmentation map.
        /// Each pixel in the map returns a probability of that pixel location being a person (~1.0) or background (~0.0).
        /// </summary>
        public sealed class SegmentationMap {

            #region --Client API--
            /// <summary>
            /// Map width.
            /// </summary>
            public readonly int width;

            /// <summary>
            /// Map height.
            /// </summary>
            public readonly int height;

            /// <summary>
            /// Render the probability map to a texture.
            /// Each pixel will have value `(p, p, p, 1.0)` where `p` is the foreground probability for that pixel.
            /// </summary>
            /// <param name="destination">Destination texture.</param>
            public void Render (RenderTexture destination) {
                // Check texture
                if (!destination)
                    throw new ArgumentNullException(nameof(destination));
                // Create buffer
                using var mapBuffer = new ComputeBuffer(width * height, 2 * sizeof(float));
                // Upload
                mapBuffer.SetData(data);
                // Create temporary
                var descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
                descriptor.enableRandomWrite = true;
                var tempBuffer = RenderTexture.GetTemporary(descriptor);
                tempBuffer.Create();
                // Render
                renderer = renderer ?? (ComputeShader)Resources.Load(@"MeetSegmentationRenderer");
                renderer.SetBuffer(0, @"Map", mapBuffer);
                renderer.SetTexture(0, @"Result", tempBuffer);
                renderer.GetKernelThreadGroupSizes(0, out var gx, out var gy, out var _);
                renderer.Dispatch(0, Mathf.CeilToInt((float)width / gx), Mathf.CeilToInt((float)height / gy), 1);
                // Blit to destination
                Graphics.Blit(tempBuffer, destination);
                RenderTexture.ReleaseTemporary(tempBuffer);
            }
            #endregion


            #region --Operations--
            private readonly float[] data;
            private static ComputeShader renderer;

            internal SegmentationMap (int width, int height, float[] data) {
                this.width = width;
                this.height = height;
                this.data = data;
            }
            #endregion
        }
    }
}