//
//  NCNativeRecorder.h
//  NatCorder
//
//  Created by Yusuf Olokoba on 6/26/2018.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

@import AVFoundation;

typedef void (^NCRecordingCompletionBlock) (NSURL* _Nullable url);

@protocol NCNativeRecorder <NSObject>
@optional
@property (readonly) CMVideoDimensions frameSize;
- (void) commitFrame:(nonnull const uint8_t*) pixelBuffer timestamp:(int64_t) timestamp;
- (void) commitSamples:(nonnull const float*) sampleBuffer sampleCount:(int) sampleCount timestamp:(int64_t) timestamp;
@required
- (void) finishWritingWithCompletionHandler:(nonnull NCRecordingCompletionBlock) completionHandler;
@end

@interface NCMP4Recorder : NSObject <NCNativeRecorder>
- (nullable instancetype) initWithPath:(nonnull NSString*) path
    width:(int) width
    height:(int) height
    frameRate:(float) frameRate
    sampleRate:(int) sampleRate
    channelCount:(int) channelCount
    videoBitRate:(int) videoBitRate
    keyframeInterval:(int) keyframeInterval
    audioBitRate: (int) audioBitRate;
@end

@interface NCHEVCRecorder : NSObject <NCNativeRecorder>
- (nullable instancetype) initWithPath:(nonnull NSString*) path
    width:(int) width
    height:(int) height
    frameRate:(float) frameRate
    sampleRate:(int) sampleRate
    channelCount:(int) channelCount
    videoBitRate:(int) videoBitRate
    keyframeInterval:(int) keyframeInterval
    audioBitRate:(int) audioBitRate;
@end

@interface NCGIFRecorder : NSObject <NCNativeRecorder>
- (nullable instancetype) initWithPath:(nonnull NSString*) path
    width:(int) width
    height:(int) height
    frameDuration:(float) frameDuration;
@end
