/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders {

    using System;
    using Internal;

    /// <summary>
    /// MP4 video recorder.
    /// </summary>
    public sealed class MP4Recorder : NativeRecorder {

        #region --Client API--
        /// <summary>
        /// Create an MP4 recorder.
        /// </summary>
        /// <param name="width">Video width.</param>
        /// <param name="height">Video height.</param>
        /// <param name="frameRate">Video frame rate.</param>
        /// <param name="sampleRate">Audio sample rate. Pass 0 for no audio.</param>
        /// <param name="channelCount">Audio channel count. Pass 0 for no audio.</param>
        /// <param name="videoBitRate">Video bit rate in bits per second.</param>
        /// <param name="keyframeInterval">Keyframe interval in seconds.</param>
        /// <param name="audioBitRate">Audio bit rate in bits per second.</param>
        public MP4Recorder (
            int width,
            int height,
            float frameRate,
            int sampleRate = 0,
            int channelCount = 0,
            int videoBitRate = 10_000_000,
            int keyframeInterval = 2,
            int audioBitRate = 64_000
        ) : base(Create(width, height, frameRate, sampleRate, channelCount, videoBitRate, keyframeInterval, audioBitRate)) { }
        #endregion


        #region --Operations--

        private static IntPtr Create (
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitRate,
            int keyframeInterval,
            int audioBitRate
        ) {
            // Check
            if (width % 2 != 0)
                throw new ArgumentException(@"MP4Recorder width must be divisible by 2", nameof(width));
            if (height % 2 != 0)
                throw new ArgumentException(@"MP4Recorder height must be divisible by 2", nameof(height));
            // Create
            NatCorder.CreateMP4Recorder(
                RecorderExtensions.GenerateRecordingPath(@".mp4"),
                width,
                height,
                frameRate,
                sampleRate,
                channelCount,
                videoBitRate,
                keyframeInterval,
                audioBitRate,
                out var recorder
            ).CheckStatus();
            if (recorder != IntPtr.Zero)
                return recorder;
            throw new InvalidOperationException(@"Failed to create MP4Recorder");
        }
        #endregion
    }
}