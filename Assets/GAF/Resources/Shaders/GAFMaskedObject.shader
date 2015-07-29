Shader "GAF/GAFMaskedObject"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaskMap ("Mask Texture", 2D) = "white" {}
		_ColorMult("ColorMult",Color) = (1.0, 1.0, 1.0, 1.0 )
		_ColorShift("ColorShift",Color) = (0.0, 0.0, 0.0, 0.0 )
		_Alpha ("Alpha factor", Range(0.0,1.0)) = 1.0
	}
	
	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
   		Stencil {
		  	Comp Equal
		  	ZFail Zero
		  	Fail Zero
		  	Pass Keep

		}

		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		Cull Off
		Zwrite Off
		Lighting Off
		
		CGPROGRAM

		#pragma surface surf Unlit noambient vertex:vert
		
		#include "UnityCG.cginc"
		
		sampler2D 	_MainTex;
		sampler2D 	_MaskMap;
		
		float4x4 	_TransformMatrix;

		float4 		_ColorMult;
		float4 		_ColorShift;
		float 		_Alpha;		

		struct Input
		{
			float2 uv_MainTex;
			float2 maskUV;
		};
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 screenPos = ComputeScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
			o.maskUV = mul(_TransformMatrix, float4(screenPos.xy, 1.0f, 1.0f)).xy;
		}
		
		fixed4 LightingUnlit(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			fixed4 c;
	        c.rgb = s.Albedo; 
	        c.a = s.Alpha;
	        return c;
		}
		
		void surf (Input input, inout SurfaceOutput o)
		{			
			half4 mainColor	= tex2D(_MainTex, input.uv_MainTex );
			half4 maskColor = tex2D(_MaskMap, input.maskUV );
			
			o.Albedo = ( mainColor.rgb * _ColorMult.rgb + _ColorShift.rgb );
			o.Alpha  = ( mainColor.a   * _Alpha         + _ColorShift.a   ) * maskColor.a;
		}
		
		ENDCG
	}
	
	FallBack "Mobile/Particles/Aplha Blended"
}
