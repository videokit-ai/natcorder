/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using AOT;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// Recorder input for recording video frames from textures with hardware acceleration on Android OpenGL ES.
    /// </summary>
    internal sealed class GLESTextureInput : ITextureInput { // INCOMPLETE

        #region --Client API--
        /// <summary>
        /// Create a GLES texture input.
        /// </summary>
        /// <param name="recorder">Media recorder to receive video frames.</param>
        public GLESTextureInput (IMediaRecorder recorder, Texture imageToCommit) {
            // Check render API
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
                throw new InvalidOperationException(@"GLESTextureInput can only be used with OpenGL ES3");
            // Initialize
            var (width, height) = recorder.frameSize;
            this.recorder = recorder;
            this.frameBuffer = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            this.frameBuffer.Create();

            this.frameBuffer2 = new Texture2D(width, height, TextureFormat.RGBA32, false);

            this.frameBufferID = frameBuffer.GetNativeTexturePtr();
            // Create native input
            CreateTexutreInput(width, height, OnReadbackCompleted, out input);


            this.image = imageToCommit;
        }

        /// <summary>
        /// Commit a video frame from a texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds.</param>
        public unsafe void CommitFrame (Texture texture, long timestamp) {
            Graphics.CopyTexture(texture, frameBuffer2);

            Action<IntPtr> callback = pixelBuffer => recorder?.CommitFrame((void*)pixelBuffer, timestamp);
            var handle = GCHandle.Alloc(callback, GCHandleType.Normal);
            var textureHandle = frameBuffer2.GetNativeTexturePtr();
            var commandBuffer = new CommandBuffer();

            commandBuffer.Blit(texture, frameBuffer);
            commandBuffer.RunOnRenderThread(() => CommitFrame(input, textureHandle, (IntPtr)handle));
            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        /// <summary>
        /// Stop recorder input and release resources.
        /// </summary>
        public void Dispose () {
            recorder = default;
            ReleaseTextureInput(input);
            frameBuffer.Release();
        }
        #endregion


        #region --Operations--
        private static readonly IntPtr RenderDelegate;
        private IMediaRecorder recorder;
        private readonly IntPtr input;
        private readonly RenderTexture frameBuffer;
        private readonly IntPtr frameBufferID;


        private readonly Texture2D frameBuffer2;

        private Texture image;

        (int, int) ITextureInput.frameSize => recorder.frameSize;

        [MonoPInvokeCallback(typeof(ReadbackHandler))]
        private static void OnReadbackCompleted (IntPtr context, IntPtr pixelBuffer) {
            var handle = (GCHandle)context;
            var data = handle.Target as Action<IntPtr>;
            handle.Free();
            data?.Invoke(pixelBuffer);
        }
        #endregion


        #region --Bridge--
        private delegate void ReadbackHandler (IntPtr context, IntPtr pixelBuffer);

        #if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(NatCorder.Assembly, EntryPoint = @"NCCreateGLESTextureInput")]
        private static extern void CreateTexutreInput (int width, int height, ReadbackHandler handler, out IntPtr input);
        [DllImport(NatCorder.Assembly, EntryPoint = @"NCGLESTextureInputCommitFrame")]
        private static extern void CommitFrame (IntPtr input, IntPtr texture, IntPtr context);
        [DllImport(NatCorder.Assembly, EntryPoint = @"NCReleaseGLESTextureInput")]
        private static extern void ReleaseTextureInput (IntPtr input);
        #else
        private static void CreateTexutreInput (int width, int height, ReadbackHandler handler, out IntPtr input) => input = IntPtr.Zero;
        private static void CommitFrame (IntPtr input, IntPtr texture, IntPtr context) { }
        private static void ReleaseTextureInput (IntPtr input) { }
        #endif
        #endregion
    }
}