using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour {

	public string teamName;
	public float baseHealth;

	void Start () {
		
		if (teamName == "BLUE") {
			InvokeRepeating ("SpawnFriendlies", 1f, 20f);
		} else if (teamName == "RED") {
			InvokeRepeating ("SpawnEnemies", 1f, 20f);
		}
	}

	void Update () {
		if (baseHealth <= 0) {
			DestroyBase ();
		}



	}

	void SpawnFriendlies() {
		Instantiate (Resources.Load ("Friendly"), transform.position, Quaternion.identity);
		Instantiate (Resources.Load ("Friendly"), transform.position + new Vector3(1,0,1), Quaternion.identity);
		Instantiate (Resources.Load ("Friendly"), transform.position + new Vector3(-1,0,-1), Quaternion.identity);
	}

	void SpawnEnemies() {
		Instantiate (Resources.Load ("Enemy"), transform.position, Quaternion.identity);
		Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3(1,0,1), Quaternion.identity);
		Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3(-1,0,-1), Quaternion.identity);
		Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3(-1,0,1), Quaternion.identity);
		Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3(1,0,-1), Quaternion.identity);
	}

	void DestroyBase() {
		
	}
}
