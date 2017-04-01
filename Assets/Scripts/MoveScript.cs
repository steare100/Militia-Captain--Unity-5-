using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

	Animator anim;
	Transform cam;
	Rigidbody rb;

	public float speed;
	public float turnSpeed;

	Vector3 directionPos;
	Vector3 lookPos;
	Vector3 storeDir;

	public float lookIKWeight;
	public float bodyWeight;
	public float headWeight;
	public float clampWeight;
	Vector3 aimPosition;

	bool isSprinting;


	void Start() {
		anim = GetComponent<Animator> ();
		cam = Camera.main.transform;
		isSprinting = false;
		rb = GetComponent<Rigidbody> ();
	}


	void Update() {
		if (Input.GetButton ("Fire3")) {
			isSprinting = true;
			anim.SetBool ("isSprinting", true);
		} else {
			isSprinting = false;
			anim.SetBool ("isSprinting", false);
		}
		Ray ray = new Ray (cam.position, cam.forward);
		aimPosition = ray.GetPoint (100);

		transform.LookAt (new Vector3 (aimPosition.x, 1, aimPosition.z));

		if (Input.GetButtonDown ("Jump")) {
			rb.AddForce (Vector3.up * 10, ForceMode.Impulse);
			anim.SetTrigger ("Jump");
		}


	}


	void FixedUpdate () {
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		if (isSprinting) {
			//should not be able to strafe while sprinting
			//should not be able to run backwards
			if (v < 0) {
				v = 0;
			}
			transform.Translate (0, 0, v * speed*3);

		} else {
			//Not sprinting. 
			//Should be able to strafe, 
			//If walking diagonally, speed needs to be capped at 1, instead of ~1.4 since moving in both axes
			if (h != 0 && v != 0) {
				transform.Translate (h * speed * 1 / 4, 0, v * speed * 1 / 4);
			} else {
				transform.Translate (h*speed, 0, v*speed);
			}
			anim.SetFloat ("horizontal", h);
			anim.SetFloat ("vertical", v);

			RaycastHit hit;
			if (Physics.Raycast (transform.position, new Vector3(0,-1,0), 5f)) {
				anim.SetBool ("isGrounded", true);
			}
		}
	}


}
