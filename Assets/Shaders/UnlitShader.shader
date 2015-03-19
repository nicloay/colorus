Shader "Custom/UnlitShader" {
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}	
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members clipRectPos)
#pragma exclude_renderers d3d11 xbox360	
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;						
			
			struct appdata_t
			{
				float4 vertex    : POSITION;
				float2 texcoord  : TEXCOORD0;								
			}  ;

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;			
			}  ;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex   = mul(UNITY_MATRIX_MVP, v.vertex);				
				o.texcoord = v.texcoord;						
				return o;
				
			}

			half4 frag (v2f IN) : COLOR
			{
				half4 col     = tex2D(_MainTex, IN.texcoord);									
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
	
}