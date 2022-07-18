/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using UnityEngine;

    /// <summary>
    /// Recorder input for recording video frames from textures with a watermark.
    /// </summary>
    public sealed class WatermarkTextureInput : ITextureInput {

        #region --Client API--
        /// <summary>
        /// Watermark image.
        /// If `null`, no watermark will be rendered.
        /// </summary>
        public Texture watermark;

        /// <summary>
        /// Watermark display rect in pixel coordinates of the recorder.
        /// </summary>
        public RectInt rect;

        /// <summary>
        /// Create a watermark texture input.
        /// </summary>
        /// <param name="input">Backing texture input to receive watermarked frames.</param>
        public WatermarkTextureInput (ITextureInput input) {
            this.input = input;
            this.shader = Resources.Load(@"WatermarkTextureInput") as ComputeShader;
        }

        /// <summary>
        /// Commit a video frame from a texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds.</param>
        public void CommitFrame (Texture texture, long timestamp) {
            // Check
            if (!watermark) {
                input.CommitFrame(texture, timestamp);
                return;
            }
            // Create retex  
            var (width, height) = input.frameSize;            
            var descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
            descriptor.enableRandomWrite = true;
            var result = RenderTexture.GetTemporary(descriptor);
            result.Create();
            // Render
            shader.SetTexture(0, @"Input", texture);
            shader.SetTexture(0, @"Watermark", watermark);
            shader.SetVector(@"Rect", new Vector4(rect.xMin, rect.yMin, rect.width, rect.height));
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