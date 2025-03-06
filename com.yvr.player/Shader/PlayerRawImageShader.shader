Shader "Player/OES_RawImage"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZTest Always
        Pass
        {
            GLSLPROGRAM
            #include "UnityStereoSupport.glslinc"
            #include "UnityStereoExtensions.glslinc"

            #ifdef VERTEX

            in vec4 _glesVertex;
            in vec4 _glesMultiTexCoord0;
            out vec2 convertedCoord;

            void main()
            {
                gl_Position = GetStereoMatrixVP(SetupStereoEyeIndex()) * (unity_ObjectToWorld * _glesVertex);
                convertedCoord = vec2(_glesMultiTexCoord0.x, (1.0 - _glesMultiTexCoord0.y));
            }

            #endif

            #ifdef FRAGMENT

            #extension GL_OES_EGL_image_external : require
            #extension GL_OES_EGL_image_external_essl3 : require

            in vec2 convertedCoord;
            uniform samplerExternalOES _MainTex;

            void main()
            {
                vec4 textureColor = texture2D(_MainTex, convertedCoord);
                gl_FragData[0] = vec4(pow(textureColor.rgb, vec3(2.2)), textureColor.a);
            }

            #endif

            ENDGLSL
        }
    }
}