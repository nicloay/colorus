Shader "Custom/MaskUV2ShaderVertexColor1" {
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}	
		_Color ("Main Color", Color) = (1,1,1,1)	
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
			half4 _Color;
			
			struct appdata_t
			{
				float4 vertex    : POSITION;
				half4  color     : COLOR;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				
			}  ;

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;			
				float2 texcoord1 : TEXCOORD1;
			}  ;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex   = mul(UNITY_MATRIX_MVP, v.vertex);				
				o.texcoord = v.texcoord;
				o.texcoord1= v.texcoord1;
				o.color    = v.color;			
				return o;
				
			}

			half4 frag (v2f IN) : COLOR
			{
				half4 col     = tex2D(_MainTex, IN.texcoord);					
				half4 maskCol = tex2D(_MainTex, IN.texcoord1);					
				if (maskCol.a != 0)					
					col = half4( lerp(col.rgb, col.rgb * _Color.rgb, col.a), col.a); 
					// try this  return half4( lerp(col, col* _Color, mask.r).rgb, col.a ); // clo
				col.a *= IN.color.a ;
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
	
}