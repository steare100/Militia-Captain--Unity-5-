using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour {

	Animator anim;
	public GameObject playerAttackOrigin;
	public float playerHealth;
	public GameObject weaponOne;
	public GameObject weaponTwo;

	void Start() {
		anim = GetComponent<Animator> ();
	}

	void OnTriggerEnter(Collider coll) {
		if (coll.tag == "Weapon") {
			playerHealth -= 50f;
			anim.SetTrigger ("GetHit");
			Debug.Log ("Player has taken damage");
		}
	}

	void Update() {
		Debug.DrawRay (playerAttackOrigin.transform.position, Vector3.forward, Color.green);
		if (Input.GetButton ("Fire2")) {
			anim.SetBool ("isBlocking", true);
		} else {
			anim.SetBool ("isBlocking", false);
		}



		if (Input.GetButton ("Fire1")) {
			anim.SetBool ("leftHeld", true);
		} 

		if (Input.GetButtonUp ("Fire1")) {
			anim.SetBool ("leftHeld", false);
			anim.SetTrigger ("LeftHit");
		}

		if (playerHealth <= 0) {
			PlayerDeath ();
		}
	}

	void PlayerDeath() {
		Destroy (gameObject.GetComponentInChildren<SkinnedMeshRenderer> ());
		Instantiate (Resources.Load ("Ragdoll"), transform.position, Quaternion.identity);
		Destroy (gameObject.GetComponent<Rigidbody> ());
		Destroy (weaponOne);
		Destroy (weaponTwo);
		Destroy (this);
	}
}
