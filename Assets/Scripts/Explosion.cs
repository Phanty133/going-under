using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public AudioClip explosionClip;

	ParticleSystem partSys;

	private void Start()
	{
		partSys = GetComponent<ParticleSystem>();

		Vector3 pos = transform.position;
		pos.z = Camera.main.transform.position.z;
		AudioSource.PlayClipAtPoint(explosionClip, pos);
	}

	private void Update()
	{
		if (partSys.main.duration <= partSys.time) Destroy(gameObject);
	}
}
