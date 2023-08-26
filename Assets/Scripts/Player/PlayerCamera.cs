using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	GameObject player;

	PlayerControls playerControls;

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		playerControls = player.GetComponent<PlayerControls>();
	}

	private void Update()
	{
		if (playerControls.inputControl)
		{
			transform.position = new Vector3(
				player.transform.position.x,
				player.transform.position.y,
				transform.position.z
			);
		}
	}
}
