using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class SnowSpriteItem{
	public Sprite sprite;
	public int totalNumber;
	public float averageSpeed;
	public float speedBias;

	[NonSerialized]
	public Queue<GameObject> cache;

	GameObject mainGO;

	public void createGameObjects(Transform parent, Vector2 camSize){
		camSize.x += sprite.rect.width;
		camSize.y += sprite.rect.height;
		if (mainGO == null){
			mainGO = new GameObject();
			mainGO.name = sprite.name;
			mainGO.transform.parent = parent;
			mainGO.transform.localPosition = Vector3.zero;
		}
		float x,y;

		Queue<GameObject> newCache = new Queue<GameObject>();
		for (int i = 0; i < totalNumber; i++) {
			GameObject snowFlakeGO;
			if (cache != null && cache.Count > 0){
				snowFlakeGO = cache.Dequeue();
			} else {
				snowFlakeGO= new GameObject();
				SpriteRenderer sr = snowFlakeGO.AddComponent<SpriteRenderer>();
				snowFlakeGO.transform.parent = mainGO.transform;
				sr.sprite = sprite;
			}
			snowFlakeGO.name = "snowFlake"+i;

			x = UnityEngine.Random.Range(-camSize.x, camSize.x);
			y = UnityEngine.Random.Range(-camSize.y, camSize.y);
			snowFlakeGO.transform.localPosition = new Vector3(x, y, 0);
			newCache.Enqueue(snowFlakeGO);
		}

		if (cache != null){
			while (cache.Count > 0)
				GameObject.Destroy(cache.Dequeue());
		}
		cache = newCache;
	}
}

public class SnowFall : MonoBehaviour {
	public SnowSpriteItem[] snowSpriteItems;

	public Camera cam;

	void Awake(){
		if (cam == null)
			cam = Camera.main;
	}

	public void GenerateSnowFall () {
		initSnowSpritesParameters();
		Vector3 position = cam.transform.position;
		position.z += 10;
		transform.position = position;
	}

	void initSnowSpritesParameters(){
		Vector2 camSize = new Vector2(cam.getOrthographicHorizontalSize(), cam.orthographicSize);
		for (int i = 0; i < snowSpriteItems.Length; i++) {				
				snowSpriteItems[i].createGameObjects(transform, camSize);
		}
	}
	
}
