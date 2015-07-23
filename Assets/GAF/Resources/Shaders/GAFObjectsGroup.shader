Shader "GAF/GAFObjectsGroup" 
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
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
		

		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		Cull Off
		Zwrite Off
		Lighting Off
	
   		
   		Stencil {
		  	Comp Equal
		  	ZFail Zero
		  	Fail Zero
		  	Pass Keep

		}

	
		CGPROGRAM
				
		#pragma surface surf Unlit noambient vertex:vert
		#pragma glsl_no_auto_normalization
				
		#include "UnityCG.cginc"
	
		sampler2D _MainTex;
		float 		_Alpha;		
				
		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
			fixed4 colorShift;
		};
	
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
			o.colorShift = v.tangent;
		}
				
		fixed4 LightingUnlit(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			fixed4 c;
			c.rgb = s.Albedo; 
			c.a = s.Alpha * _Alpha;
			return c;
		}

		void surf (Input input, inout SurfaceOutput o)
		{
			fixed4 mainColor = tex2D(_MainTex, input.uv_MainTex );

			o.Albedo = mainColor.rgb * input.color.rgb + input.colorShift.rgb;
			o.Alpha  = mainColor.a   * input.color.a   + input.colorShift.a;
		}

		ENDCG 
	}
	
	Fallback "GAF/GAFObjectsGroupFallback"
}
