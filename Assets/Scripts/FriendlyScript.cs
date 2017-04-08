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
	bool IsInFightingRange;

	void Start() {
		agent = GetComponent<NavMeshAgent> ();	
		friendlyAnim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		isDead = false;
	}


	void OnTriggerEnter(Collider coll) {
		if (coll.tag == "Weapon") {
			friendlyHealth -= 50;
		}
	
	}


	void Update() {

		if (friendlyHealth <= 0) {
			FriendlyDeath ();
		}
		if (agent != null) {
			/*If the enemy doesn't have a friendly to move to and fight:
				Find someone new to move to and attack, or if you cant find one
					stand still and don't animate

			*/

			if (!hasTarget) {
				target = DesignateTarget (500f);
				if (target.GetComponent<CapsuleCollider> () != null && target.GetComponent<EnemyScript>().isDead == false) {
					hasTarget = true;
				} else {
					friendlyAnim.SetFloat ("vertical", 0);
					hasTarget = false;
				}
			} else {


				if (!IsInFightingRange) {
					if (target.GetComponent<CapsuleCollider> () == null) {
						hasTarget = false;
					} else {
						agent.SetDestination (target.transform.position);
						friendlyAnim.SetFloat ("vertical", 1f);
					}
					if (Vector3.Distance (gameObject.transform.position, target.transform.position) <= agent.stoppingDistance) {
						IsInFightingRange = true;
						friendlyAnim.SetFloat ("vertical", 0);
					} else {
						IsInFightingRange = false;
						friendlyAnim.SetFloat ("vertical", 1f);
					}
				} else {
					//if enemy is close enough to fight
					//checks if enemy is close enough to fight
					if (Vector3.Distance (gameObject.transform.position, target.transform.position) <= agent.stoppingDistance) {
						IsInFightingRange = true;
						friendlyAnim.SetFloat ("vertical", 0);
					} else {
						IsInFightingRange = false;
						friendlyAnim.SetFloat ("vertical", 1f);
					}

					if (target.GetComponent<CapsuleCollider> () == null) {
						CancelInvoke ();
						hasTarget = false;
						IsInFightingRange = false;
						friendlyAnim.SetBool ("leftHeld", false);
					} else {
						Invoke ("FriendlyPrep", 1f);
						Invoke ("FriendlyAttack", 2f);
						friendlyAnim.SetFloat ("vertical", 0);
					}

				}


			}
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

	bool CheckIfInFightingRange(Transform target) {
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
		gameObject.tag = "Dead";
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

	GameObject DesignateTarget(float searchRange) {
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
		
}
