Shader "LowPoly/Simple" 
{
	Properties
	{
		_MainTex ("Color Scheme", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Offset ("Offset", Range(0, 5)) = 0.0
		_Scale("Scale", Range(0, 1)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0

		struct Input
		{
			float3 color : COLOR;
		};		

		sampler2D _MainTex;
		fixed4 _Color;
		half _Offset;
		half _Scale;

		void vert (inout appdata_full v)
		{
			v.color = tex2Dlod(_MainTex, _Scale * v.texcoord + _Offset) * _Color;
        }

		half _Glossiness;
		half _Metallic;
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = IN.color;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
