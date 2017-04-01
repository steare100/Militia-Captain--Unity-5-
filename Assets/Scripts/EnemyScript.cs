using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour {

	public float EnemyHealth;

	Animator enemyAnim;
	Rigidbody rb;
	NavMeshAgent agent;

	GameObject player;
	public GameObject attackOrigin;
	public GameObject weapon;

	GameObject attacker;

	public bool isDead;
	bool hasTarget;

	void Start() {
		enemyAnim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		agent = GetComponent<NavMeshAgent> ();
		isDead = false;

	}

	void OnCollisionEnter(Collision coll) {
		if (coll.gameObject.tag == "Weapon") {
			EnemyHealth -= 10f;
		}
	}

	void Update() {
		
		if (agent != null) {
			if (!hasTarget) {
				player = DesignateTarget (500f);
				if (agent != null) {
					agent.SetDestination (player.transform.position);
				}
				player.SendMessageUpwards ("AssignAttacker", gameObject, SendMessageOptions.DontRequireReceiver);

			}


			float v = agent.speed;

			if (!IsInFightingRange (player.transform)) {
				RotateTowardsPlayer (player.transform);
				enemyAnim.SetFloat ("vertical", v);
			} else {
				enemyAnim.SetFloat ("vertical", 0);
			}
			if (EnemyHealth <= 0f) {
				EnemyDeath ();
			}
		
		}

		if (IsInFightingRange (player.transform)) {
			Invoke("EnemyPrep", 1f);
			Invoke ("EnemyAttack", 2f);
			return;
		} else {
			enemyAnim.SetBool ("leftHeld", false);
		}


	}

	void EnemyTakeDamage(float damage) {
		EnemyHealth -= damage;
	}

	void RotateTowardsPlayer(Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*agent.angularSpeed);
	}

	bool IsInFightingRange(Transform target) {
		float distance = Vector3.Distance (target.position, transform.position);
		return distance <= 6f;
	}


	void EnemyDeath() {
		Instantiate (Resources.Load ("Ragdoll") as GameObject, transform);
		Instantiate(Resources.Load ("Sword") as GameObject, transform);
		Destroy (gameObject.GetComponentInChildren<SkinnedMeshRenderer> ());
		Destroy (gameObject.GetComponent<CapsuleCollider> ());
		Destroy (weapon);
		Destroy (agent);
		isDead = true;
		//Destroy (this);

	}

	void EnemyAttack() {
		enemyAnim.SetTrigger ("LeftHit");
		RaycastHit hit;
		Debug.DrawRay (attackOrigin.transform.position, new Vector3(0f,0f,-1f), Color.green);
		if (Physics.Raycast (attackOrigin.transform.position, new Vector3(0f,0f,-1f), out hit, 50f, LayerMask.GetMask("Friendly"))) {
			//Debug.Log (hit.collider.gameObject.name);
			if (hit.collider.gameObject.tag == "Friendly") {
				hit.collider.gameObject.SendMessage ("FriendlyTakeDamage", 7f);
			}
		}	
	}

	void EnemyPrep() {
		enemyAnim.SetBool ("leftHeld", true);
	}

	GameObject DesignateTarget(float searchRange) {
		GameObject[] friendlies = GameObject.FindGameObjectsWithTag ("Friendly");
		float distance;
		float currentDistance = searchRange;
		GameObject target = null;
		foreach (GameObject friendly in friendlies) {
			distance = Vector3.Distance (gameObject.transform.position, friendly.transform.position);
				if(distance < currentDistance) {
					currentDistance = distance;
					target = friendly;
			}
		}
		return target;
	}

	void AssignAttacker(GameObject _attacker) {
		attacker = _attacker;
	}
}
