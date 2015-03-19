using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CanvasRadialLayer : CanvasLayer {
	static TextureFormat textureFormat = TextureFormat.ARGB32;
	public bool isInProgress{
		get{
			return fillAnimate;
		}
	}
	Texture radialTexture;
	float animationStartTime;
	bool fillAnimate;
	float duration = 1.6f;
	static int FILL_VALUE_ID = Shader.PropertyToID("_FillValue");

	public CanvasRadialLayer(IntVector2 size, float zPosition, Shader shader, Texture radialTexture, CanvasCamera canvasCamera)
				:base(size,zPosition, shader, canvasCamera){
		this.radialTexture = radialTexture;
		material.SetTexture("_FillTex", radialTexture);
		enabled = false;
		this.texture = new Texture2D(size.x,size.y, textureFormat,false);
		material.mainTexture = texture;
	}

	public IEnumerator updateColorsAndAnimate(Color32[] colors, IntVector2 center){
		setParameters(center);
		texture.SetPixels32(colors);
		texture.Apply();
		updateAnimationVars();
		enabled = true;
		while(fillAnimate){
			updateAnimationVars();
			yield return null;
		}
		enabled = false;
	}


	void setParameters (IntVector2 center) {
		float maxX = Mathf.Max(center.x, size.x - center.x);
		float maxY = Mathf.Max(center.y, size.y - center.y);
		
		Vector2 v1 = getUvPoint(maxX, maxY);
		float ratioX = (size.x  - maxX) / maxX;
		float ratioY = (size.y - maxY) / maxY;
		
		Vector2 v2 = new Vector2(v1.x * ratioX, v1.y * ratioY);
		
		float x0, x1, y0, y1;
		if (center.x == (int)maxX){
			x0 = 0.5f - v1.x;
			x1 = 0.5f + v2.x;
		} else {
			x0 = 0.5f - v2.x;
			x1 = 0.5f + v1.x;
		}
		
		if (center.y == (int)maxY){
			y0 = 0.5f - v1.y;
			y1 = 0.5f + v2.y;
		} else {
			y0 = 0.5f - v2.y;
			y1 = 0.5f + v1.y;
		}
		Vector2[] uv1 = new Vector2[6];
		uv1[0] =          new Vector2(x0, y0);
		uv1[1] = uv1[5] = new Vector2(x0, y1);
		uv1[2] = uv1[3] = new Vector2(x1, y0);
		uv1[4] =          new Vector2(x1, y1);
		

		mesh.uv1 = uv1;
		
		fillAnimate = true;
		animationStartTime = Time.realtimeSinceStartup;
	}

	Vector2 getUvPoint(float x, float y){
		float diag=Mathf.Sqrt(x * x + y * y);
		return new Vector2(0.5f * x / diag, 0.5f * y / diag);
	}

	void updateAnimationVars () {
		float value = 1 - InterpolationUtil.moveInLinear(Time.realtimeSinceStartup - animationStartTime, 0,duration, 1);
		if (value <= 0){
			value = 0;
			fillAnimate = false;
		}

		material.SetFloat(FILL_VALUE_ID, value);
	}

	override public void render(){
		if (enabled)
			Graphics.DrawMesh (mesh, position, quaternion, material, 8, camera);
	}
}
