using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSoundManager : MonoBehaviour {

	private AudioSource sfx_FootSteps;
	// Use this for initialization
	void Start () {
		AudioSource[] sounds = GetComponents<AudioSource>();
		sfx_FootSteps = sounds[0];
		FootStepsPlay();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FootStepsPlay()
	{
		sfx_FootSteps.Play();
	}

	public void FootStepsStop()
	{
		sfx_FootSteps.Stop();
	}
}
