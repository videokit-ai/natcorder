/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using UnityEngine;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Recorder input for recording video frames from textures.
    /// Textures will be recorded by performing a synchronous pixel buffer readback.
    /// </summary>
    public sealed class TextureInput : ITextureInput {

        #region --Client API--
        /// <summary>
        /// Create a texture input which performs synchronous readbacks.
        /// </summary>
        /// <param name="recorder">Media recorder to receive video frames.</param>
        public TextureInput (IMediaRecorder recorder) {
            this.recorder = recorder;
            this.readbackBuffer = new Texture2D(
                recorder.frameSize.width,
                recorder.frameSize.height,
                TextureFormat.RGBA32,
                false,
                false
            );
        }

        /// <summary>
        /// Commit a video frame from a texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds.</param>
        public unsafe void CommitFrame (Texture texture, long timestamp) {
            // Blit
            var (width, height) = recorder.frameSize;
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            Graphics.Blit(texture, renderTexture);
            // Readback // Completely kills performance
            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            readbackBuffer.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            RenderTexture.active = prevActive;
            RenderTexture.ReleaseTemporary(renderTexture);
            // Commit
            recorder.CommitFrame(readbackBuffer.GetRawTextureData<byte>(), timestamp);
        }

        /// <summary>
        /// Stop recorder input and release resources.
        /// </summary>
        public void Dispose () => Texture2D.Destroy(readbackBuffer);
        #endregion


        #region --Operations--
        private readonly IMediaRecorder recorder;
        private readonly Texture2D readbackBuffer;
        (int, int) ITextureInput.frameSize => recorder.frameSize;
        #endregion
    }
}