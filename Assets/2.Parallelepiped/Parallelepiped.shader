Shader "Custom/Parallelepiped"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [KeywordEnum(FRONT, BACK, RIGHT, LEFT, TOP, BOTTOM)] _FACES ("Faces", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma shader_feature _FACES_FRONT _FACES_BACK _FACES_RIGHT _FACES_LEFT _FACES_TOP _FACES_BOTTOM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 customUV;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void vert (inout appdata_full v, out Input o) 
        {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			#if defined(_FACES_FRONT) || defined(_FACES_BACK)
				o.customUV = v.vertex.xy;
            #elif defined(_FACES_RIGHT) || defined(_FACES_LEFT)
				o.customUV = v.vertex.zy;
            #elif defined(_FACES_TOP) || defined(_FACES_BOTTOM)
				o.customUV = v.vertex.xz;
			#endif
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.customUV) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
