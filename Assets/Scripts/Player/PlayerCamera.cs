using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	GameObject player;

	PlayerControls playerControls;

	bool TryFindPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null) playerControls = player.GetComponent<PlayerControls>();

		return player != null;
	}

	private void Update()
	{
		if (player == null)
		{
			bool plyrFound = TryFindPlayer();
			if (!plyrFound) return;
		}

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
