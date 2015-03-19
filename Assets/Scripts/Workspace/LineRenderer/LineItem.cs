using UnityEngine;
using System.Collections;

public class LineItem {
	private Material        mat              ;
	public  Mesh            mesh             ;
	private LineMeshTypeInt lineMeshType     ;
	static Vector3 meshPosition = new Vector3(0,0,25);


	public LineItem (LineMeshTypeInt lineMeshType,  Material mat) {
		this.lineMeshType = lineMeshType;
		this.mat = mat;
	}
	int a;
	public void renderMesh (Camera cam) {
		Graphics.DrawMesh (mesh, meshPosition, Quaternion.identity, mat, 8, cam);
	}
	
	public void setPoints (IntVector2 pointA, IntVector2 pointB) {
		mesh = lineMeshType.GetLineMesh (pointA, pointB, mat.mainTexture.width, mat.mainTexture.height);
	}
	
	public void setPoint (IntVector2 point) {
		mesh = lineMeshType.GetPointMesh(point,mat.mainTexture.width, mat.mainTexture.height);
	}
}
