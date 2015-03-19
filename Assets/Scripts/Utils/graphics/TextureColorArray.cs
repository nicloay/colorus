using UnityEngine;
using System.Collections;

public class TextureColorArray {
	public int width;
	public int height;
	public Color32[] Colors;
	public Rect pixelRect;
	
	public TextureColorArray (int widht, int height, Color32[] colors)
	{
		if (colors.Length != (widht * height))
			throw new System.Exception("color size must be equal widht*height");
		this.width = widht;
		this.height = height;
		this.Colors = colors;
	}

}
