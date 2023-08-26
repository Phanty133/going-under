using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
	public AudioClip hitClip;
	public float hitPlayDelay_s = 0.3f;

	float hitTimer = 0;


	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
		{
			if (hitTimer > 0) return;
			Vector3 hitPos = other.contacts[0].point;
			hitPos.z = Camera.main.transform.position.z;

			AudioSource.PlayClipAtPoint(hitClip, hitPos);
			hitTimer = hitPlayDelay_s;
		}
	}

	private void Update()
	{
		if (hitTimer < 0)
		{
			hitTimer = 0;
		}
		else if (hitTimer == 0) return;

		hitTimer -= Time.deltaTime;
	}
}
