using YVR.AndroidDevice.Core;

namespace YVR.Player
{
    public class DreamPlayerElements : IAJCElements
    {
        public string className => "com.pfdm.playersdk.DreamPlayer";

        internal readonly string setMediaSource = "setMediaSource";
        internal readonly string setupVideoTexture = "setupVideoTexture";
        internal readonly string setPlaybackSpeed = "setPlaybackSpeed";
        internal readonly string getPlaybackSpeed = "getPlaybackSpeed";
        internal readonly string setLooping = "setLooping";
        internal readonly string isLooping = "isLooping";
        internal readonly string getCurrentPosition = "getCurrentPosition";
        internal readonly string getDuration = "getDuration";
        internal readonly string getVideoWidth = "getVideoWidth";
        internal readonly string getVideoHeight = "getVideoHeight";
        internal readonly string setVolume = "setVolume";

        internal readonly string getSubtitleTrackInfo = "getSubtitleTrackInfo";
        internal readonly string setSubtitleTrackInfo = "setSubtitleTrackInfo";

        internal readonly string getAudioTrackInfo = "getAudioTrackInfo";
        internal readonly string setAudioTrackInfo = "setAudioTrackInfo";

        internal readonly string prepareAsync = "prepareAsync";
        internal readonly string start = "start";
        internal readonly string pause = "pause";
        internal readonly string stop = "stop";
        internal readonly string release = "release";
        internal readonly string isPlaying = "isPlaying";
        internal readonly string seekTo = "seekTo";
        internal readonly string setListener = "setListener";
        internal readonly string isMuted = "isMuted";
        internal readonly string setMuted = "setMuted";
        internal readonly string getVolume = "getVolume";
        internal readonly string setColorSpace = "setColorSpace";

        #region not required to be used and concerned with

        // 应该永远使用 prepareAsync
        internal readonly string prepare = "prepare";

        // 接口未经验证，且没有应用场景
        internal readonly string setSize = "setSize";

        // java 侧会自己设置，不需要调用
        internal readonly string setVideoTexture = "setVideoTexture";

        // 由 native 部分触发
        internal readonly string updateTexImage = "updateTexImage";

        #endregion
    }
}