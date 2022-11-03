//
//  NCMP4Recorder.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 10/25/2022.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#include "NCMediaRecorder.h"

#pragma region --Client API--
/*!
 @function NCCreateMP4Recorder

 @abstract Create an MP4 recorder.

 @discussion Create an MP4 recorder that records with the H.264 AVC codec.

 @param path
 Recording path. This path must be writable on the local file system.

 @param width
 Video width.

 @param height
 Video height.

 @param frameRate
 Video frame rate.

 @param sampleRate
 Audio sample rate. Pass 0 if recording without audio.

 @param channelCount
 Audio channel count. Pass 0 if recording without audio.

 @param videoBitRate
 Video bit rate in bits per second.

 @param keyframeInterval
 Video keyframe interval in seconds.

 @param audioBitRate
 Audio bit rate in bits per second. Ignored if no audio format is provided.

 @param recorder
 Output media recorder.

 @returns Recorder creation status.
*/
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCCreateMP4Recorder (
    const char* path,
    int32_t width,
    int32_t height,
    float frameRate,
    int32_t sampleRate,
    int32_t channelCount,
    int32_t videoBitRate,
    int32_t keyframeInterval,
    int32_t audioBitRate,
    NCMediaRecorder** recorder
);
#pragma endregion
