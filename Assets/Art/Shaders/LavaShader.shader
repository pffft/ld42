Shader "Custom/LavaShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _FlowMap ("Flow (RG)", 2D) = "black" {}
        _FlowSpeed ("Flow Speed", Range(0,1)) = 0.03
        _ZoomScale ("Zoom Scale", Range(0, 2)) = 0.04
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows    
		#pragma target 3.0

		sampler2D _MainTex, _FlowMap;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        float _FlowSpeed;
        float _ZoomScale;
        
        float2 FlowUV(float2 uv, float2 flowVector, float time) {
            float progress = frac(time * _FlowSpeed);
            return uv - flowVector * progress;
        }
        
        float2 FlowOffset(float2 uv, float time) {
            float2 offset = _FlowSpeed * float2(cos(time), sin(time));
            float scale = 1.0 + (_ZoomScale * cos(time));
            return 0.5 + (scale * (uv - 0.5)) + offset;
        } 
        
		void surf (Input IN, inout SurfaceOutputStandard o) {
            //float2 flowVector = tex2D(_FlowMap, IN.uv_MainTex).rg * 2 - 1;
            //float2 uv = FlowUV(IN.uv_MainTex, flowVector, _Time.y);
            float2 uv = FlowOffset(IN.uv_MainTex, _Time.y);
			fixed4 c = tex2D (_MainTex, uv) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
