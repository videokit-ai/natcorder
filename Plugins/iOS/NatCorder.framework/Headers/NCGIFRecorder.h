//
//  NCGIFRecorder.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 10/25/2022.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#include "NCMediaRecorder.h"

#pragma region --Client API--
/*!
 @function NCCreateGIFRecorder

 @abstract Create a GIF recorder.

 @discussion Create an animated GIF recorder.
 The generated GIF image will loop forever.

 @param path
 Recording path. This path must be writable on the local file system.

 @param width
 Image width.

 @param height
 Image height.

 @param delay
 Per-frame delay in seconds.

 @param recorder
 Output media recorder.

 @returns Recorder creation status.
*/
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCCreateGIFRecorder (
    const char* path,
    int32_t width,
    int32_t height,
    float delay,
    NCMediaRecorder** recorder
);
#pragma endregion
