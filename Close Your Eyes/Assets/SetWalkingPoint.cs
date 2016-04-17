using UnityEngine;
using System.Collections;

public class SetWalkingPoint : MonoBehaviour {
	Ray viewRay = new Ray();
	RaycastHit rayHit;
	public GameObject walkIndicatorPrefab;
	GameObject walkIndicator;
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

	bool placeSelected = false;

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

			}
		} else if (!placeSelected) {
			walkIndicator.SetActive (false);

		} else {
			walkIndicator.transform.GetChild (0).transform.Rotate (0, 0, 1); 
			player.transform.position = Vector3.MoveTowards(player.transform.position, newPos, Time.deltaTime);
			human.transform.rotation = Quaternion.Slerp (human.transform.rotation, newRot, Time.deltaTime);
			Debug.Log (human.transform.rotation);
			Debug.Log (newRot);
			if (player.transform.position == newPos || Input.GetMouseButtonUp (0)) {
				placeSelected = false;
				walkIndicator.SetActive (false);
			}
		}

		if (rayHit.transform != null && rayHit.collider.gameObject.name == "playerCollider") {
			if (startedLookingAtPlayer == 0) {
				startedLookingAtPlayer = Time.time;
			} else {
				topAnim.SetFloat("speedMult", 1f);
				bottomAnim.SetFloat("speedMult", 1f);
				topAnim.Play ("TopLidSlide", 1, 0);
				bottomAnim.Play ("New Animation", 1, 0);
				Debug.Log ("HERE");

				humanColor.a = Mathf.Lerp (humanColor.a, .3f, Time.deltaTime*2); 
				humanModel.GetComponent<Renderer> ().material.SetColor ("_Color", humanColor);
				playerInvisible = true;
			}

		} else if (startedLookingAtPlayer != 0) {
			startedLookingAtPlayer = 0;
			humanColor.a = 1; 
			humanModel.GetComponent<Renderer> ().material.SetColor("_Color", humanColor);

			topAnim.SetFloat("speedMult", -1f);
			bottomAnim.SetFloat("speedMult", -1f);
			topAnim.Play ("TopLidSlide");
			bottomAnim.Play ("New Animation");
			playerInvisible = false;
		}
			
	}
}
