namespace YVR.Player
{
    public enum PlaybackErrorCode
    {
        // General errors (1xxx)
        /// <summary>
        /// Caused by an error whose cause could not be identified.
        /// </summary>
        Unspecified = 1000,

        /// <summary>
        /// Caused by an unidentified error in a remote Player, which is a Player that runs on a different host or process.
        /// </summary>
        RemoteError = 1001,

        /// <summary>
        /// Caused by the loading position falling behind the sliding window of available live content.
        /// </summary>
        BehindLiveWindow = 1002,

        /// <summary>
        /// Caused by a generic timeout.
        /// </summary>
        Timeout = 1003,

        /// <summary>
        /// Caused by a failed runtime check. This can happen when the application fails to comply with the player's API requirements (for example, by passing invalid arguments), or when the player reaches an invalid state.
        /// </summary>
        FailedRuntimeCheck = 1004,

        // Input/Output errors (2xxx)
        /// <summary>
        /// Caused by an Input/Output error which could not be identified.
        /// </summary>
        IoUnspecified = 2000,

        /// <summary>
        /// Caused by a network connection failure.
        /// Possible reasons include no network connectivity, misspelled domain, unreachable host, or closed connection.
        /// </summary>
        IoNetworkConnectionFailed = 2001,

        /// <summary>
        /// Caused by a network timeout, meaning the server is taking too long to fulfill a request.
        /// </summary>
        IoNetworkConnectionTimeout = 2002,

        /// <summary>
        /// Caused by a server returning a resource with an invalid "Content-Type" HTTP header value.
        /// For example, this can happen when the player is expecting a piece of media, but the server returns a paywall HTML page.
        /// </summary>
        IoInvalidHttpContentType = 2003,

        /// <summary>
        /// Caused by an HTTP server returning an unexpected HTTP response status code.
        /// </summary>
        IoBadHttpStatus = 2004,

        /// <summary>
        /// Caused by a non-existent file.
        /// </summary>
        IoFileNotFound = 2005,

        /// <summary>
        /// Caused by lack of permission to perform an IO operation.
        /// For example, lack of permission to access internet or external storage.
        /// </summary>
        IoNoPermission = 2006,

        /// <summary>
        /// Caused by the player trying to access cleartext HTTP traffic when the app's Network Security Configuration does not permit it.
        /// </summary>
        IoCleartextNotPermitted = 2007,

        /// <summary>
        /// Caused by reading data out of the data bound.
        /// </summary>
        IoReadPositionOutOfRange = 2008,

        // Content parsing errors (3xxx)
        /// <summary>
        /// Caused by a parsing error associated with a media container format bitstream.
        /// </summary>
        ParsingContainerMalformed = 3001,

        /// <summary>
        /// Caused by a parsing error associated with a media manifest.
        /// </summary>
        ParsingManifestMalformed = 3002,

        /// <summary>
        /// Caused by attempting to extract a file with an unsupported media container format, or an unsupported media container feature.
        /// </summary>
        ParsingContainerUnsupported = 3003,

        /// <summary>
        /// Caused by an unsupported feature in a media manifest.
        /// </summary>
        ParsingManifestUnsupported = 3004,

        // Decoding errors (4xxx)
        /// <summary>
        /// Caused by a decoder initialization failure.
        /// </summary>
        DecoderInitFailed = 4001,

        /// <summary>
        /// Caused by a decoder query failure.
        /// </summary>
        DecoderQueryFailed = 4002,

        /// <summary>
        /// Caused by a failure while trying to decode media samples.
        /// </summary>
        DecodingFailed = 4003,

        /// <summary>
        /// Caused by trying to decode content whose format exceeds the capabilities of the device.
        /// </summary>
        DecodingFormatExceedsCapabilities = 4004,

        /// <summary>
        /// Caused by trying to decode content whose format is not supported.
        /// </summary>
        DecodingFormatUnsupported = 4005,

        // AudioTrack errors (5xxx)
        /// <summary>
        /// Caused by an AudioTrack initialization failure.
        /// </summary>
        AudioTrackInitFailed = 5001,

        /// <summary>
        /// Caused by an AudioTrack write operation failure.
        /// </summary>
        AudioTrackWriteFailed = 5002,

        /// <summary>
        /// Caused by an AudioTrack write operation failure in offload mode. (Unstable API)
        /// </summary>
        AudioTrackOffloadWriteFailed = 5003,

        // DRM errors (6xxx)
        /// <summary>
        /// Caused by an unspecified error related to DRM protection.
        /// </summary>
        DrmUnspecified = 6000,

        /// <summary>
        /// Caused by a chosen DRM protection scheme not being supported by the device.
        /// </summary>
        DrmSchemeUnsupported = 6001,

        /// <summary>
        /// Caused by a failure while provisioning the device.
        /// </summary>
        DrmProvisioningFailed = 6002,

        /// <summary>
        /// Caused by attempting to play incompatible DRM-protected content.
        /// </summary>
        DrmContentError = 6003,

        /// <summary>
        /// Caused by a failure while trying to obtain a license.
        /// </summary>
        DrmLicenseAcquisitionFailed = 6004,

        /// <summary>
        /// Caused by an operation being disallowed by a license policy.
        /// </summary>
        DrmDisallowedOperation = 6005,

        /// <summary>
        /// Caused by an error in the DRM system.
        /// </summary>
        DrmSystemError = 6006,

        /// <summary>
        /// Caused by the device having revoked DRM privileges.
        /// </summary>
        DrmDeviceRevoked = 6007,

        /// <summary>
        /// Caused by an expired DRM license being loaded into an open DRM session.
        /// </summary>
        DrmLicenseExpired = 6008,

        // Frame processing errors (7xxx)
        /// <summary>
        /// Caused by a failure when initializing a VideoFrameProcessor. (Unstable API)
        /// </summary>
        VideoFrameProcessorInitFailed = 7000,

        /// <summary>
        /// Caused by a failure when processing a video frame. (Unstable API)
        /// </summary>
        VideoFrameProcessingFailed = 7001,

        /// <summary>
        /// Player implementations that want to surface custom errors can use error codes greater than this value.
        /// </summary>
        CustomBase = 1000000
    }
}