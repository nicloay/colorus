using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class IntVector2 : IComparable {
	public int x;
	public int y;



	public static IntVector2 operator +(IntVector2 v1, IntVector2 v2){
		return new IntVector2(v1.x+v2.x,v2.y+v2.y);	
	}
	
	public static IntVector2 operator -(IntVector2 v1, IntVector2 v2){
		return new IntVector2(v1.x-v2.x,v2.y-v2.y);	
	}

	public IntVector2():this(0,0)
	{
	}

	public IntVector2(IntVector2 src){
		x = src.x;
		y = src.y;
	}

	public IntVector2 (int x, int y) {
		this.x = x;
		this.y = y;		
	}
	
	public Vector3 toVector3(float z=0){	
		return new Vector3(x,y,z);	
	}
	
	public Vector2 toVector2(){
		return new Vector2(x,y);	
	}
	
	public IntVector2 (Vector2 vector2) {
		x = Mathf.RoundToInt (vector2.x);
		y = Mathf.RoundToInt (vector2.y);
	}
	
	public IntVector2 (float x, float y) {
		this.x = Mathf.RoundToInt (x);
		this.y = Mathf.RoundToInt (y);
	}
	
	int sqrMagnitude {
		get { return x * x + y * y; }
	}
	
	public static IntVector2 arrayToIntVector2 (int[] arr) {
		return new IntVector2 (arr [0], arr [1]);
	}
	
	
	
	#region IComparable implementation
	int IComparable.CompareTo (object obj) {
		int xoffset = 1000000;
		IntVector2 objjjj = (IntVector2)obj;
		int thisMagnitude = x * xoffset + y;
		int objMagnitude = objjjj.x * xoffset + y;
		
		return (thisMagnitude - objMagnitude);
	}
	#endregion

	public override String ToString () {
		
		return "[" + x + "," + y + "]";
	}
	
	public bool equalsTo (IntVector2 vector) {
		return (x == vector.x && y == vector.y);
	}
	
	public float lengthTo (IntVector2 vector2) {
		return Mathf.Sqrt ((float)((this.x - vector2.x) * (this.x - vector2.x)) + (float)((this.y - vector2.y) * (this.y - vector2.y)));		
	}
}
