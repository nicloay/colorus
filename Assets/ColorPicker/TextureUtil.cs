using UnityEngine;
using System.Collections;

//todo don't forget to remove it
using System.IO;
namespace colorpicker{
	public class TextureUtil {
		
		public static Texture2D createEmptyTexture(int width, int height, Color32 color){
			int size=width*height;
			Texture2D texture = new Texture2D(width,height, TextureFormat.ARGB32, false);
			Color32[] pixels = new Color32[size];
			for (int i = 0; i < size; i++) {
				pixels[i]=color;				
			}
			texture.SetPixels32(pixels);
			texture.Apply();
			return texture;
		}
		
		public static Texture2D createFrame(int width, int height, int borderSize, Color32 borderColor, Color32 innerColor){
			int size=width * height;
			Texture2D texture =  new Texture2D(width,height);
			Color32[] pixels = new Color32[size];
			int firstRowEnd  =        width * borderSize;
			int lastRowStart = size - width * borderSize;
			int leftBound = borderSize;
			int rightBound = width - borderSize;		
			int remainder;
			for (int i = 0; i < pixels.Length; i++) {
				if (i<firstRowEnd || i > lastRowStart)
					pixels[i]=borderColor;
				else {
					remainder = i % width;
					if (remainder < leftBound || remainder >= rightBound)
						pixels[i]=borderColor;
					else
						pixels[i]=innerColor;
				}
			}		
			texture.SetPixels32(pixels);
			texture.Apply();		
			
			return texture;
		}		
	}	
}
