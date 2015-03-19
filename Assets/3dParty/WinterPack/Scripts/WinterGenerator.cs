using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof( SnowFall))]
public class WinterGenerator : MonoBehaviour {
	public Camera cam;
	public Sprite[] snowDriftSprites;
	public int snowDriftHorizontalOverflowMin = 1;
	public int snowDriftHorizontalOverflowMax = 20;
	public SnowFall snowFall;

	public Sprite[] snowFlakes;


	void Awake(){
		snowFall = GetComponent<SnowFall>();
		if (cam == null)
			cam = Camera.main;
		Vector3 camPosition = cam.transform.position;
		camPosition.z += 20;
		transform.position = camPosition;	
	}

	GameObject snowDriftsGO;
	Queue<SpriteRenderer> snowDriftsPool;
	public void generateSnowDrifts(){

		if (snowDriftsGO == null){
			snowDriftsGO = new GameObject();
			snowDriftsGO.name = "SnowDrifts";
			snowDriftsGO.transform.parent = transform;
			snowDriftsGO.transform.localPosition = Vector3.zero;
		}

		int zMin= 0;
		int zMax = 10;
		Sprite sprite;
		Vector3 position = Vector3.zero;
		position.y -= cam.orthographicSize;
		float camOrthoWidth = cam.getOrthographicHorizontalSize();
		position.x -= camOrthoWidth;
		bool first = true;
		SpriteRenderer snowDriftSR;
		Queue<SpriteRenderer> newPool = new Queue<SpriteRenderer>();
		int i=0;
		int z;
		do {
			if (snowDriftsPool != null && snowDriftsPool.Count > 0){
				snowDriftSR = snowDriftsPool.Dequeue();
			} else {
				z = Random.Range(zMin, zMax);
				sprite = snowDriftSprites[ Random.Range(0,snowDriftSprites.Length)];
				snowDriftSR = sprite.createGameObject(z);
				snowDriftSR.transform.parent = snowDriftsGO.transform;
			}


			snowDriftSR.gameObject.name = "SnowDrift" + i;
			if (first){
				position.x -= Random.Range(0, (int)snowDriftSR.sprite.rect.width);
				first = false;
			}
			snowDriftSR.transform.localPosition = position;
			newPool.Enqueue(snowDriftSR);

			position.x += snowDriftSR.sprite.rect.width - Random.Range(snowDriftHorizontalOverflowMin,snowDriftHorizontalOverflowMax);
			i++;
		} while (position.x < camOrthoWidth);
		if (snowDriftsPool != null){
			while (snowDriftsPool.Count >0)
				GameObject.Destroy(snowDriftsPool.Dequeue().gameObject);
		}
		snowDriftsPool = newPool;
	}

}
