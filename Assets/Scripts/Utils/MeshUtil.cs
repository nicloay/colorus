using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class MeshUtil : MonoBehaviour {
	public static string ASSET_MESH_HOME = "Assets/Resources/Meshes/";
	
	public static GameObject createPlane (float width, float height, string name) {
		GameObject go = new GameObject ();		
		go.AddComponent (typeof(MeshRenderer));
		MeshFilter meshFilter = (MeshFilter)go.AddComponent (typeof(MeshFilter));
		
		Mesh mesh = createPlaneMesh (width, height);
		if(Application.isPlaying)
			meshFilter.mesh = mesh;
		else
			meshFilter.sharedMesh = mesh;
	
#if UNITY_EDITOR			
		saveSharedMeshAsset (name, mesh);		
#endif				
		
		return go;
	}

	public static Mesh createPlaneMesh(IntVector2 size){
		return createPlaneMesh((float)size.x, (float)size.y);
	}

	public static Mesh createPlaneMesh(Vector2 size){
		return createPlaneMesh(size.x, size.y);
	}

	public static Mesh createPlaneMesh (float width, float height) {
		Mesh mesh = new Mesh ();
		Vector3 point = Vector3.zero;
		point.z = Z_POSITION;
		Vector3[] vertices = new Vector3[6];
		int[] triangles = new int[6]{0,1,2,3,5,4};
		Vector2[] uvs = new Vector2[6]{
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2 (1, 0),
			new Vector2 (1, 0),
			new Vector2 (1, 1),
			new Vector2 (0, 1)			
		};
		point.x -= width / 2;
		point.y -= height / 2;
		vertices [0] = point;
		
		point.y += height;
		vertices [1] = point;
		
		point.x += width;
		point.y -= height;
		vertices [2] = point;
		vertices [3] = point; 
		point.y += height;
		vertices [4] = point;
		point.x -= width;
		vertices [5] = point;
		
		
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		return mesh;
	}
	
	public static Mesh createTriangleSideLineMesh (IntVector2 point1, IntVector2 point2, float sizeX, float sizeY) {
		Mesh mesh = new Mesh ();
		float halfSizeX = sizeX / 2;
		float halfSizeY = sizeY / 2;		
		IntVector2 leftPoint;
		IntVector2 rightPoint;
		if (point1.x < point2.x) {
			leftPoint = point1;
			rightPoint = point2;
		} else {
			rightPoint = point1;
			leftPoint = point2;
		}
		Vector3[] vertices = new Vector3[6];
		int    [] triangles;
		Vector2[] uvs = new Vector2[6];
		
		for (int i = 0; i < 6; i++) {
			vertices [i].z  = Z_POSITION;
		}
		
		
		
		vertices [0].x = vertices [1].x = leftPoint.x  - halfSizeX;
		vertices [2].x =                  leftPoint.x  + halfSizeX;
		vertices [3].x = vertices [5].x = rightPoint.x + halfSizeX;
		vertices [4].x =                  rightPoint.x - halfSizeX;
		
		if (leftPoint.y < rightPoint.y) {
			vertices [0].y = vertices [2].y = leftPoint.y  - halfSizeY;
			vertices [1].y =                  leftPoint.y  + halfSizeY;
			vertices [3].y = vertices [4].y = rightPoint.y + halfSizeY;
			vertices [5].y =                  rightPoint.y - halfSizeY;			
			triangles = new int[12]{0, 1, 2, 
						2, 1, 4, 
						4, 5, 2, 
						4, 3, 5};
			uvs[0] =          new Vector2(0, 0);
			uvs[1] = uvs[4] = new Vector2(0, 1);
			uvs[2] = uvs[5] = new Vector2(1, 0);
			uvs[3] =          new Vector2(1, 1);
		} else {
			vertices [0].y = vertices [2].y = leftPoint.y  + halfSizeY;
			vertices [1].y =                  leftPoint.y  - halfSizeY;
			vertices [3].y = vertices [4].y = rightPoint.y - halfSizeY;
			vertices [5].y =                  rightPoint.y + halfSizeY;
			triangles = new int[12]{0, 2, 1, 
						2, 4, 1, 
						2, 5, 4, 
						5, 3, 4};
			uvs[0] =          new Vector2(0, 1);
			uvs[1] = uvs[4] = new Vector2(0, 0);
			uvs[2] = uvs[5] = new Vector2(1, 1);
			uvs[3] =          new Vector2(1, 0);
		}
		float screenOffsetX = PropertiesSingleton.instance.width / 2;
		float screenOffsetY = PropertiesSingleton.instance.height / 2;
		
		for (int i = 0; i < 6; i++) {
			vertices[i].x = vertices [i].x - screenOffsetX;
			vertices[i].y = vertices [i].y - screenOffsetY;			
		}
		
		mesh.vertices  = vertices  ;
		mesh.uv        = uvs       ;
		mesh.triangles = triangles ;
		mesh.RecalculateBounds  () ;
		mesh.RecalculateNormals () ;
			
		return mesh;		
	}
	
	public static void setVertexColor(Mesh mesh,Color32 color){
		Color32[] colors= new Color32[ mesh.vertices.Length];
		for (int i = 0; i < colors.Length; i++) {
			colors[i] = color;
		}
		mesh.colors32= colors;
	}
	
	/*
	 * mesh vertex indexes
	 *  2,3		4,8,9		10,14,15	16
	 * ____________
	 * |  |\  |\  |
	 * |\ x \ | \ |
	 * | \   \   \
	 * ____________
	 * x - center
	 * 
	 * 	1		0,5,7		6,11,13		12,17
	 * 
	 */

	static float Z_POSITION = 0.0f;
	public static Mesh createSquareSideLineMesh (IntVector2 point1, IntVector2 point2, float width) {	
		float offset = width / 2;
		float lineLength = point1.lengthTo (point2);
	
		float cosA = ((float)(point2.x - point1.x)) / lineLength;
		float sinA = ((float)(point2.y - point1.y)) / lineLength;
		
		Mesh mesh = new Mesh ();					
		Vector3[] vertices = new Vector3[18];
		Vector2[] uvs = new Vector2[18];
		int[] triangles = new int[18]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17};
		
		uvs [0] = uvs [5] = uvs [7] = uvs [6] = uvs [11] = uvs [13] = new Vector2 (0.5f, 0.0f);
		uvs [4] = uvs [8] = uvs [9] = uvs [10] = uvs [14] = uvs [15] = new Vector2 (0.5f, 1.0f);
		uvs [1] = new Vector2 (0.0f, 0.0f);
		uvs [16] = new Vector2 (1.0f, 1.0f);
		uvs [2] = uvs [3] = new Vector2 (0.0f, 1.0f);
		uvs [12] = uvs [17] = new Vector2 (1.0f, 0.0f);
		
			
		vertices [2] = vertices [3] = new Vector3 (-offset, offset, Z_POSITION);
		vertices [4] = vertices [8] = vertices [9] = new Vector3 (0.0f, offset, Z_POSITION);
		vertices [10] = vertices [14] = vertices [15] = new Vector3 (lineLength, offset, Z_POSITION);
		vertices [16] = new Vector3 (lineLength + offset, offset, Z_POSITION);
		vertices [1] = new Vector3 (-offset, -offset, Z_POSITION);
		vertices [0] = vertices [5] = vertices [7] = new Vector3 (0.0f, -offset, Z_POSITION);
		vertices [6] = vertices [11] = vertices [13] = new Vector3 (lineLength, -offset, Z_POSITION);
		vertices [12] = vertices [17] = new Vector3 (lineLength + offset, -offset, Z_POSITION);
		
		float screenOffsetX = PropertiesSingleton.instance.width / 2;
		float screenOffsetY = PropertiesSingleton.instance.height / 2;
		for (int i = 0; i < 18; i++) {
			float newX = vertices [i].x * cosA - vertices [i].y * sinA;
			float newY = vertices [i].x * sinA + vertices [i].y * cosA;			
			vertices [i].x = newX + point1.x - screenOffsetX;
			vertices [i].y = newY + point1.y - screenOffsetY;
			vertices [i].z = Z_POSITION;
		}
		
		
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
			
		return mesh;
	}
	
	public static Mesh createLineMeshPoint (IntVector2 point, float width, float height) {	
		float offsetX = width / 2;
		float offsetY = height / 2;
		Mesh mesh = new Mesh ();					
		Vector3[] vertices = new Vector3[6];
		int[] triangles = new int[6]{0,1,2,3,4,5};
		Vector2[] uvs = new Vector2[6];
		uvs [0] = uvs [3] = new Vector2 (1.0f, 0.0f);
		uvs [2] = uvs [4] = new Vector2 (0.0f, 1.0f);
		uvs [1] = new Vector2 (0.0f, 0.0f);
		uvs [5] = new Vector2 (1.0f, 1.0f);
		
			
		vertices [0] = vertices [3] = new Vector3 ( offsetX, -offsetY, Z_POSITION);
		vertices [1] =                new Vector3 (-offsetX, -offsetY, Z_POSITION);
		vertices [2] = vertices [4] = new Vector3 (-offsetX,  offsetY, Z_POSITION);
		vertices [5] =                new Vector3 ( offsetX,  offsetY, Z_POSITION);
		
		float screenOffsetX = PropertiesSingleton.instance.width / 2;
		float screenOffsetY = PropertiesSingleton.instance.height / 2;
		for (int i = 0; i < 6; i++) {
			vertices [i].x = vertices [i].x + point.x - screenOffsetX;
			vertices [i].y = vertices [i].y + point.y - screenOffsetY;
			vertices [i].z = Z_POSITION;
		}
		
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
			
		return mesh;
	}
	
	
	
#if UNITY_EDITOR	
	public static void saveSharedMeshAsset (string name, Mesh mesh)
	{		
		AssetDatabase.CreateAsset(mesh, ASSET_MESH_HOME+name+".asset" );
		AssetDatabase.SaveAssets();
	}
#endif
}
