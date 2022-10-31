/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders {

    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Unity.Collections;
    using Internal;

    /// <summary>
    /// Waveform audio recorder.
    /// </summary>
    public sealed class WAVRecorder : NativeRecorder {

        #region --Client API--
        /// <summary>
        /// Create a WAV recorder.
        /// </summary>
        /// <param name="sampleRate">Audio sample rate.</param>
        /// <param name="channelCount">Audio channel count.</param>
        public WAVRecorder (int sampleRate, int channelCount) : base(Create(sampleRate, channelCount)) { }

        /// <summary>
        /// This recorder does not support committing pixel buffers.
        /// </summary>
        public override void CommitFrame<T> (T[] pixelBuffer, long timestamp = default) {
            base.CommitFrame(pixelBuffer, timestamp);
        }

        /// <summary>
        /// This recorder does not support committing pixel buffers.
        /// </summary>
        public override unsafe void CommitFrame<T> (NativeArray<T> pixelBuffer, long timestamp = default) {
            base.CommitFrame(pixelBuffer, timestamp);
        }

        /// <summary>
        /// This recorder does not support committing pixel buffers.
        /// </summary>
        public override unsafe void CommitFrame (void* pixelBuffer, long timestamp = default) {
            base.CommitFrame(pixelBuffer, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Not used.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void CommitSamples (float[] sampleBuffer, long timestamp = default) {
            base.CommitSamples(sampleBuffer, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Not used.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void CommitSamples (NativeArray<float> sampleBuffer, long timestamp = default) {
            base.CommitSamples(sampleBuffer, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="sampleCount">Total number of samples in the buffer.</param>
        /// <param name="timestamp">Not used.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override unsafe void CommitSamples (float* sampleBuffer, int sampleCount, long timestamp = default) {
            base.CommitSamples(sampleBuffer, sampleCount, timestamp);
        }

        /// <summary>
        /// Finish writing.
        /// </summary>
        /// <returns>Path to recorded waveform file.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override Task<string> FinishWriting () => base.FinishWriting();
        #endregion


        #region --Operations--

        private static IntPtr Create (int sampleRate, int channelCount) {
            NatCorder.CreateWAVRecorder(
                RecorderExtensions.GenerateRecordingPath(@".wav"),
                sampleRate,
                channelCount,
                out var recorder
            ).CheckStatus();
            if (recorder != IntPtr.Zero)
                return recorder;
            throw new InvalidOperationException(@"Failed to create WAVRecorder");
        }
        #endregion
    }
}