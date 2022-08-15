/* 
*   NatCorder
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Recorders.Clocks {

    using System;
    using System.Runtime.CompilerServices;
    using Stopwatch = System.Diagnostics.Stopwatch;

    /// <summary>
    /// Realtime clock for generating timestamps
    /// </summary>
    public sealed class RealtimeClock : IClock {

        #region --Client API--
        /// <summary>
        /// Current timestamp in nanoseconds.
        /// The very first value reported by this property will always be zero.
        /// </summary>
        public long timestamp {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                var time = stopwatch.Elapsed.Ticks * 100L;
                if (!stopwatch.IsRunning)
                    stopwatch.Start();
                return time;
            }
        }

        /// <summary>
        /// Is the clock paused?
        /// </summary>
        public bool paused {
            [MethodImpl(MethodImplOptions.Synchronized)] get => !stopwatch.IsRunning;
            [MethodImpl(MethodImplOptions.Synchronized)] set => (value ? (Action)stopwatch.Stop : stopwatch.Start)();
        }

        /// <summary>
        /// Create a realtime clock.
        /// </summary>
        public RealtimeClock () => this.stopwatch = new Stopwatch();
        #endregion

        private readonly Stopwatch stopwatch;
    }
}