/* 
*   NatCorder
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Internal {

    using AOT;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public abstract class NativeRecorder : IMediaRecorder {
        
        #region --Client API--
        /// <summary>
        /// Video size.
        /// </summary>
        public virtual (int width, int height) frameSize {
            get {
                recorder.FrameSize(out var width, out var height).CheckStatus();
                return (width, height);
            }
        }

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitFrame<T> (T[] pixelBuffer, long timestamp) where T : unmanaged {
            fixed (T* baseAddress = pixelBuffer)
                CommitFrame(baseAddress, timestamp);
        }

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitFrame<T> (NativeArray<T> pixelBuffer, long timestamp) where T : unmanaged {
            CommitFrame(pixelBuffer.GetUnsafeReadOnlyPtr(), timestamp);
        }

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitFrame (void* pixelBuffer, long timestamp) => recorder.CommitFrame(
            pixelBuffer,
            timestamp
        ).CheckStatus();

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitSamples (float[] sampleBuffer, long timestamp) {
            fixed (float* baseAddress = sampleBuffer)
                CommitSamples(baseAddress, sampleBuffer.Length, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitSamples (NativeArray<float> sampleBuffer, long timestamp) => CommitSamples(
            (float*)sampleBuffer.GetUnsafeReadOnlyPtr(),
            sampleBuffer.Length,
            timestamp
        );

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="sampleCount">Total number of samples in the buffer.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        public virtual unsafe void CommitSamples (float* sampleBuffer, int sampleCount, long timestamp) => recorder.CommitSamples(
            sampleBuffer,
            sampleCount,
            timestamp
        ).CheckStatus();

        /// <summary>
        /// Finish writing.
        /// </summary>
        /// <returns>Path to recorded media file.</returns>
        public virtual unsafe Task<string> FinishWriting () {
            var recordingTask = new TaskCompletionSource<string>();
            var handle = GCHandle.Alloc(recordingTask, GCHandleType.Normal);
            try {
                recorder.FinishWriting(OnRecorderCompleted, (IntPtr)handle).CheckStatus();
                return recordingTask.Task;
            } catch {
                handle.Free();
                throw;
            }
        }
        #endregion


        #region --Operations--
        private readonly IntPtr recorder;
        private static string directory;

        protected NativeRecorder (IntPtr recorder) => this.recorder = recorder;

        /// <summary>
        /// Create a recording path.
        /// </summary>
        /// <param name="extension">Recording file extension.</param>
        /// <returns>Recording path.</returns>
        internal static string CreatePath (string extension = null) {
            var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            var name = $"recording_{timestamp}{extension ?? string.Empty}";
            var path = Path.Combine(directory, name);
            return path;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnInitialize () {
            NatCorder.SetSessionToken(NatCorderSettings.Instance.Token);
            directory = Application.isEditor ? Directory.GetCurrentDirectory() : Application.persistentDataPath;
        }

        [MonoPInvokeCallback(typeof(NatCorder.RecordingHandler))]
        private static unsafe void OnRecorderCompleted (IntPtr context, IntPtr path) {
            // Get task
            var handle = (GCHandle)context;
            var recordingTask = handle.Target as TaskCompletionSource<string>;
            handle.Free();
            // Complete task
            if (path != IntPtr.Zero)
                recordingTask.SetResult(Marshal.PtrToStringAnsi(path));
            else
                recordingTask.SetException(new Exception(@"Recorder failed to finish writing"));
        }

        public static implicit operator IntPtr (NativeRecorder recorder) => recorder.recorder;
        #endregion
    }
}