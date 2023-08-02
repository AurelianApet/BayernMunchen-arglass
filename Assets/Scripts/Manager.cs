using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	public GameObject logo;
	public GameObject effects;
	public GameObject audio;

	public GameObject video;
	private VideoPlaybackBehaviour m_video;

	// Use this for initialization
	void Start () {
		m_video = video.transform.GetComponent<VideoPlaybackBehaviour> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.gameObject.name == "Bayer_emblem_fbx") {
					logo.SetActive (false);
					effects.SetActive (false);
					audio.GetComponent<AudioSource>().Stop();
					video.SetActive (true);
				}
			}
		}
		logo.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f));
	}
}
