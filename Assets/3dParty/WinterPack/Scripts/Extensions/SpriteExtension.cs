using UnityEngine;
using System.Collections;

public static class SpriteExtension {

	public static SpriteRenderer createGameObject(this Sprite sprite, int sortingOrder){
		GameObject go = new GameObject(sprite.name);
		SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
		sr.sprite = sprite;
		sr.sortingOrder = sortingOrder;
		return sr;
	}

}
