/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using AOT;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using Internal;

    /// <summary>
    /// Recorder input for recording video frames from textures with hardware acceleration on Android OpenGL ES3.
    /// </summary>
    public sealed class GLESTextureInput : TextureInput {

        #region --Client API--
        /// <summary>
        /// Create a GLES texture input.
        /// </summary>
        /// <param name="recorder">Media recorder to receive video frames.</param>
        public GLESTextureInput (IMediaRecorder recorder) : base(recorder) {
            // Check platform
            if (Application.platform != RuntimePlatform.Android)
                throw new InvalidOperationException(@"GLESTextureInput can only be used on Android");
            // Check render API
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
                throw new InvalidOperationException(@"GLESTextureInput can only be used with OpenGL ES3");
            // Initialize
            var (width, height) = recorder.frameSize;
            this.frameBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            this.frameBuffer.Create();
            this.frameBufferID = frameBuffer.GetNativeTexturePtr();
            // Create native input
            NatCorderExt.CreateTexutreInput(width, height, OnReadbackCompleted, out input);
            this.commit = true;
        }

        /// <summary>
        /// Commit a video frame from a texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds.</param>
        public override unsafe void CommitFrame (Texture texture, long timestamp) {
            Action<IntPtr> callback = pixelBuffer => recorder.CommitFrame((void*)pixelBuffer, timestamp);
            var handle = GCHandle.Alloc(callback, GCHandleType.Normal);
            var commandBuffer = new CommandBuffer();
            commandBuffer.name = @"GLESTextureInput";
            commandBuffer.Blit(texture, frameBuffer);
            commandBuffer.RunOnRenderThread(() => {
                lock (this)
                    if (commit)
                        input.CommitFrame(frameBufferID, (IntPtr)handle);
                    else
                        handle.Free();
            });
            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        /// <summary>
        /// Stop recorder input and release resources.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Dispose () {
            commit = false;
            frameBuffer.Release();
            input.ReleaseTextureInput();
            base.Dispose();
        }
        #endregion


        #region --Operations--
        private readonly RenderTexture frameBuffer;
        private readonly IntPtr frameBufferID;
        private readonly IntPtr input;
        private bool commit;

        [MonoPInvokeCallback(typeof(NatCorderExt.ReadbackHandler))]
        private static void OnReadbackCompleted (IntPtr context, IntPtr pixelBuffer) {
            var handle = (GCHandle)context;
            var handler = handle.Target as Action<IntPtr>;
            handle.Free();
            handler?.Invoke(pixelBuffer);
        }
        #endregion
    }
}