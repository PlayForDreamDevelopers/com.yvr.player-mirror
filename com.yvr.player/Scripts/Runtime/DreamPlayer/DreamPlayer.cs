using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using YVR.AndroidDevice.Core;

namespace YVR.Player
{
    public class DreamPlayer : AJCMgr<DreamPlayer, AJCMocker, DreamPlayerElements>
    {
        private enum GLEvent : byte
        {
            None = 0,
            SetupVideoTexture,
            UpdateTexImage,
        }

        [DllImport("playersdk")]
        private static extern IntPtr GetRenderEventFunc();

        [DllImport("playersdk")]
        private static extern byte CreatePlayer(IntPtr player, IntPtr playerWrapper);

        [DllImport("playersdk")]
        private static extern void DestroyPlayer(int playerId);

        private GCHandle m_ContainerGCHandle;

        private GCHandle containerGCHandle
        {
            get
            {
                if (!m_ContainerGCHandle.IsAllocated) m_ContainerGCHandle = GCHandle.Alloc(this);
                return m_ContainerGCHandle;
            }
        }

        private readonly byte m_PlayerID;
        private int m_VideoTextureId = -1;
        private int m_BlackTextureId = -1;

        public Texture2D texture { get; private set; }
        public Texture2D blackTexture { get; private set; }

        private bool m_NeedCreateTexture = false;

        public readonly DreamPlayerListener listener;

        public bool prepared { get; private set; }
        public PlaybackState playbackState { get; private set; }

        private readonly ConcurrentQueue<GLEvent> m_GLEvents = new();

        public DreamPlayer() : base(
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            listener = new DreamPlayerListener();
            listener.playbackStateChanged += state => { playbackState = state; };
            listener.videoTextureIdCreated += (textureId, blackTextureId) =>
            {
                m_VideoTextureId = textureId;
                m_BlackTextureId = blackTextureId;
                m_NeedCreateTexture = true;
            };
            SetListener(listener);

            m_PlayerID = CreatePlayer(ajcBase.objPtr, GCHandle.ToIntPtr(containerGCHandle));
            InvokeGLEvent(GLEvent.SetupVideoTexture);
            Log(m_PlayerID, $"created id is: {m_PlayerID}");
        }

        // 确保在主线程调用
        private void InvokeGLEvent(GLEvent glEvent)
        {
            // Log(m_PlayerID, $"glEvent: {glEvent}");
            m_GLEvents.Enqueue(glEvent);
        }

        internal void Update()
        {
            if (m_NeedCreateTexture)
            {
                texture = Texture2D.CreateExternalTexture(1, 1, TextureFormat.RGB24, false, false, (IntPtr)m_VideoTextureId);
                blackTexture = Texture2D.CreateExternalTexture(1, 1, TextureFormat.RGB24, false, false, (IntPtr)m_BlackTextureId);
                m_NeedCreateTexture = false;
            }

            InvokeGLEvent(GLEvent.UpdateTexImage);

            if (m_GLEvents.Count > 0)
            {
                if (m_GLEvents.TryDequeue(out var glEvent))
                {
                    // Log(m_PlayerID, $"dequeue glEvent: {glEvent}");
                    GL.IssuePluginEvent(GetRenderEventFunc(), m_PlayerID * 0X1000 + (byte)glEvent);
                }
            }
        }

        internal void SetMediaSource(string path)
        {
            CloseMedia();
            ajcBase.CallJNI(elements.setMediaSource, path);
            Log(m_PlayerID, path);
        }

        internal void CloseMedia()
        {
            playbackState = PlaybackState.None;
            prepared = false;
            Stop();
            Log(m_PlayerID);
        }

        internal void SetPlaybackSpeed(float speed)
        {
            ajcBase.CallJNI(elements.setPlaybackSpeed, speed);
            Log(m_PlayerID, speed);
        }

        internal float GetPlaybackSpeed()
        {
            var playbackSpeed = ajcBase.CallJNI<float>(elements.getPlaybackSpeed);
            Log(m_PlayerID, playbackSpeed);
            return playbackSpeed;
        }

        internal void SetLooping(bool looping)
        {
            Log(m_PlayerID, looping);
            ajcBase.CallJNI(elements.setLooping, looping);
        }

        internal bool IsLooping()
        {
            bool isLooping = ajcBase.CallJNI<bool>(elements.isLooping);
            Log(m_PlayerID, isLooping);
            return isLooping;
        }

        internal long GetCurrentPosition()
        {
            long position = ajcBase.CallJNI<long>(elements.getCurrentPosition);
            // Log(position);
            return position;
        }

        internal long GetDuration()
        {
            var duration = ajcBase.CallJNI<long>(elements.getDuration);
            // Log(duration);
            return duration;
        }

        internal int GetVideoWidth()
        {
            var width = ajcBase.CallJNI<int>(elements.getVideoWidth);
            // Log(width);
            return width;
        }

        internal int GetVideoHeight()
        {
            var height = ajcBase.CallJNI<int>(elements.getVideoHeight);
            // Log(height);
            return height;
        }

        internal void SetVolume(float volume)
        {
            ajcBase.CallJNI(elements.setVolume, volume);
            Log(m_PlayerID, volume);
        }

        internal float GetVolume()
        {
            var volume = ajcBase.CallJNI<float>(elements.getVolume);
            Log(m_PlayerID, volume);
            return volume;
        }

        internal string[] GetSubtitleTrackInfo()
        {
            var subtitles = ajcBase.CallJNI<string[]>(elements.getSubtitleTrackInfo);
            Log(m_PlayerID, subtitles);
            return subtitles;
        }

        internal string[] GetAudioTrackInfo()
        {
            var audioTracks = ajcBase.CallJNI<string[]>(elements.getAudioTrackInfo);
            Log(m_PlayerID, audioTracks);
            return audioTracks;
        }

        internal void SetSubtitleTrackInfo(int index)
        {
            ajcBase.CallJNI(elements.setSubtitleTrackInfo, index);
            Log(m_PlayerID, index);
        }

        internal void SetAudioTrackInfo(int index)
        {
            ajcBase.CallJNI(elements.setAudioTrackInfo, index);
            Log(m_PlayerID, index);
        }

        internal void PrepareAsync()
        {
            ajcBase.CallJNI(elements.prepareAsync);
            Log(m_PlayerID);
        }

        internal void Start()
        {
            ajcBase.CallJNI(elements.start);
            Log(m_PlayerID);
        }

        internal void Pause()
        {
            ajcBase.CallJNI(elements.pause);
            Log(m_PlayerID);
        }

        internal void Stop()
        {
            ajcBase.CallJNI(elements.stop);
            Log(m_PlayerID);
        }

        internal void Release()
        {
            ajcBase.CallJNI(elements.release);
            Log(m_PlayerID);
        }

        internal bool IsPlaying()
        {
            bool isPlaying = ajcBase.CallJNI<bool>(elements.isPlaying);
            // Log(isPlaying);
            return isPlaying;
        }

        internal void SeekTo(long positionMs)
        {
            ajcBase.CallJNI(elements.seekTo, positionMs);
            Log(m_PlayerID, positionMs);
        }

        internal bool IsMuted()
        {
            bool muted = ajcBase.CallJNI<bool>(elements.isMuted);
            Log(m_PlayerID, muted);
            return muted;
        }

        internal void SetMute(bool mute)
        {
            ajcBase.CallJNI(elements.setMuted, mute);
            Log(m_PlayerID, mute);
        }

        internal void SetColorSpace(int colorSpace)
        {
            ajcBase.CallJNI(elements.setColorSpace, colorSpace);
            Log(m_PlayerID, colorSpace);
        }

        private void SetListener(DreamPlayerListener dreamPlayerListener)
        {
            ajcBase.CallJNI(elements.setListener, dreamPlayerListener);
            Log(m_PlayerID, dreamPlayerListener);
        }

        ~DreamPlayer()
        {
            DestroyPlayer(m_PlayerID);
        }

        [Conditional("YVR_PLAYER_LOG")]
        private static void Log(int id, object msg = null, [CallerMemberName] string method = null)
        {
            Log(id, msg?.ToString(), method);
        }

        [Conditional("YVR_PLAYER_LOG")]
        private static void Log(int id, string msg, [CallerMemberName] string method = null)
        {
            UnityEngine.Debug.Log($"DreamPlayerLog [{id}] {method}: {msg}");
        }
    }
}