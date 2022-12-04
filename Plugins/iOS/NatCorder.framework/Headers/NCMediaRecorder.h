//
//  NCMediaRecorder.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 10/25/2022.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#include <stdint.h>
#include "NCMediaSession.h"

#pragma region --Types--
/*!
 @struct NCMediaRecorder
 
 @abstract Media recorder.

 @discussion Media recorder.
*/
struct NCMediaRecorder;
typedef struct NCMediaRecorder NCMediaRecorder;

/*!
 @abstract Callback invoked with path to recorded media file.
 
 @param context
 User context provided to recorder.
 
 @param path
 Path to record file. If recording fails for any reason, this path will be `NULL`.
*/
typedef void (*NCRecordingHandler) (void* context, const char* path);
#pragma endregion


#pragma region --Client API--
/*!
 @function NCMediaRecorderGetFrameSize
 
 @abstract Get the recorder's frame size.
 
 @discussion Get the recorder's frame size.
 
 @param recorder
 Media recorder.
 
 @param outWidth
 Output frame width.
 
 @param outHeight
 Output frame height.
 */
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCMediaRecorderGetFrameSize (
    NCMediaRecorder* recorder,
    int32_t* outWidth,
    int32_t* outHeight
);

/*!
 @function NCMediaRecorderCommitFrame

 @abstract Commit a video frame to the recording.

 @discussion Commit a video frame to the recording.
 
 @param recorder
 Media recorder.

 @param pixelBuffer
 Pixel buffer containing a single video frame.
 The pixel buffer MUST be laid out in RGBA8888 order (32 bits per pixel).

 @param timestamp
 Frame timestamp. The spacing between consecutive timestamps determines the
 effective framerate of some recorders.
 */
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCMediaRecorderCommitFrame (
    NCMediaRecorder* recorder,
    const uint8_t* pixelBuffer,
    int64_t timestamp
);

/*!
 @function NCMediaRecorderCommitSamples

 @abstract Commit an audio frame to the recording.

 @discussion Commit an audio frame to the recording.

 @param recorder
 Media recorder.

 @param sampleBuffer
 Sample buffer containing a single audio frame. The sample buffer MUST be in 32-bit PCM
 format, interleaved by channel for channel counts greater than 1.

 @param sampleCount
 Total number of samples in the sample buffer. This should account for multiple channels.

 @param timestamp
 Frame timestamp. The spacing between consecutive timestamps determines the
 effective framerate of some recorders.
*/
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCMediaRecorderCommitSamples (
    NCMediaRecorder* recorder,
    const float* sampleBuffer,
    int32_t sampleCount,
    int64_t timestamp
);

/*!
 @function NCMediaRecorderFinishWriting

 @abstract Finish writing and invoke the completion handler.

 @discussion Finish writing and invoke the completion handler. The recorder is automatically
 released, along with any resources it owns. The recorder MUST NOT be used once this function
 has been invoked.
 
 The completion handler will be invoked soon after this function is called. If recording fails for any reason,
 the completion handler will receive `NULL` for the recording path.

 @param recorder
 Media recorder.

 @param handler
 Recording completion handler invoked once recording is completed.

 @param context
 Context passed to completion handler. Can be `NULL`.
*/
NML_BRIDGE NML_EXPORT NCMediaStatus NML_API NCMediaRecorderFinishWriting (
    NCMediaRecorder* recorder,
    NCRecordingHandler handler,
    void* context
);
#pragma endregion
