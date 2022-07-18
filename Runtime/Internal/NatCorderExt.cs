/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Internal {

    using System;
    using System.Runtime.InteropServices;

    public static class NatCorderExt { // NatCorder.h

        #region --Delegates--
        public delegate void ReadbackHandler (IntPtr context, IntPtr pixelBuffer);
        #endregion


        #region --GLESTextureInput--
        #if UNITY_ANDROID && !UNITY_EDITOR
        [DllImport(NatCorder.Assembly, EntryPoint = @"NCCreateGLESTextureInput")]
        public static extern void CreateTexutreInput (
            int width,
            int height,
            ReadbackHandler handler,
            out IntPtr input
        );
        [DllImport(NatCorder.Assembly, EntryPoint = @"NCGLESTextureInputCommitFrame")]
        public static extern void CommitFrame (
            this IntPtr input,
            IntPtr texture,
            IntPtr context
        );
        [DllImport(NatCorder.Assembly, EntryPoint = @"NCReleaseGLESTextureInput")]
        public static extern void ReleaseTextureInput (this IntPtr input);
        #else
        public static void CreateTexutreInput (
            int width,
            int height,
            ReadbackHandler handler,
            out IntPtr input
        ) => input = IntPtr.Zero;
        public static void CommitFrame (
            this IntPtr input,
            IntPtr texture,
            IntPtr context
        ) { }
        public static void ReleaseTextureInput (this IntPtr input) { }
        #endif
        #endregion
    }
}