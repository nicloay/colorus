Shader "Custom/LinePointerShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (Alpha8)", 2D) = "white" {}				
	}

   SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Pass {
     CGPROGRAM //Shader Start, Vertex Shader named vert, Fragment shader named frag
     #pragma vertex vert
     #pragma fragment frag
     #include "UnityCG.cginc"
     //Link properties to the shader
     float4 _Color;
     sampler2D _MainTex;
     
     struct v2f 
     {
	     float4  pos : SV_POSITION;
	     float2  uv : TEXCOORD0;
     };

     float4 _MainTex_ST;

     v2f vert (appdata_base v)
     {
	     v2f o;
	     o.pos = mul (UNITY_MATRIX_MVP, v.vertex); //Transform the vertex position
	     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex); //Prepare the vertex uv
	     return o;
     }

     half4 frag (v2f i) : COLOR
     {
         float4 texcol = tex2D (_MainTex, i.uv); //base texture
         float4 result = float4(_Color[0],_Color[1],_Color[2],texcol[3]);
         return result;
     }

     ENDCG //Shader End
    }
   }
}


