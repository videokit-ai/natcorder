/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Inputs {

    using System;
    using UnityEngine;

    /// <summary>
    /// Recorder input for recording video frames from textures.
    /// </summary>
    public interface ITextureInput : IDisposable {

        /// <summary>
        /// Texture input frame size.
        /// It is usually not required that committed textures have the frame size, though it is highly recommended.
        /// </summary>
        (int width, int height) frameSize { get; }

        /// <summary>
        /// Commit a video frame from a texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="timestamp">Frame timestamp in nanoseconds.</param>
        void CommitFrame (Texture texture, long timestamp);
    }
}