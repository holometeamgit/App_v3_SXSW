/* 
*   NatMic
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatMic.Internal {

    using UnityEngine;
    using UnityEngine.Scripting;
    using System;

    public sealed class AudioDeviceAndroid : AudioDevice {

        #region --Intropection--

        public new static AudioDeviceAndroid[] GetDevices () {
            AudioDevice = AudioDevice ?? new AndroidJavaClass(@"com.olokobayusuf.natmic.AudioDevice");
            using (var devicesArray = AudioDevice.CallStatic<AndroidJavaObject>(@"devices")) {
                var devices = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(devicesArray.GetRawObject());
                var result = new AudioDeviceAndroid[devices.Length];
                for (var i = 0; i < devices.Length; i++)
                    result[i] = new AudioDeviceAndroid(devices[i]);
                return result;
            }
        }
        #endregion


        #region --AudioDevice--

        public override string Name {
            get => device.Call<string>(@"name");
        }

        public override string UniqueID {
            get => device.Call<string>(@"uniqueID");
        }

        public override bool EchoCancellation {
            get => device.Call<bool>(@"echoCancellation");
        }
        #endregion


        #region --IAudioDevice--

        public override bool IsRecording {
            get => device.Call<bool>(@"isRecording");
        }

        public override bool StartRecording (int sampleRate, int channelCount, IAudioProcessor processor) {
            this.processor = processor;
            return device.Call<bool>(@"startRecording", sampleRate, channelCount, new SampleBufferDelegate(this));
        }

        public override void StopRecording () {
            this.processor = null;
            device.Call(@"stopRecording");
        }
        #endregion


        #region --Operations--
        
        private readonly AndroidJavaObject device;
        private volatile IAudioProcessor processor;
        private static AndroidJavaClass AudioDevice;

        private AudioDeviceAndroid (AndroidJavaObject device) => this.device = device;

        private class SampleBufferDelegate : AndroidJavaProxy {

            private readonly AudioDeviceAndroid device;

            public SampleBufferDelegate (AudioDeviceAndroid device) : base(@"com.olokobayusuf.natmic.AudioDevice$Callback") => this.device = device;

            [Preserve]
            private void onSampleBuffer (AndroidJavaObject frame) {
                // Marshal sample buffer
                float[] sampleBuffer;
                using (var nativeSampleBuffer = frame.Get<AndroidJavaObject>(@"sampleBuffer"))
                    sampleBuffer = AndroidJNI.FromFloatArray(nativeSampleBuffer.GetRawObject());
                var sampleRate = frame.Get<int>(@"sampleRate");
                var channelCount = frame.Get<int>(@"channelCount");
                var timestamp = frame.Get<long>(@"timestamp");
                // Pass to processor
                try {
                    device.processor.OnSampleBuffer(sampleBuffer, sampleRate, channelCount, timestamp);
                } catch (Exception ex) {
                    Debug.LogError("NatMic Error: AudioDevice processor raised exception: "+ex);
                } finally {
                    frame.Dispose();
                }
            }
        }
        #endregion
    }
}