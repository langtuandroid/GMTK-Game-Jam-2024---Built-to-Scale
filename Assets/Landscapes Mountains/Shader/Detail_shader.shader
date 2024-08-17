Shader "Custom Material" {

Properties {

_Color ("Main Color", Color) = (1,1,1,1)
_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
_SpecMap ("Specular map", 2D) = "black" {}
_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
_DetailRange ("Detail Intensity", Range (0.8, 5)) = 2
_DetailMapScale ("Detail Scale", Range (0.0, 10)) = 1
_DetailMap ("Detail (RGB)", 2D) = "gray" {}
_BumpMap ("Normalmap", 2D) = "bump" {}
_DetailNormalOpacity ("Detail Normal Opacity", Range (0, 1)) = 1
_DetailNormalMap("Detail Normal Map", 2D) = "bump" {}

}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 400
	CGPROGRAM
	
	#pragma surface surf BlinnPhong
	#pragma target 3.0
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _SpecMap;
	sampler2D _DetailMap;
    sampler2D _DetailBumpMap;
    sampler2D _DetailNormalMap;

	float4 _Color;
	float _DetailNormalOpacity;
	float _Shininess;
	float _DetailRange;
	float _DetailMapScale;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv_SpecMap;
		float2 uv_DetailMap;
		float2 uv_DetailNormalMap;
	};

	void surf (Input IN, inout SurfaceOutput o) {
	float4 tex = tex2D(_MainTex, IN.uv_MainTex)* _Color;
	float4 specTex = tex2D(_SpecMap, IN.uv_SpecMap);
	float4 detail = tex2D(_DetailMap,IN.uv_DetailMap*_DetailMapScale);
	tex.rgb *= detail*_DetailRange;
	o.Albedo = tex.rgb;
	o.Gloss = specTex.r;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess * specTex.g;
	float4 sNormal = tex2D(_BumpMap, IN.uv_BumpMap);
	float4 dNormal = tex2D(_DetailNormalMap, IN.uv_DetailNormalMap);
	float4 fNormal = dNormal<=0.5 ? (sNormal*dNormal)*2 : 1-(2*(1-sNormal)*(1-dNormal));
    fNormal = lerp(sNormal, fNormal, _DetailNormalOpacity);
	o.Normal = UnpackNormal(fNormal);

	}
	ENDCG
}

FallBack "Curstom Material"
}