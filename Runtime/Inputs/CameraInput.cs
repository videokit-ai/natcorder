/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Rendering;
    using Clocks;

    /// <summary>
    /// Recorder input for recording video frames from one or more game cameras.
    /// </summary>
    public class CameraInput : IDisposable {

        #region --Client API--
        /// <summary>
        /// Control number of successive camera frames to skip while recording.
        /// This is very useful for GIF recording, which typically has a lower framerate appearance.
        /// </summary>
        public int frameSkip;

        /// <summary>
        /// Configure the camera input for HDR or post-processing FX rendering.
        /// </summary>
        public bool HDR {
            get => frameDescriptor.colorFormat == RenderTextureFormat.ARGBHalf;
            set => frameDescriptor.colorFormat = value ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
        }

        /// <summary>
        /// When an Android renders with OpenGL ES3, the default `TextureInput` implementation can be very expensive.
        /// Enabling this flag will use the custom `GLESTextureInput` to accelerate pixel buffer readbacks from the GPU.
        /// ARFoundation apps can see rendering performance greatly benefit from enabling this flag.
        /// This flag only has an effect on Android when rendering with OpenGL ES3.
        /// This flag defaults to `true`.
        /// </summary>
        public static bool UseAndroidGLESTextureInput = true;

        /// <summary>
        /// Create a video recording input from one or more game cameras.
        /// </summary>
        /// <param name="recorder">Media recorder to receive video frames.</param>
        /// <param name="clock">Recording clock for generating timestamps.</param>
        /// <param name="cameras">Game cameras to record.</param>
        public CameraInput (IMediaRecorder recorder, IClock clock, params Camera[] cameras) : this(CreateInput(recorder), clock, cameras) { }
        
        /// <summary>
        /// Create a video recording input from one or more game cameras.
        /// </summary>
        /// <param name="recorder">Media recorder to receive video frames.</param>
        /// <param name="cameras">Game cameras to record.</param>
        public CameraInput (IMediaRecorder recorder, params Camera[] cameras) : this(recorder, default, cameras) { }

        /// <summary>
        /// Create a video recording input from one or more game cameras.
        /// </summary>
        /// <param name="input">Texture input to receive video frames.</param>
        /// <param name="clock">Recording clock for generating timestamps.</param>
        /// <param name="cameras">Game cameras to record.</param>
        public CameraInput (ITextureInput input, IClock clock, params Camera[] cameras) {
            // Sort cameras by depth
            Array.Sort(cameras, (a, b) => (int)(100 * (a.depth - b.depth)));
            var (width, height) = input.frameSize;
            // Save state
            this.input = input;
            this.clock = clock;
            this.cameras = cameras;
            this.frameDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 24) {
                sRGB = true,
                msaaSamples = Mathf.Max(QualitySettings.antiAliasing, 1)
            };
            // Start recording
            attachment = new GameObject("NatCorder CameraInputAttachment").AddComponent<CameraInputAttachment>();
            attachment.StartCoroutine(CommitFrames());
        }

        /// <summary>
        /// Create a video recording input from one or more game cameras.
        /// </summary>
        /// <param name="input">Texture input to receive video frames.</param>
        /// <param name="cameras">Game cameras to record.</param>
        public CameraInput (ITextureInput input, params Camera[] cameras) : this(input, default, cameras) { }

        /// <summary>
        /// Stop recorder input and release resources.
        /// </summary>
        public void Dispose () {
            GameObject.Destroy(attachment.gameObject);
            input.Dispose();
        }
        #endregion


        #region --Operations--
        private readonly ITextureInput input;
        private readonly IClock clock;
        protected readonly Camera[] cameras;
        private readonly CameraInputAttachment attachment;
        private RenderTextureDescriptor frameDescriptor;
        private int frameCount;

        private IEnumerator CommitFrames () {
            var yielder = new WaitForEndOfFrame();
            for (;;) {
                // Check frame index
                yield return yielder;
                if (frameCount++ % (frameSkip + 1) != 0)
                    continue;
                // Render cameras
                var frameBuffer = RenderTexture.GetTemporary(frameDescriptor);
                for (var i = 0; i < cameras.Length; i++)
                    CommitFrame(cameras[i], frameBuffer);
                // Commit
                input.CommitFrame(frameBuffer, clock?.timestamp ?? 0L);
                RenderTexture.ReleaseTemporary(frameBuffer);
            }
        }

        protected virtual void CommitFrame (Camera source, RenderTexture destination) {
            var prevTarget = source.targetTexture;
            source.targetTexture = destination;
            source.Render();
            source.targetTexture = prevTarget;
        }

        private static ITextureInput CreateInput (IMediaRecorder recorder) {
            if (SystemInfo.supportsAsyncGPUReadback) 
                return new AsyncTextureInput(recorder);
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && UseAndroidGLESTextureInput)
                return new GLESTextureInput(recorder);
            return new TextureInput(recorder);
        }

        private sealed class CameraInputAttachment : MonoBehaviour { }
        #endregion
    }
}