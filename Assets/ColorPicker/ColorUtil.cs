using UnityEngine;
using System.Collections;

namespace colorpicker{
	public class ColorUtil : MonoBehaviour {		
		private static float lastH=0f;
		
		// also read here http://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
		public static Color32 getRandomColor () {	
			float r = Random.Range(0.2f, 0.8f);
			lastH += r;
			lastH %= 1;		
			return hsvToRgb(lastH , Random.Range(0.8f,1.0f) ,1.0f );	
		}
		
		// h - 0->1
		// s  0->1
		// v  0-> 1
		
		public static Color32 hsvToRgb(HSVColor hsvColor){
			return hsvToRgb(hsvColor.h,hsvColor.s,hsvColor.v);
		}
		
		public static Color32 hsvToRgb (float h, float s, float v) {
			
			int H = (int)(h * 6);
			
			float f = h * 6 - H;
			float p = v * (1 - s);
			float q = v * (1 - f * s);
			float t = v * (1 - (1 - f) * s);
			
			float r=0;
			float g=0;
			float b=0;
			
			switch (H) {
			case 0:
				r = v; g = t; b = p;
				break;	
			case 1:
				r = q; g = v; b = p;
				break;
			case 2:
				r = p; g = v; b = t;
				break;
			case 3:
				r = p; g = q; b = v;
				break;
			case 4:
				r = t; g = p; b = v;
				break;
			case 5:
				r = v; g = p; b = q;
				break;
			}
			return new UnityEngine.Color32((byte)(255*r),(byte)(255*g),(byte)(255*b),255);
		}
		
		public static HSVColor rgbToHsv(Color32 color){
			HSVColor result=new HSVColor();
			float min,max,delta;
			float r = (float)color.r/255.0f;
			float g = (float)color.g/255.0f;
			float b = (float)color.b/255.0f;
			
			min = Mathf.Min(r,g,b);
			max = Mathf.Max(r,g,b);
			result.v = max;
			delta = max - min;
			
			if( max != 0 )
				result.s = delta / max;		// s
			else {
				// r = g = b = 0		// s = 0, v is undefined
				result.s = 0;
				result.h = -1;
				return result;
			}
			if( r == max )
				result.h = ( g - b ) / delta;		// between yellow & magenta
			else if( g == max )
				result.h = 2 + ( b - r ) / delta;	// between cyan & yellow
			else
				result.h = 4 + ( r - g ) / delta;	// between magenta & cyan
			result.h *= 60;				// degrees
			if( result.h < 0 )
				result.h += 360;
			result.h /=360;
			return result;			
		}		
		
		// Unity EditorGUIUtility.RGBToHSV copy
		public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V){
			if ((double) rgbColor.b > (double) rgbColor.g && (double) rgbColor.b > (double) rgbColor.r)
				RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
			else if ((double) rgbColor.g > (double) rgbColor.r)
				RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
			else
				RGBToHSVHelper(0.0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
		}

	    static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V){
			V = dominantcolor;
			if ((double) V != 0.0){
				float num1 = (double) colorone <= (double) colortwo ? colorone : colortwo;
				float num2 = V - num1;
				if ((double) num2 != 0.0){
				  S = num2 / V;
				  H = offset + (colorone - colortwo) / num2;
				} else {
				  S = 0.0f;
				  H = offset + (colorone - colortwo);
				}
				H = H / 6f;
				if ((double) H >= 0.0)
					return;
				H = H + 1f;
			} else {
				S = 0.0f;
				H = 0.0f;
			}
		}
		
	}
}