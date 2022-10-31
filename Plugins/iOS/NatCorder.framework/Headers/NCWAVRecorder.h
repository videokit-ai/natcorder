//
//  NCWAVRecorder.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 10/25/2022.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#include "NCMediaRecorder.h"

#pragma region --Client API--
/*!
 @function NCCreateWAVRecorder

 @abstract Create a WAV audio recorder.

 @discussion Create a WAV audio recorder.

 @param path
 Recording path. This path must be writable on the local file system.

 @param sampleRate
 Audio sample rate.

 @param channelCount
 Audio channel count.

 @param recorder
 Output media recorder.
*/
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCCreateWAVRecorder (
    const char* path,
    int32_t sampleRate,
    int32_t channelCount,
    NCMediaRecorder** recorder
);
#pragma endregion
