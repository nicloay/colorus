using System;

public class ConverterUtils
{
	public static string convertFloatArrayToString(float[] floatArray){
		byte[] byteArray=new byte[floatArray.Length*4];
		Buffer.BlockCopy(floatArray,0,byteArray,0,byteArray.Length);
		string result=Convert.ToBase64String(byteArray);
		return result;
	}
	
	public static float[] convertStringToFloatArray(string str){
		byte[] byteArray=Convert.FromBase64String(str);
		float[] floatArray=new float[byteArray.Length/4];
		Buffer.BlockCopy(byteArray,0,floatArray,0,byteArray.Length);
		return floatArray;
	}
	
}


