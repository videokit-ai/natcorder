/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Recorders.Editor")]
namespace NatML.Recorders {

    using Unity.Collections;

    /// <summary>
    /// A media writer capable of writing video frames and/or audio frames, to a media output.
    /// All writer methods are thread safe, and as such can be called from any thread.
    /// </summary>
    public interface IMediaWriter {

        /// <summary>
        /// Recording video frame size.
        /// </summary>
        (int width, int height) frameSize { get; }

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        void CommitFrame<T> (T[] pixelBuffer, long timestamp) where T : unmanaged;

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        void CommitFrame<T> (NativeArray<T> pixelBuffer, long timestamp) where T : unmanaged;

        /// <summary>
        /// Commit a video pixel buffer for encoding.
        /// The pixel buffer MUST have an RGBA8888 pixel layout.
        /// </summary>
        /// <param name="pixelBuffer">Pixel buffer to commit.</param>
        /// <param name="timestamp">Pixel buffer timestamp in nanoseconds.</param>
        unsafe void CommitFrame (void* pixelBuffer, long timestamp);

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        void CommitSamples (float[] sampleBuffer, long timestamp);

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        void CommitSamples (NativeArray<float> sampleBuffer, long timestamp);

        /// <summary>
        /// Commit an audio sample buffer for encoding.
        /// The sample buffer MUST be a linear PCM buffer interleaved by channel.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer to commit.</param>
        /// <param name="sampleCount">Total number of samples in the buffer.</param>
        /// <param name="timestamp">Sample buffer timestamp in nanoseconds.</param>
        unsafe void CommitSamples (float* sampleBuffer, int sampleCount, long timestamp);
    }
}