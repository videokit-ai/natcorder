/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders {

    using System.Threading.Tasks;

    /// <summary>
    /// A recorder capable of recording video frames, and optionally audio frames, to a media output.
    /// All recorder methods are thread safe, and as such can be called from any thread.
    /// </summary>
    public interface IMediaRecorder : IMediaWriter {

        /// <summary>
        /// Finish writing.
        /// </summary>
        /// <returns>Path to recorded media file.</returns>
        Task<string> FinishWriting ();
    }
}