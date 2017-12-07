using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

	// keep track of game elements which can pick this up
	public string targetTag = "Player";

	void OnTriggerEnter2D(Collider2D target) {
		if (target.gameObject.tag == targetTag) {
			OnCollect (target.gameObject);
			OnDestroy ();
		}

	}


	// Use this for initialization
	protected virtual void OnCollect (GameObject target) {
	
	}
	
	// Update is called once per frame
	protected virtual void OnDestroy () {
		Destroy (gameObject);
	}
}
