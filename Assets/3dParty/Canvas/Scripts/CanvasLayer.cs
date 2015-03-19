using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class CanvasLayer {
	static TextureFormat textureFormat = TextureFormat.ARGB32;

	protected bool enabled;
	protected Mesh mesh;
	protected Texture2D texture;
	protected Vector3 position;
	protected IntVector2 size;
	protected Quaternion quaternion;
	protected Material material;
	protected Camera camera;
	public CanvasLayer(IntVector2 size, float zPosition, Shader shader, CanvasCamera canvasCamera){
		enabled = false;
		camera = canvasCamera.camera;
		material = new Material(shader);
		quaternion = Quaternion.identity;
		this.size = new IntVector2( size);
		mesh = MeshUtil.createPlaneMesh(size);
		position = new Vector3(0,0,zPosition);
	}

	public void setTexture(Texture2D texture){
		this.texture = texture;
		material.mainTexture = texture;
		enabled = texture!=null;
	}

	public Texture2D getTexture(){
		return texture;
	}

	public Texture2D setBlank(Color32[] colors){
		if (texture == null){
			this.texture = new Texture2D(size.x, size.y, textureFormat, false);
			this.texture.filterMode = FilterMode.Point;
		}
		material.mainTexture = texture;
		texture.SetPixels32(colors);
		texture.Apply();
		enabled = true;
		return texture;
	}

	public IEnumerator updateColors(Color32[] colors, bool onNextFrame=false){
		if (onNextFrame)
			yield return null;		

		if (texture == null){
			setBlank(colors);
		} else {
			texture.SetPixels32(colors);
			texture.Apply();
		}
	}

	public virtual void render(){
		if (enabled)
			Graphics.DrawMesh (mesh, position, quaternion, material, 8, camera);
	}
}
