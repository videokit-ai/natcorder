/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Internal {

    using System;
    using System.Runtime.InteropServices;

    public static class NatCorder { // NatCorder.h

        public const string Assembly =
        #if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
        @"__Internal";
        #else
        @"NatCorder";
        #endif


        #region --Enumerations--
        public enum Status : int {
            Ok                  = 0,
            InvalidArgument     = 1,
            InvalidOperation    = 2,
            MissingAppToken     = 100,
            InvalidAppToken     = 101,
            MissingNatMLHub     = 102,
            InvalidNatMLHub     = 103,
        }
        #endregion


        #region --Delegates--
        public delegate void RecordingHandler (IntPtr context, IntPtr path);
        #endregion


        #region --Initialization--
        [DllImport(Assembly, EntryPoint = @"NCSetAppToken")]
        public static extern Status SetAppToken (
            [MarshalAs(UnmanagedType.LPStr)] string token
        );
        #endregion


        #region --IMediaRecorder--
        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderFrameSize")]
        public static extern void FrameSize (
            this IntPtr recorder,
            out int width,
            out int height
        );
        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderCommitFrame")]
        public static extern unsafe void CommitFrame (
            this IntPtr recorder,
            void* pixelBuffer,
            long timestamp
        );
        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderCommitSamples")]
        public static extern unsafe void CommitSamples (
            this IntPtr recorder,
            float* sampleBuffer,
            int sampleCount,
            long timestamp
        );
        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderFinishWriting")]
        public static extern void FinishWriting (
            this IntPtr recorder,
            RecordingHandler handler,
            IntPtr context
        );
        #endregion


        #region --Constructors--
        [DllImport(Assembly, EntryPoint = @"NCCreateMP4Recorder")]
        public static extern Status CreateMP4Recorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitrate,
            int keyframeInterval,
            int audioBitRate,
            out IntPtr recorder
        );
        [DllImport(Assembly, EntryPoint = @"NCCreateHEVCRecorder")]
        public static extern Status CreateHEVCRecorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitRate,
            int keyframeInterval,
            int audioBitRate,
            out IntPtr recorder
        );
        [DllImport(Assembly, EntryPoint = @"NCCreateGIFRecorder")]
        public static extern Status CreateGIFRecorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int width,
            int height,
            float frameDuration,
            out IntPtr recorder
        );
        [DllImport(Assembly, EntryPoint = @"NCCreateWEBMRecorder")]
        public static extern Status CreateWEBMRecorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitRate,
            int audioBitRate,
            out IntPtr recorder
        );
        #endregion


        #region --Utility--

        public static void CheckStatus (this Status status) {
            switch (status) {
                case Status.Ok: break;
                case Status.InvalidArgument:    throw new ArgumentException();
                case Status.InvalidOperation:   throw new InvalidOperationException();
                case Status.MissingAppToken:    throw new InvalidOperationException(@"NatCorder app token has not been set. Check your NatML access key in 'Project Settings > NatML'");
                case Status.InvalidAppToken:    throw new InvalidOperationException(@"NatCorder app token is invalid. Check your NatML access key and plan.");
                case Status.MissingNatMLHub:    throw new InvalidOperationException(@"NatMLHub native library could not be found.");
                case Status.InvalidNatMLHub:    throw new InvalidOperationException(@"NatMLHub native library is invalid.");
                default:                        throw new InvalidOperationException();
            }
        }
        #endregion
    }
}