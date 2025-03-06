using UnityEngine;
using UnityEngine.Serialization;

namespace YVR.Player
{
    public enum HDRType
    {
        Auto = -1,
        SDR,
        HDR10,
        HLG,
    }

    // TODO: Mock for Editor Env
    public class YPlayer : MonoBehaviour
    {
        [FormerlySerializedAs("m_materials")] [SerializeField]
        private Material[] m_TargetMaterials;

        public Material[] targetMaterials
        {
            get { return m_TargetMaterials; }
            set
            {
                m_TargetMaterials = value;
                if (m_DreamPlayer != null && texture != null)
                {
                    UpdateMaterial(m_TargetMaterials);
                }
            }
        }

        public Texture2D texture => m_DreamPlayer.texture;
        public Texture2D blackTexture => m_DreamPlayer.blackTexture;

        private DreamPlayer m_DreamPlayer;
        public DreamPlayerListener listener => m_DreamPlayer.listener;

        public void Init()
        {
            if (m_DreamPlayer == null)
            {
                m_DreamPlayer = new DreamPlayer();
            }
        }

        public void UpdateMaterial(Material[] mats)
        {
            if (mats == null) return;
            if (m_DreamPlayer == null) return;

            for (int i = 0; i < mats.Length; i++)
            {
                Material renderMaterial = mats[i];
                renderMaterial.mainTexture = texture;
            }
        }

        public void Release()
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.Release();
            m_DreamPlayer = null;
        }

        public void OpenMedia(string url, bool autoPlay = true)
        {
            m_DreamPlayer.SetMediaSource(url);
            m_DreamPlayer.PrepareAsync();
            if (autoPlay)
            {
                m_DreamPlayer.Start();
            }
        }

        public bool IsInitComplete()
        {
            return m_DreamPlayer is { texture: not null, blackTexture: not null };
        }

        public void CloseMedia()
        {
            m_DreamPlayer.CloseMedia();
        }

        public void SetLooping(bool isLooping)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetLooping(isLooping);
        }

        public bool IsLooping()
        {
            if (m_DreamPlayer == null) return false;
            return m_DreamPlayer.IsLooping();
        }

        public bool CanPlay()
        {
            if (m_DreamPlayer == null) return false;
            return m_DreamPlayer.prepared;
        }

        public bool IsPlaying()
        {
            if (m_DreamPlayer == null) return false;
            return m_DreamPlayer.IsPlaying();
        }

        public bool IsPaused()
        {
            return !IsPlaying();
        }

        public bool IsFinished()
        {
            if (m_DreamPlayer == null) return false;
            return m_DreamPlayer.playbackState == PlaybackState.Ended;
        }

        public void Play()
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.Start();
        }

        public void Pause()
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.Pause();
        }

        public void Stop()
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.Stop();
        }

        public void Rewind()
        {
            Seek(0);
            Play();
        }

        public void Seek(long time)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SeekTo(time);
        }

        public string[] GetSubtitleTrackInfo()
        {
            if (m_DreamPlayer == null) return null;
            return m_DreamPlayer.GetSubtitleTrackInfo();
        }

        public void SetSubtitleTrackInfo(int index)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetSubtitleTrackInfo(index);
        }

        public string[] GetAudioTrackInfo()
        {
            if (m_DreamPlayer == null) return null;
            return m_DreamPlayer.GetAudioTrackInfo();
        }

        public void SetAudioTrackInfo(int index)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetAudioTrackInfo(index);
        }

        public long GetCurrentTime()
        {
            if (m_DreamPlayer == null) return 0;
            return m_DreamPlayer.GetCurrentPosition();
        }

        public long GetDuration()
        {
            if (m_DreamPlayer == null) return 0;
            return m_DreamPlayer.GetDuration();
        }

        public int GetVideoWidth()
        {
            if (m_DreamPlayer == null) return -1;
            return m_DreamPlayer.GetVideoWidth();
        }

        public int GetVideoHeight()
        {
            if (m_DreamPlayer == null) return -1;
            return m_DreamPlayer.GetVideoHeight();
        }

        public float GetPlaybackSpeed()
        {
            if (m_DreamPlayer == null) return -1f;
            return m_DreamPlayer.GetPlaybackSpeed();
        }

        public void SetPlaybackSpeed(float rate)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetPlaybackSpeed(rate);
        }

        public void MuteAudio(bool bMute)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetMute(bMute);
        }

        public bool IsMuted()
        {
            if (m_DreamPlayer == null) return false;
            return m_DreamPlayer.IsMuted();
        }

        public void SetVolume(float volume)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetVolume(volume);
        }

        public float GetVolume()
        {
            if (m_DreamPlayer == null) return 0;
            return m_DreamPlayer.GetVolume();
        }

        public void SetColorSpace(int colorSpace)
        {
            if (m_DreamPlayer == null) return;
            m_DreamPlayer.SetColorSpace(colorSpace);
        }

        private void Update()
        {
            if (m_DreamPlayer == null) return;

            m_DreamPlayer.Update();
            UpdateMaterial(targetMaterials);
        }

        public string GetDebugInfo()
        {
            if (m_DreamPlayer == null) return "YPlayer not initialized.";

            return $"isPlaying: {IsPlaying()}\n" +
                   $"isPaused: {IsPaused()}\n" +
                   $"isFinished: {IsFinished()}\n" +
                   $"canPlay: {CanPlay()}\n" +
                   $"isLooping: {IsLooping()}\n" +
                   $"isMuted: {IsMuted()}\n" +
                   $"volume: {GetVolume()}\n" +
                   $"playbackSpeed: {GetPlaybackSpeed()}\n" +
                   $"currentTime: {GetCurrentTime()}\n" +
                   $"duration: {GetDuration()}\n" +
                   $"videoWidth: {GetVideoWidth()}\n" +
                   $"videoHeight: {GetVideoHeight()}";
        }
    }
}