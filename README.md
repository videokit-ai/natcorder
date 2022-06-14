# NatCorder
NatCorder is a high performance, cross-platform video recording API for Unity Engine. First, create a recorder:
```csharp
// Create a recorder
var recorder = new GIFRecorder(
    640,  // image width
    480,  // image height
    0.1   // frame duration
);
```
Next, commit a bunch of video and/or audio frames to the recorder:
```csharp
// Commit video frames
Texture2D[] frames = ...;
foreach (var frame in frames)
    recorder.CommitFrame(frame.GetPixels32());
```
And when you're all done, finish writing and use the video as you desire:
```csharp
// Finish recording
string videoPath = await recorder.FinishWriting();
```


NatCorder comes with a rich featureset including:

+ Record any texture, anything that can be rendered into a texture, or any pixel data.
+ Record to MP4 videos and animated GIF images.
+ Control recording quality and file size with bitrate and keyframe interval.
+ Record at any resolution. You can specify what resolution recording you want.
+ Get the path to recorded video in device storage.
+ Record game audio with video.
+ Support for recording HEVC videos.
+ Support for Android, iOS, macOS, WebGL, and Windows.

See the [online docs](https://docs.natml.ai/natcorder) to learn more about NatCorder.

## Installing NatCorder
Using NatCorder requires an active [NatML Cloud](https://natml.ai/pricing) subscription. First, add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.natcorder": "1.8.3"
  }
}
```
Then retrieve your access key from [NatML Hub](https://hub.natml.ai/profile) and add it to your Project Settings:

![specifying your access key](.media/key.png)

___

## Requirements
- Unity 2020.3+
- Android API Level 24+
- iOS 13+
- macOS 10.15+ (Apple Silicon and Intel)
- Windows 10+, 64-bit only
- WebGL:
  - Firefox 25+
  - Chrome 47+
  - Safari 27+

## Resources
- Join the [NatML community on Discord](https://discord.gg/y5vwgXkz2f).
- See the [NatCorder documentation](https://docs.natml.ai/natcorder).
- Check out [NatML on GitHub](https://github.com/natmlx).
- Read the [NatML blog](https://blog.natml.ai/).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/natcorder-video-recording-api.505146/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!