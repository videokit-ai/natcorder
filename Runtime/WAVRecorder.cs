/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders {

    using System;
    using System.IO;
    using System.Text;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    /// <summary>
    /// Waveform audio recorder.
    /// </summary>
    public sealed class WAVRecorder : IMediaRecorder {

        #region --Client API--
        /// <summary>
        /// Not supported.
        /// </summary>
        public (int width, int height) frameSize => default;

        /// <summary>
        /// Create an WAV recorder.
        /// </summary>
        /// <param name="sampleRate">Audio sample rate.</param>
        /// <param name="channelCount">Audio channel count.</param>
        public WAVRecorder (int sampleRate, int channelCount) {
            var path = RecorderExtensions.GenerateRecordingPath(@".wav");
            this.sampleRate = sampleRate;
            this.channelCount = channelCount;
            this.stream = new FileStream(path, FileMode.Create);
            this.shortBuffer = new short[8192]; // should be big enough
            this.sampleCount = 0;
            // Pre-allocate WAVE header
            var header = new byte[44];
            stream.Write(header, 0, header.Length);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Not used.</param>
        public unsafe void CommitSamples (float[] sampleBuffer, long timestamp = default) {
            fixed (float* baseAddress = sampleBuffer)
                CommitSamples(baseAddress, sampleBuffer.Length, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Not used.</param>
        public unsafe void CommitSamples (NativeArray<float> sampleBuffer, long timestamp = default) {
            CommitSamples((float*)sampleBuffer.GetUnsafeReadOnlyPtr(), sampleBuffer.Length, timestamp);
        }

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="sampleCount">Total number of samples in the buffer.</param>
        /// <param name="timestamp">Not used.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public unsafe void CommitSamples (float* sampleBuffer, int sampleCount, long timestamp = default) {
            for (var i = 0; i < sampleCount; ++i)
                shortBuffer[i] = (short)(sampleBuffer[i] * short.MaxValue);
            fixed (short* data = shortBuffer)
                using (var bufferStream = new UnmanagedMemoryStream((byte*)data, sampleCount * sizeof(short)))
                    bufferStream.CopyTo(stream);
            this.sampleCount += sampleCount;
        }

        /// <summary>
        /// Finish writing.
        /// </summary>
        /// <returns>Path to recorded waveform file.</returns>
        public Task<string> FinishWriting () {
            // Write header
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(Encoding.UTF8.GetBytes("RIFF"), 0, 4);
            stream.Write(BitConverter.GetBytes(stream.Length - 8), 0, 4);
            stream.Write(Encoding.UTF8.GetBytes("WAVE"), 0, 4);
            stream.Write(Encoding.UTF8.GetBytes("fmt "), 0, 4);
            stream.Write(BitConverter.GetBytes(16), 0, 4);
            stream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);                                // Channel count
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);                                  // Sample rate
            stream.Write(BitConverter.GetBytes(sampleRate * channelCount * sizeof(short)), 0, 4);   // Output rate in bytes
            stream.Write(BitConverter.GetBytes((ushort)(channelCount * 2)), 0, 2);                  // Block alignment
            stream.Write(BitConverter.GetBytes((ushort)16), 0, 2);                                  // Bits per sample
            stream.Write(Encoding.UTF8.GetBytes("data"), 0, 4);
            stream.Write(BitConverter.GetBytes(sampleCount * sizeof(ushort)), 0, 4);                // Total sample count
            // Close stream and return
            stream.Dispose();
            return Task.FromResult(stream.Name);
        }
        #endregion


        #region --Operations--
        private readonly int sampleRate, channelCount;
        private readonly FileStream stream;
        private readonly short[] shortBuffer;
        private int sampleCount;

        unsafe void IMediaWriter.CommitFrame<T> (T[] pixelBuffer, long timestamp) {
            fixed (T* baseAddress = pixelBuffer)
                (this as IMediaRecorder).CommitFrame(baseAddress, timestamp);
        }

        unsafe void IMediaWriter.CommitFrame<T> (NativeArray<T> pixelBuffer, long timestamp) {
            (this as IMediaRecorder).CommitFrame(pixelBuffer.GetUnsafeReadOnlyPtr(), timestamp);
        }

        unsafe void IMediaWriter.CommitFrame (void* pixelBuffer, long timestamp) {
            Debug.LogError("NatCorder Error: WAVRecorder does not support committing video frames");
        }
        #endregion
    }
}