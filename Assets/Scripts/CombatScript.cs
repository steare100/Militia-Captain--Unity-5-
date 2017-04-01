using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour {

	Animator anim;
	public GameObject playerAttackOrigin;

	void Start() {
		anim = GetComponent<Animator> ();
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
	}
}
