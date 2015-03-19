using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;
using System.Reflection.Emit;


public class ProfilerHack : MonoBehaviour {
	[MenuItem("Window/Hack Profiler")]
	public static void doIt(){
		Type createBuiltinWindowsType = null;
		Type profilerWindowType = null;
		foreach (Assembly a in  AppDomain.CurrentDomain.GetAssemblies()){

			if (a.GetName().Name.Equals ( "UnityEditor")){
				foreach (Type t in  a.GetTypes()){
					if (t.Name.Equals("CreateBuiltinWindows"))
						createBuiltinWindowsType = t;
					
					if (t.Name.Equals("ProfilerWindow"))
						profilerWindowType = t;

					if (profilerWindowType!=null && createBuiltinWindowsType!=null)
						break;
				}
			}
			if (profilerWindowType!=null && createBuiltinWindowsType!=null)
				break;
		}
		if (createBuiltinWindowsType==null){
			Debug.LogError("something wrong, can't find editor assembly");
		}

		//MethodInfo m =  createBuiltinWindowsType.GetMethod("ShowProfilerWindow", BindingFlags.NonPublic | BindingFlags.Static);
		//m.Invoke(null,null);

		EditorWindow w = EditorWindow.GetWindow( profilerWindowType);
		Debug.Log(" w is null = " + (w == null));
		FieldInfo fi= profilerWindowType.GetField("m_HasProfilerLicense", BindingFlags.NonPublic | BindingFlags.Instance );
		fi.SetValue(w,true);
		Debug.Log(fi.GetValue(w));

	}
}
