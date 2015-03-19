using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestEditorMenues : Editor {

	[MenuItem("test/testLineMesh")]
	public static void testLineMesh(){
		Mesh mesh=MeshUtil.createSquareSideLineMesh(new IntVector2(0,0), new IntVector2(100,200),15f);
		MeshUtil.saveSharedMeshAsset("testLinemesh",mesh);
	}
}
