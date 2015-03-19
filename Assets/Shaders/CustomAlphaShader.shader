Shader "Custom/CustomAlphaShader" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Main texture", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
		
	}

	Category {
	   Lighting On
	   ZWrite Off
	   Cull Back
	   Blend SrcAlpha OneMinusSrcAlpha
	   Tags {Queue=Transparent}
	   SubShader {
            Material {
	           Emission [_Color]
            }
            Pass {
            	AlphaTest Greater [_Cutoff]
	           SetTexture [_MainTex] {
	                  Combine Texture * Primary, Texture * Primary
                }
                        
           }
        } 
	}
}
