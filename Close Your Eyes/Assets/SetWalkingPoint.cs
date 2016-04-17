using UnityEngine;
using System.Collections;

public class SetWalkingPoint : MonoBehaviour {
	Ray viewRay = new Ray();
	RaycastHit rayHit;
	public GameObject walkIndicatorPrefab;
	public GameObject walkIndicator;
	Vector3 newPos;
	Quaternion newRot;
	GameObject player;
	GameObject human;
	GameObject humanModel;

	GameObject topLid;
	GameObject bottomLid;
	Animator topAnim;
	Animator bottomAnim;

	float startedLookingAtPlayer = 0;
	public bool playerInvisible = false;
	Color humanColor;

	public bool placeSelected = false;
	public bool moving = false;

	// Use this for initialization
	void Start () {
		walkIndicator = Instantiate (walkIndicatorPrefab, new Vector3 (), Quaternion.Euler(90, 0, 0)) as GameObject;
		player = GameObject.FindWithTag ("Player");
		human = GameObject.FindWithTag ("Human");
		humanModel = GameObject.Find ("HumanMale");
		humanColor = humanModel.GetComponent<Renderer> ().material.GetColor("_Color");

		topLid = GameObject.Find ("TopLid");
		topAnim = topLid.GetComponent<Animator> ();
		bottomLid = GameObject.Find ("BottomLid");
		bottomAnim = bottomLid.GetComponent<Animator> ();

		topAnim.SetFloat("speedMult", 0f);
		bottomAnim.SetFloat("speedMult", 0f);

		//this.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y + 3, player.transform.position.z - 1);
		this.transform.Rotate (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		viewRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		Physics.Raycast (viewRay, out rayHit);

		if (rayHit.transform != null && rayHit.collider.gameObject.name == "playerCollider" && !moving && startedLookingAtPlayer >= 0) {
			Debug.Log (startedLookingAtPlayer);
			Debug.Log (Time.time);

			if (startedLookingAtPlayer == 0) {
				startedLookingAtPlayer = Time.time;
			} else if (Time.time - startedLookingAtPlayer < 5) {
				topAnim.SetFloat ("speedMult", 1f);
				bottomAnim.SetFloat ("speedMult", 1f);
				topAnim.speed = 1;
				bottomAnim.speed = 1;
				if (!(topAnim.GetCurrentAnimatorStateInfo (0).normalizedTime > 1)) {
					topAnim.Play ("TopLidSlide");
					bottomAnim.Play ("New Animation");
				}
				humanColor.a = Mathf.Lerp (humanColor.a, .1f, Time.deltaTime * 1.8f);
				humanModel.GetComponent<Renderer> ().material.SetFloat ("_Mode", 3);
				humanModel.GetComponent<Renderer> ().material.SetColor ("_Color", humanColor);

				if (humanColor.a <= .2) {
					playerInvisible = true;

				}
			} else {
				startedLookingAtPlayer = -200;
				humanColor.a = 1; 
				humanModel.GetComponent<Renderer> ().material.SetFloat ("_Mode", 0);
				humanModel.GetComponent<Renderer> ().material.SetColor ("_Color", humanColor);

				topAnim.SetTime (0);
				bottomAnim.SetTime(0);
				playerInvisible = false;
			}

		} else {
			humanColor.a = 1; 
			humanModel.GetComponent<Renderer> ().material.SetFloat ("_Mode", 0);
			humanModel.GetComponent<Renderer> ().material.SetColor ("_Color", humanColor);

			topAnim.SetTime (0);
			bottomAnim.SetTime(0);
			playerInvisible = false;
			if (startedLookingAtPlayer < 0) {
				startedLookingAtPlayer++;
			}
		}

		if (rayHit.transform != null && rayHit.collider.gameObject.name == "Floor" &&
		    Vector3.Distance (Camera.main.transform.position, rayHit.point) < 10 && !placeSelected) {

			newPos = new Vector3 (rayHit.point.x, rayHit.point.y + .01f, rayHit.point.z);
			if (rayHit.point.x + .3f > rayHit.collider.bounds.center.x + rayHit.collider.bounds.extents.x) {
				newPos.x = rayHit.collider.bounds.center.x + rayHit.collider.bounds.extents.x - .3f;
			} else if (rayHit.point.x - .3f < rayHit.collider.bounds.center.x - rayHit.collider.bounds.extents.x) {
				newPos.x = rayHit.collider.bounds.center.x - rayHit.collider.bounds.extents.x + .3f;
			}
			if (rayHit.point.z + .3f > rayHit.collider.bounds.center.z + rayHit.collider.bounds.extents.z) {
				newPos.z = rayHit.collider.bounds.center.z + rayHit.collider.bounds.extents.z - .3f;
			} else if (rayHit.point.z - .3f < rayHit.collider.bounds.center.z - rayHit.collider.bounds.extents.z) {
				newPos.z = rayHit.collider.bounds.center.z - rayHit.collider.bounds.extents.z + .3f;
			}
			walkIndicator.transform.position = newPos;
			newPos.y = player.transform.position.y;
			newPos.z += .7f;
			newRot = Quaternion.LookRotation (newPos - human.transform.position);
			newRot.x = human.transform.rotation.x;
			newRot.z = human.transform.rotation.z;
			walkIndicator.transform.GetChild (0).transform.Rotate (0, 0, 1);
			walkIndicator.transform.GetChild (1).transform.Rotate (0, 0, -1);
			walkIndicator.SetActive (true);

			if (Input.GetMouseButtonUp (0)) {
				placeSelected = true;
				moving = true;
			}
		} else if (!placeSelected) {
			walkIndicator.SetActive (false);

		} else if (moving) {
			walkIndicator.transform.GetChild (0).transform.Rotate (0, 0, 1); 
			player.transform.position = Vector3.MoveTowards(player.transform.position, newPos, Time.deltaTime*1.2f);
			human.transform.rotation = Quaternion.Slerp (human.transform.rotation, newRot, Time.deltaTime);
			if (player.transform.position == newPos || Input.GetMouseButtonUp (0)) {
				placeSelected = false;
				moving = false;
				walkIndicator.SetActive (false);
			}
		}


			
	}
}
