using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YVR.Player
{
    public class DreamPlayerListener : AndroidJavaProxy
    {
        public DreamPlayerListener() : base("com.pfdm.playersdk.IDreamPlayerListener")
        {
        }

        public event Action<int> bufferingUpdated;

        public event Action<PlaybackState> playbackStateChanged;

        public event Action firstFrameRendered;

        public event Action frameAvailable;

        public event Action<PlaybackErrorCode> error;

        public event Action prepared;

        public event Action seekCompleted;

        public event Action<string> textOutputing;

        public event Action<AndroidJavaObject> imageOuputing;

        public event Action<int, int> videoSizeChanged;

        public event Action<int, int> videoTextureIdCreated;

        public event Action<int> colorSpaceChanged;

        /**
         * 资源缓存百分比发生改变
         *
         * @param percentage 缓存百分比,值为1-100
         */
        void onBufferingUpdated(int percentage)
        {
            Log(percentage);
            bufferingUpdated?.Invoke(percentage);
        }

        /**
         * 播放状态发生改变
         *
         * @param playbackState {STATE_IDLE=1, STATE_BUFFERING=2, STATE_READY=3, STATE_ENDED=4}
         */
        void onPlaybackStateChanged(int playbackState)
        {
            Log((PlaybackState)playbackState);
            playbackStateChanged?.Invoke((PlaybackState)playbackState);
        }

        /**
         * 播放错误码
         *
         * @param errorCode 参考com.pfdm.player.Playback.ErrorCode
         */
        void onPlayerError(int errorCode)
        {
            Log(errorCode);
            error?.Invoke((PlaybackErrorCode)errorCode);
        }

        /**
         * 资源准备完成
         */
        void onPrepared()
        {
            Log();
            prepared?.Invoke();
        }

        /**
         * 完成移动到指定时间点播放
         */
        void onSeekCompleted()
        {
            Log();
            seekCompleted?.Invoke();
        }

        /**
         * 输出一行字幕
         *
         * @param text 字幕字符串
         */
        void onTextOutput(string text)
        {
            Log(text);
            textOutputing?.Invoke(text);
        }

        /**
         * 输出一张位图字幕
         *
         * @param bitmap 位图字幕
         */
        void onImageOutput(AndroidJavaObject bitmap)
        {
            Log(bitmap);
            imageOuputing?.Invoke(bitmap);
        }

        /**
         * 视频尺寸发生改变
         *
         * @param width  宽度
         * @param height 高度
         */
        void onVideoSizeChanged(int width, int height)
        {
            Log($"{width},{height}");
            videoSizeChanged?.Invoke(width, height);
        }

        void onFrameAvailable()
        {
            Log();
            frameAvailable?.Invoke();
        }

        void onVideoTextureCreated(int textureId, int blackTextureId)
        {
            Log($"texture is: {textureId}, blackTextureId: {blackTextureId}");
            videoTextureIdCreated?.Invoke(textureId, blackTextureId);
        }

        void onRenderedFirstFrame()
        {
            Log();
            firstFrameRendered?.Invoke();
        }

        void onColorSpaceChanged(int colorSpace)
        {
            Log(colorSpace);
            colorSpaceChanged?.Invoke(colorSpace);
        }

        [Conditional("YVR_PLAYER_LOG")]
        private static void Log(object msg = null, [CallerMemberName] string method = null)
        {
            Log(msg?.ToString(), method);
        }

        [Conditional("YVR_PLAYER_LOG")]
        private static void Log(string msg, [CallerMemberName] string method = null)
        {
            UnityEngine.Debug.Log($"DreamPlayerLog[CB] {method}: {msg}");
        }
    }
}