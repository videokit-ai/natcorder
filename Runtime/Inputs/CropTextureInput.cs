/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using UnityEngine;

    /// <summary>
    /// Recorder input for recording video frames from textures with cropping.
    /// The crop texture input always preserves the aspect of the cropped image.
    /// </summary>
    public sealed class CropTextureInput : ITextureInput {

        #region --Client API--
        /// <summary>
        /// Crop rect in pixel coordinates of the recorder.
        /// </summary>
        public RectInt rect;

        /// <summary>
        /// Whether to letterbox cropped region in frame instead of filling the frame.
        /// </summary>
        public bool letterbox;

        /// <summary>
        /// Background color when letterboxing frame.
        /// </summary>
        public Color background = Color.black;

        /// <summary>
        /// Create a crop texture input.
        /// </summary>
        /// <param name="input">Backing texture input to receive cropped frames.</param>
        public CropTextureInput (ITextureInput input) {
            this.input = input;
            this.rect = new RectInt(0, 0, input.frameSize.width, input.frameSize.height);
            this.shader = Resources.Load(@"CropTextureInput") as ComputeShader;
        }

        /// <summary>
        /// Commit a video frame from a texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds.</param>
        public void CommitFrame (Texture texture, long timestamp) {
            // Compute crop scale
            var (width, height) = input.frameSize;
            var frameSize = new Vector2(width, height);
            var scaleVector = new Vector2(frameSize.x / rect.width, frameSize.y / rect.height);
            var scale = letterbox ? Mathf.Min(scaleVector.x, scaleVector.y) : Mathf.Max(scaleVector.x, scaleVector.y);
            var center = new Vector2(rect.center.x / frameSize.x, rect.center.y / frameSize.y);
            // Compute bounds
            var pixelSize = scale * (Vector2)rect.size;
            var boundSize = new Vector2(pixelSize.x / frameSize.x, pixelSize.y / frameSize.y);
            var boundOffset = 0.5f * (Vector2.one - boundSize);
            var bounds = new Rect(boundOffset, boundSize);
            // Create retex
            var descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
            descriptor.enableRandomWrite = true;
            var result = RenderTexture.GetTemporary(descriptor);
            result.Create();
            // Render
            shader.SetTexture(0, @"Input", texture);
            shader.SetVector(@"Scale", new Vector3(center.x, center.y, scale));
            shader.SetVector(@"Bounds", new Vector4(bounds.xMin, bounds.xMax, bounds.yMin, bounds.yMax));
            shader.SetVector(@"Background", background);
            shader.SetTexture(0, @"Result", result);
            shader.GetKernelThreadGroupSizes(0, out var gx, out var gy, out var _);
            shader.Dispatch(0, Mathf.CeilToInt((float)width / gx), Mathf.CeilToInt((float)height / gy), 1);
            // Commit
            input.CommitFrame(result, timestamp);
            RenderTexture.ReleaseTemporary(result);
        }

        /// <summary>
        /// Stop recorder input and release resources.
        /// </summary>
        public void Dispose () => input.Dispose();
        #endregion


        #region --Operations--
        private readonly ITextureInput input;
        private readonly ComputeShader shader;
        (int, int) ITextureInput.frameSize => input.frameSize;
        #endregion
    }
}