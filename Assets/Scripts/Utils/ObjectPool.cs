using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoSingleton<ObjectPool>
{

	public Queue<GameObject> objectPool;

	public void Pull(GameObject obj){
		obj.SetActive(false);
		objectPool.Enqueue (obj);
		obj.transform.parent = gameObject.transform;
	}

	public GameObject getObject(){
		return objectPool.Dequeue ();
	}
}