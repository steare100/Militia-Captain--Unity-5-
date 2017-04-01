using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyScript : MonoBehaviour {

	float range;
	bool hasTarget;
	GameObject target;
	NavMeshAgent agent;
	Animator friendlyAnim;
	public GameObject weapon;
	Rigidbody rb;
	public float friendlyHealth;
	public GameObject attackOrigin;
	GameObject attacker;

	public bool isDead;

	void Start() {
		agent = GetComponent<NavMeshAgent> ();	
		friendlyAnim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		isDead = false;
	}

	void Update() {
		if (!hasTarget) {
			target = FindClosestEnemy (500f);
			if (target != null) {
				agent.SetDestination (target.transform.position);
				target.SendMessageUpwards ("AssignAttacker", gameObject, SendMessageOptions.DontRequireReceiver);
			} 
		}
			
		float v = agent.speed;
		if (agent.remainingDistance > agent.stoppingDistance) {
			RotateTowardsEnemy (target.transform);
			friendlyAnim.SetFloat ("vertical", v);
		} else {
			friendlyAnim.SetFloat ("vertical", 0);
		}
		if (friendlyHealth <= 0f) {
			FriendlyDeath ();
		}

 		if (!target.GetComponent<EnemyScript>().isDead && IsInFightingRange (target.transform)) {
			Invoke("FriendlyPrep", 1f);
			Invoke ("FriendlyAttack", 2f);

		} else {
			CancelInvoke ();
			friendlyAnim.SetBool ("leftHeld", false);
		}

	}

	void FriendlyTakeDamage(float damage) {
		friendlyHealth -= damage;
	}

	void RotateTowardsEnemy(Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*agent.angularSpeed);
	}

	bool IsInFightingRange(Transform target) {
		float distance = Vector3.Distance (target.position, transform.position);
		return distance <= agent.stoppingDistance;
	}


	GameObject FindClosestEnemy(float searchRange) {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		float distance;
		float currentDistance = searchRange;
		GameObject target = null;
		foreach (GameObject enemy in enemies) {
			distance = Vector3.Distance (gameObject.transform.position, enemy.transform.position);
			if(distance < currentDistance) {
				currentDistance = distance;
				target = enemy;
			}
		}
		return target;
	}

	void FriendlyDeath() {
		Instantiate (Resources.Load ("Ragdoll") as GameObject, transform);
		Instantiate(Resources.Load ("Sword") as GameObject, transform);
		Destroy (gameObject.GetComponentInChildren<SkinnedMeshRenderer> ());
		Destroy (gameObject.GetComponent<CapsuleCollider> ());
		Destroy (weapon);
		Destroy (agent);
		Destroy (this);

	}	

	void FriendlyAttack() {
		friendlyAnim.SetTrigger ("LeftHit");
		RaycastHit hit;
		//Debug.DrawRay (attackOrigin.transform.position, new Vector3(0f,0f,1f), Color.green);
		if (Physics.Raycast (attackOrigin.transform.position, new Vector3(0f,0f,1f), out hit, 50f, LayerMask.GetMask("Enemy"))) {
			//Debug.Log (hit.collider.gameObject.name);
			if (hit.collider.gameObject.tag == "Enemy") {
				hit.collider.gameObject.SendMessage ("EnemyTakeDamage", 7f);
			}
		}
	}

	void FriendlyPrep() {
		friendlyAnim.SetBool ("leftHeld", true);
	}

	void AssignAttacker(GameObject _attacker) {
		attacker = _attacker;
	}
}
