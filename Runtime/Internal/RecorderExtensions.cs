/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders {

    using AOT;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal static class RecorderExtensions {

        #region --Client API--
        /// <summary>
        /// Generate a recording path.
        /// </summary>
        /// <param name="extension">Recording file extension.</param>
        /// <returns>Recording path.</returns>
        internal static string GenerateRecordingPath (string extension = null) {
            var timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            var name = $"recording_{timestamp}{extension ?? string.Empty}";
            var path = Path.Combine(directory, name);
            return path;
        }

        /// <summary>
        /// Register a delegate to be invoked on the Unity render thread.
        /// </summary>
        /// <param name="commandBuffer">Command buffer.</param>
        /// <param name="action">Delegate to invoke on the Unity render thread.</param>
        internal static void RunOnRenderThread (this CommandBuffer commandBuffer, Action action) {
            var handle = GCHandle.Alloc(action, GCHandleType.Normal);
            commandBuffer.IssuePluginEventAndData(RenderThreadCallback, default, (IntPtr)handle);
        }
        #endregion


        #region --Operations--
        private static string directory;
        private static readonly IntPtr RenderThreadCallback;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnityRenderingEventAndData (int _, IntPtr data);

        static RecorderExtensions () => RenderThreadCallback = Marshal.GetFunctionPointerForDelegate<UnityRenderingEventAndData>(OnRenderThreadInvoke);

        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialize () => directory = Application.isEditor ? Directory.GetCurrentDirectory() : Application.persistentDataPath;

        [MonoPInvokeCallback(typeof(UnityRenderingEventAndData))]
        private static void OnRenderThreadInvoke (int _, IntPtr context) {
            var handle = (GCHandle)context;
            var action = handle.Target as Action;
            handle.Free();
            action?.Invoke();
        }
        #endregion
    }
}