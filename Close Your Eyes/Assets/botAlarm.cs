using UnityEngine;
using System.Collections;
using VolumetricLines;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class botAlarm : MonoBehaviour {
	public int Type;
	public float FOV;
	public float laserSpeed;
	public int onFor;
	public int offFor;
	public Material laserMat;
	public AudioClip alarm1;
	public AudioClip alarm2;
	AudioSource alarmSource1;
	AudioSource alarmSource2;

	public AudioClip generatorOn;
	public AudioClip generatorOff;
	public AudioClip generatorSus;
	AudioSource generatorSource;
	AudioSource generatorSource2;
	AudioSource generatorSource3;

	Ray botRay = new Ray();
	RaycastHit rayHit;
	GameObject laser;
	Vector3 laserOrigin;
	VolumetricLineBehavior volLine;

	int onOrOff = 1;

	int laserDirec = 1;
	float laserAngle = 0f;
	float ded = 0;

	// Use this for initialization
	void Start () {
		laser = new GameObject();
		laser.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = laser.AddComponent<MeshRenderer>();
		meshRenderer.material = laserMat;

		laserOrigin = transform.parent.transform.position;
		laserOrigin.x -= 2.2f;
		laserOrigin.y += 1f;
		laserOrigin.z += 1.2f;

		volLine = laser.AddComponent<VolumetricLineBehavior>();
		volLine.SetLinePropertiesAtStart = true;
		volLine.LineColor = Color.red;
		volLine.LineWidth = 0.1f;

		volLine.StartPos = laserOrigin;
		volLine.EndPos = laserOrigin;

		//laser.GetComponent<Mesh> ().bounds = new Bounds (new Vector3 (), Vector3.one * 100);

		alarmSource1 = gameObject.AddComponent<AudioSource>();
		alarmSource2 = gameObject.AddComponent<AudioSource>();

		alarmSource1.clip = alarm1;
		alarmSource2.clip = alarm2;
		alarmSource1.volume = .5f;
		alarmSource2.volume = .5f;

		float pitch = Random.Range (0.5f, 1.5f);
		generatorSource = gameObject.AddComponent<AudioSource>();
		generatorSource.clip = generatorSus;
		generatorSource.volume = .2f;
		generatorSource.pitch = pitch;

		generatorSource2 = gameObject.AddComponent<AudioSource>();
		generatorSource2.clip = generatorOn;
		generatorSource2.volume = .2f;
		generatorSource2.pitch = pitch;

		generatorSource3 = gameObject.AddComponent<AudioSource>();
		generatorSource3.clip = generatorOff;
		generatorSource3.volume = .2f;
		generatorSource3.pitch = pitch;
	}
	
	// Update is called once per frame
	void Update () {
		if (onOrOff > 0) {
			onOrOff++;
			
			laserAngle += laserDirec * laserSpeed;
			if (Mathf.Abs (laserAngle) >= FOV / 2f) {
				laserDirec *= -1;
			}
			botRay = new Ray (laserOrigin, Quaternion.Euler (0, laserAngle, 0) * (Quaternion.Euler (4, 304, 6) * (laserOrigin.normalized + Vector3.left)).normalized);
			Physics.Raycast (botRay, out rayHit);

			if (rayHit.transform != null) {
				volLine.EndPos = rayHit.point;
				if (rayHit.collider.gameObject.name == "playerCollider") {
					if (ded == 0) {
						ded = Time.time;
						alarmSource1.pitch = Random.Range (0.5f, 1.5f);
						alarmSource1.Play ();
						alarmSource2.pitch = Random.Range (0.5f, 1.5f);
						alarmSource2.Play ();
					}
				}
			}

			if (onOrOff > onFor) {
				onOrOff = -1;
				laser.SetActive (false);
				generatorSource3.Play ();
				generatorSource.Stop ();
			} else if (!generatorSource.isPlaying) {
				generatorSource.Play ();
			}
		} else {
			onOrOff--;

			if (onOrOff < -offFor) {
				onOrOff = 1;
				laser.SetActive (true);
				generatorSource2.Play ();
			}
		}
	
		if (ded > 0 && ded + 5 < Time.time) {
			alarmSource1.Stop ();
			alarmSource2.Stop ();
			GameObject.FindWithTag ("Player").transform.position = new Vector3 (0, GameObject.Find ("Player").transform.position.y, 0);
			GameObject.FindWithTag ("Player").transform.rotation = new Quaternion ();
			ded = 0;
		}
	}
}
