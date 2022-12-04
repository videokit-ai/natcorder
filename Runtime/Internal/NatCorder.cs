/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Internal {

    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

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
            NotImplemented      = 3,
            InvalidSession      = 101,
            MissingNatMLHub     = 102,
            InvalidNatMLHub     = 103,
            InvalidPlan         = 104,
            LimitedPlan         = 105,
        }
        #endregion


        #region --Delegates--
        public delegate void RecordingHandler (IntPtr context, IntPtr path);
        #endregion


        #region --Initialization--
        [DllImport(Assembly, EntryPoint = @"NCSetSessionToken")]
        public static extern Status SetSessionToken (
            [MarshalAs(UnmanagedType.LPStr)] string token
        );
        #endregion


        #region --IMediaRecorder--
        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderGetFrameSize")]
        public static extern Status FrameSize (
            this IntPtr recorder,
            out int width,
            out int height
        );

        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderCommitFrame")]
        public static extern unsafe Status CommitFrame (
            this IntPtr recorder,
            void* pixelBuffer,
            long timestamp
        );

        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderCommitSamples")]
        public static extern unsafe Status CommitSamples (
            this IntPtr recorder,
            float* sampleBuffer,
            int sampleCount,
            long timestamp
        );

        [DllImport(Assembly, EntryPoint = @"NCMediaRecorderFinishWriting")]
        public static extern Status FinishWriting (
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
            float delay,
            out IntPtr recorder
        );

        [DllImport(Assembly, EntryPoint = @"NCCreateWAVRecorder")]
        public static extern Status CreateWAVRecorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int sampleRate,
            int channelCount,
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
            int keyframeInterval,
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
                case Status.NotImplemented:     throw new NotImplementedException();
                case Status.InvalidSession:     throw new InvalidOperationException(@"NatCorder session token is invalid. Check your NatML access key.");
                case Status.MissingNatMLHub:    throw new InvalidOperationException(@"NatMLHub native library could not be found.");
                case Status.InvalidNatMLHub:    throw new InvalidOperationException(@"NatMLHub native library is invalid.");
                case Status.InvalidPlan:        throw new InvalidOperationException(@"NatML billing plan does not support this operation. Check your plan and upgrade at https://hub.natml.ai");
                case Status.LimitedPlan:        Debug.LogWarning(@"NatML billing plan only allows for limited functionality. Check your plan and upgrade at https://hub.natml.ai"); break;
                default:                        throw new InvalidOperationException();
            }
        }
        #endregion
    }
}