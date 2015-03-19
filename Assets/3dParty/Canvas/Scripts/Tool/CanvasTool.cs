using System;
using UnityEngine;
using System.Collections.Generic;


public struct PixelCache{
	public int number;
	public int x;
	public int y;
	
	public PixelCache (int number, int x, int y)
	{
		this.number = number;
		this.x = x;
		this.y = y;
	}	
}

[Serializable]
public class CanvasTool {
	public Texture2D pointerTexture;
	public Texture2D pointerMaskTexture;
	public Texture2D lineTexture;
	public LineMeshType meshType;
	Color32[] _colors;
	
	public Color32[] colors{
		get {
			if (_colors == null)
				_colors = lineTexture.GetPixels32();
			return _colors;
		}
	} 

	int _lineWidth=-1;
	public int lineWidth{
		get{
			if (_lineWidth==-1)
				_lineWidth = lineTexture.width;
			return _lineWidth;
		}
	}

	int _lineHeight=-1;
	public int lineHeight{
		get{
			if (_lineHeight==-1)
				_lineHeight = lineTexture.height;
			return _lineHeight;
		}
	}



	public Dictionary<CanvasToolDirection, PixelCache[]> cache;//TODO read at start
	PixelCache[] cacheNorth;
	PixelCache[] cacheNorthEast;
	PixelCache[] cacheEast;
	PixelCache[] cacheSouthEast;
	PixelCache[] cacheSouth;
	PixelCache[] cacheSouthWest;
	PixelCache[] cacheWest;
	PixelCache[] cacheNorthWest;
	PixelCache[][][] _fullCache;

	public PixelCache[][][] fullCache{
		get {
			if (_fullCache == null)
				cacheDirections();
			return _fullCache;
		}
	}

	void cacheDirections(){
		cacheNorth     = getPixelNumbers(colors, lineWidth, lineHeight,  0,  1);
		cacheNorthEast = getPixelNumbers(colors, lineWidth, lineHeight,  1,  1);
		cacheEast      = getPixelNumbers(colors, lineWidth, lineHeight,  1,  0);
		cacheSouthEast = getPixelNumbers(colors, lineWidth, lineHeight,  1, -1);
		cacheSouth     = getPixelNumbers(colors, lineWidth, lineHeight,  0, -1);
		cacheSouthWest = getPixelNumbers(colors, lineWidth, lineHeight, -1, -1);
		cacheWest      = getPixelNumbers(colors, lineWidth, lineHeight, -1,  0);
		cacheNorthWest = getPixelNumbers(colors, lineWidth, lineHeight, -1,  1);
		_fullCache = new PixelCache[3][][];
		_fullCache[0]=new PixelCache[3][]{cacheSouthWest, cacheSouth, cacheSouthEast};
		_fullCache[1]=new PixelCache[3][]{cacheWest     , null      , cacheEast     };
		_fullCache[2]=new PixelCache[3][]{cacheNorthWest, cacheNorth, cacheNorthEast};
		
	}

	PixelCache[] getPixelNumbers(Color32[] origColors, int width, int height, int xOffset, int yOffset){
		int currentPosition, previousPosition;
		Color32 currentColor, previousColor;
		int xDiff,yDiff;
		List<PixelCache> resultPixelNumbers = new List<PixelCache>();
		for (int y = 0; y < height; y++) {
			yDiff = y + yOffset;
			if (yDiff < -1 || yDiff > height )
				continue;
			for (int x = 0; x < width; x++) {
				currentPosition = y*width + x;
				currentColor = origColors[currentPosition];
				if (currentColor.a==0)
					continue;
				xDiff = x + xOffset;
				if (xDiff < -1 || xDiff > width)
					continue;
				
				if (currentColor.a > 0 && ((xDiff == -1)||(yDiff==-1)||(xDiff==width)||(yDiff==height))){
					resultPixelNumbers.Add(new PixelCache(currentPosition,x,y));
					continue;
				}
				
				
				previousPosition = yDiff * width + xDiff;			
				previousColor = origColors[previousPosition];
				if (currentColor.a > previousColor.a)
					resultPixelNumbers.Add(new PixelCache(currentPosition,x,y));
			}
		}
		return resultPixelNumbers.ToArray();
	}

}
