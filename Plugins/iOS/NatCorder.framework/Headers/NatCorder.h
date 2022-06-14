//
//  NatCorder.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 12/29/2019.
//  Copyright © 2022 NatML Inc. All Rights Reserved.
//

#pragma once

#include <stdint.h>

// Platform defines
#ifdef __cplusplus
    #define BRIDGE extern "C"
#else
    #define BRIDGE extern
#endif

#ifdef _WIN64
    #define EXPORT __declspec(dllexport)
#else
    #define EXPORT
    #define APIENTRY
#endif


#pragma region --Enumerations--
/*!
 @enum NCAppTokenStatus

 @abstract NatCorder application token status.

 @constant NC_APP_TOKEN_VALID
 NatCorder app token is valid

 @constant NC_APP_TOKEN_INVALID
 NatCorder app token is invalid.

 @constant NC_APP_TOKEN_HUB_MISSING
 The NatMLHub dependency library is missing.

 @constant NC_APP_TOKEN_HUB_INVALID
 The NatMLHub dependency library is invalid.
 */
enum NCAppTokenStatus {
    NC_APP_TOKEN_INVALID        = 0,
    NC_APP_TOKEN_VALID          = 1,
    NC_APP_TOKEN_HUB_MISSING    = 2,
    NC_APP_TOKEN_HUB_INVALID    = 3,
};
typedef enum NCAppTokenStatus NCAppTokenStatus;
#pragma endregion


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


#pragma region --NatML--
/*!
 @function NCSetAppToken
 
 @abstract Set the NatCorder app token.
 
 @discussion Set the NatCorder app token.
 
 @param token
 NatCorder app token.

 @returns App token status.
*/
BRIDGE EXPORT NCAppTokenStatus APIENTRY NCSetAppToken (const char* token);
#pragma endregion


#pragma region --IMediaRecorder--
/*!
 @function NCMediaRecorderFrameSize
 
 @abstract Get the recorder's frame size.
 
 @discussion Get the recorder's frame size.
 
 @param recorder
 Opaque handle to a created recorder.
 
 @param outWidth
 Frame width.
 
 @param outHeight
 Frame height.
 */
BRIDGE EXPORT void APIENTRY NCMediaRecorderFrameSize (
    NCMediaRecorder* recorder,
    int32_t* outWidth,
    int32_t* outHeight
);

/*!
 @function NCMediaRecorderCommitFrame

 @abstract Commit a video frame to the recording.

 @discussion Commit a video frame to the recording.
 
 @param recorder
 Opaque handle to a created recorder.

 @param pixelBuffer
 Pixel buffer containing a single video frame. The pixel buffer MUST be laid out in RGBA8888 order (32 bits per pixel).

 @param timestamp
 Frame timestamp. The spacing between consecutive timestamps determines the
 effective framerate of some recorders.
 */
BRIDGE EXPORT void APIENTRY NCMediaRecorderCommitFrame (
    NCMediaRecorder* recorder,
    const uint8_t* pixelBuffer,
    int64_t timestamp
);

/*!
 @function NCMediaRecorderCommitSamples

 @abstract Commit an audio frame to the recording.

 @discussion Commit an audio frame to the recording.

 @param recorder
 Opaque handle to a created recorder.

 @param sampleBuffer
 Sample buffer containing a single audio frame. The sample buffer MUST be in 32-bit PCM
 format, interleaved by channel for channel counts greater than 1.

 @param sampleCount
 Total number of samples in the sample buffer. This should account for multiple channels.

 @param timestamp
 Frame timestamp. The spacing between consecutive timestamps determines the
 effective framerate of some recorders.
*/
BRIDGE EXPORT void APIENTRY NCMediaRecorderCommitSamples (
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
 Opaque handle to a created recorder.

 @param completionHandler
 Function pointer to be invoked once recording is completed.

 @param context
 Context passed to completion handler. Can be `NULL`.
*/
BRIDGE EXPORT void APIENTRY NCMediaRecorderFinishWriting (
    NCMediaRecorder* recorder,
    NCRecordingHandler completionHandler,
    void* context
);
#pragma endregion


#pragma region --Constructors--
/*!
 @function NCCreateMP4Recorder

 @abstract Create an MP4 recorder.

 @discussion Create an MP4 recorder that records with the H.264 AVC codec.

 @param path
 Path to record media file. This path must be accessible and must be on the local file system.

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
 Opaque pointer to the created recorder.
*/
BRIDGE EXPORT void APIENTRY NCCreateMP4Recorder (
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

/*!
 @function NCCreateHEVCRecorder
 
 @abstract Create an HEVC recorder.
 
 @discussion Create an MP4 recorder that records with the H.265 HEVC codec.
 
 @param path
 Path to record media file. This path must be accessible and must be on the local file system.

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
 Opaque pointer to the created recorder.
 */
BRIDGE EXPORT void APIENTRY NCCreateHEVCRecorder (
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

/*!
 @function NCCreateGIFRecorder

 @abstract Create a GIF recorder.

 @discussion Create an animated GIF recorder. The generated GIF image will loop forever.

 @param path
 Path to record file. This path must be accessible and must be on the local file system.

 @param width
 Image width.

 @param height
 Image height.

 @param frameDuration
 Single frame duration in seconds.

 @param recorder
 Opaque pointer to the created recorder.
*/
BRIDGE EXPORT void APIENTRY NCCreateGIFRecorder (
    const char* path,
    int32_t width,
    int32_t height,
    float frameDuration,
    NCMediaRecorder** recorder
);

/*!
 @function NCCreateWEBMRecorder

 @abstract Create a WEBM video recorder.

 @discussion Create a WEBM video recorder.
 This is currently supported on WebGL only.

 @param path
 Recording directory. This path must be an accessible directory and must be on the local file system.

 @param width
 Video width.

 @param height
 Vide. height.

 @param frameRate
 Video frame rate.

 @param sampleRate
 Audio sample rate. Pass 0 if recording without audio.

 @param channelCount
 Audio channel count. Pass 0 if recording without audio.

 @param videoBitRate
 Video bit rate in bits per second.

 @param audioBitRate
 Audio bit rate in bits per second. Ignored if no audio format is provided.

 @param recorder
 Opaque pointer to the created recorder.
*/
BRIDGE EXPORT void APIENTRY NCCreateWEBMRecorder (
    const char* path,
    int32_t width,
    int32_t height,
    float frameRate,
    int32_t sampleRate,
    int32_t channelCount,
    int32_t videoBitRate,
    int32_t audioBitRate,
    NCMediaRecorder** recorder
);
#pragma endregion
